using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace MusicPlaylistAnalyzer
{

    public class PlaylistInfo
    {
        public string Name;
        public string Artist;
        public string Album;
        public string Genre;
        public int Size;
        public int Time;
        public int Year;
        public int Plays;

        public PlaylistInfo(string name, string artist, string album, string genre, int size, int time, int year, int plays)
        {
            Name = name;
            Artist = artist;
            Album = album;
            Genre = genre;
            Size = size;
            Time = time;
            Year = year;
            Plays = plays;
        }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("MusicPlaylistAnalyzer <Music_Playlist_File_Path> <Output_File_Path>");
                Environment.Exit(1);
            }

            string playlist = args[0];
            string report = args[1];

            if (!File.Exists(report))
            {
                using (StreamWriter newReport = new StreamWriter(report))
                    File.Create(report);
            }
            else
            {
                using (StreamWriter newReport = new StreamWriter(report))
                    newReport.WriteLine(string.Empty);
            }


            List<PlaylistInfo> music = null;

            try
            {
                music = FileLoader.LoadList(playlist);
            }
            catch (Exception)
            {
                Console.WriteLine("ERROR! File could not be loaded");
                return;
            }

            var output = CreateReportOutput.CreateReport(music);

            try
            {
                using (StreamWriter outputFile = new StreamWriter(report))
                {
                    outputFile.WriteLine(output);
                }
            }

            catch (Exception)
            {
                Console.WriteLine("ERROR! No output recieved");
                Environment.Exit(3);
            }

        }
    }

    class FileLoader
    {
        static int itemsInRow = 8;

        public static List<PlaylistInfo> LoadList(string playlist)
        {
            List<PlaylistInfo> music = new List<PlaylistInfo>();

            try
            {
                using (StreamReader reader = new StreamReader(playlist))
                {
                    int currentLine = 0;
                    var firstRow = reader.ReadLine();
                    while (!(reader.EndOfStream))
                    {
                        currentLine++;

                        var line = reader.ReadLine();
                        var values = line.Split('\t');

                        if (values.Length != itemsInRow)
                        {
                            throw new Exception($"Row {currentLine} only has {values.Length} different values. The line should contain {itemsInRow} different values.");
                        }

                        try
                        {
                            string name = values[0];
                            string artist = values[1];
                            string album = values[2];
                            string genre = values[3];
                            int size = Int32.Parse(values[4]);
                            int time = Int32.Parse(values[5]);
                            int year = Int32.Parse(values[6]);
                            int plays = Int32.Parse(values[7]);
                            PlaylistInfo playlistInfo = new PlaylistInfo(name, artist, album, genre, size, time, year, plays);
                            music.Add(playlistInfo);
                        }

                        catch (Exception)
                        {
                            Console.WriteLine("ERROR! Invalid input file format.");
                        }
                    }
                }
            }

            catch (Exception)
            {
                Console.WriteLine("ERROR! No file found.");
            }

            return music;
        }
    }

    class CreateReportOutput
    {
        public static string CreateReport(List<PlaylistInfo> music)
        {
            string output = "Music Playlist Report\n\n";

            if (music.Count < 1)
            {
                output += "ERROR! No data found\n";
                return output;
            }

            output += "How many songs received 200 or more plays?\n";
            output += "How many songs are in the playlist with the genre \"Alternative\"?\n";
            output += "How many songs are in the playlist with the Genre of \"Hip-Hop/Rap\"?\n";
            output += "What songs are in the playlist from the album \"Welcome to the Fishbowl?\"\n";
            output += "What are the songs in the playlist from before 1970?\n";
            output += "What are the song names that are more than 85 characters long?\n";
            output += "What is the longest song? (longest in Time)\n";
            output += "\n\n\n";


            var playCount = from PlaylistInfo in music where PlaylistInfo.Plays >= 200 select PlaylistInfo.Plays;
            output += $"Songs that recieved 200 or more plays:";
            foreach(var twoHundredPlaysong in playCount)
                output += $"{twoHundredPlaysong}\n";
            output += "\n";

            var alternativeSongs = from PlaylistInfo in music where PlaylistInfo.Genre == "Alternative" select PlaylistInfo.Genre;
            output += $"Number of Alternative songs: {alternativeSongs.Count()}\n\n";

            var rapSongs = from PlaylistInfo in music where PlaylistInfo.Genre == "Hip-Hop/Rap" select PlaylistInfo.Genre;
            output += $"Number of Hip-Hop/Rap songs: {rapSongs.Count()}\n\n";

            var fishbowl = from PlaylistInfo in music where PlaylistInfo.Album == "Welcome to the Fishbowl" select PlaylistInfo.Name;
            output += "Songs from the album Welcome to the Fishbowl:\n";
            foreach (var song in fishbowl)
                output += $"{song}\n";
            output += "\n";

            var specificYear = from PlaylistInfo in music where PlaylistInfo.Year < 1970 select PlaylistInfo.Name;
            output += "Songs from before 1970:\n";
            foreach (var before1970 in specificYear)
                output += $"{before1970}\n";
            output += "\n";

            var longNames = from PlaylistInfo in music where PlaylistInfo.Name.Length > 85 select PlaylistInfo.Name;
            output += "Song names longer than 85 characters:\n";
            foreach (var eightyFiveCharNames in longNames)
                output += $"{eightyFiveCharNames}\n";
            output += "\n";

            int longestSong = music.Max(PlaylistInfo => PlaylistInfo.Time);
            var duration = from PlaylistInfo in music where PlaylistInfo.Time == longestSong select PlaylistInfo.Name;
            output += "Longest song:\n";
            foreach (var longestTimeName in duration)
                output += $"{longestTimeName}\n";
            output += "\n";

            return output;
        }
    }
}


