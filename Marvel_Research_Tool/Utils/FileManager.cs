using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Marvel_Research_Tool.Data_Model;

namespace Marvel_Research_Tool.Utils
{
    public class FileManager
    {
        // Default values
        public const string DEFAULT_DATA_DIR = @"D:\Sandbox\mcp";
        public const string DEFAULT_KEY_PATH = @"_key\mcp_key.csv";
        public const string DEFAULT_SALES_DIR = "_sales";
        public const string DEFAULT_RESULTS_DIR = "_results";

        // Result files and directories names
        // These files are in letter directories (separate file for each letter)
        public const string RESULT_CH_NAMES = "ch_names_"; // id, names
        public const string RESULT_CH_COMICS = "ch_comics_"; // id, list of comics
        public const string RESULT_CH_LINKS = "ch_links_"; // id, links
        // These RESULT files are written to the _results directory
        public const string RESULT_SERIES_COMICS_SALES = "series_comics_sales.csv"; // series, list of comics
        public const string RESULT_COMICS_SALES = "comic_sales.csv"; // comic, # sold
        public const string RESULT_CHARACTER_COMICS = "character-comics.csv";
        public const string RESULT_CHARACTER_SERIES = "character-series.csv";
        public const string RESULT_CHARACTER_CHARACTERS = "character-characters.csv";
        public const string RESULT_TRAINING_SET = "training_set.csv";
        public const string RESULT_TEST_SET = "test_set.csv";

        public const string SYSTEM_INPUT_PAGES = "input_pages.txt";
        public const string SYSTEM_LINKAGE_GRAPH = "linkage_graph.txt";
        public const string SYSTEM_BAD_MAPPING = "bad_mapping.txt";
        public const string CSV = ".csv";

        #region Get File Path Methods

        public static string GetLinksFilePath(string pathCurrentLetterDir)
        {
            string currentLetter = Path.GetFileName(pathCurrentLetterDir);
            return Path.Combine(pathCurrentLetterDir, RESULT_CH_LINKS + currentLetter + CSV);
        }

        public static string GetNamesFilePath(string pathCurrentLetterDir)
        {
            string currentLetter = Path.GetFileName(pathCurrentLetterDir);
            return Path.Combine(pathCurrentLetterDir, RESULT_CH_NAMES + currentLetter + CSV);
        }

        public static string GetComicsFilePath(string pathCurrentLetterDir)
        {
            string currentLetter = Path.GetFileName(pathCurrentLetterDir);
            return Path.Combine(pathCurrentLetterDir, RESULT_CH_COMICS + currentLetter + CSV);
        }

        public static string GetLinksFilePath(char currentLetter)
        {
            return Path.Combine(DEFAULT_DATA_DIR, currentLetter.ToString(), RESULT_CH_LINKS + currentLetter + CSV);
        }

        public static string GetNamesFilePath(char currentLetter)
        {
            return Path.Combine(DEFAULT_DATA_DIR, currentLetter.ToString(), RESULT_CH_NAMES + currentLetter + CSV);
        }

        public static string GetComicsFilePath(char currentLetter)
        {
            return Path.Combine(DEFAULT_DATA_DIR, currentLetter.ToString(), RESULT_CH_COMICS + currentLetter + CSV);
        }

        public static string GetPathResultCharComics(string pathDataDir)
        {
            return Path.Combine(pathDataDir, DEFAULT_RESULTS_DIR, RESULT_CHARACTER_COMICS);
        }

        public static string GetPathResultCharSeries(string pathDataDir)
        {
            return Path.Combine(pathDataDir, DEFAULT_RESULTS_DIR, RESULT_CHARACTER_SERIES);
        }

        public static string GetPathResultCharChars(string pathDataDir)
        {
            return Path.Combine(pathDataDir, DEFAULT_RESULTS_DIR, RESULT_CHARACTER_CHARACTERS);
        }

        public static string GetPathResultTrainingSet(string pathDataDIr)
        {
            return Path.Combine(pathDataDIr, DEFAULT_RESULTS_DIR, RESULT_TRAINING_SET);
        }

        public static string GetPathResultTestSet(string pathDataDIr)
        {
            return Path.Combine(pathDataDIr, DEFAULT_RESULTS_DIR, RESULT_TEST_SET);
        }

        #endregion Get File Path Methods

        public static HashSet<string> LoadComicsById(string id)
        {
            HashSet<string> comics = null;
            char letter = id[0];
            string input = String.Empty;
            bool flagFoundID = false;
            List<string> comicsInfo = null;

            using (StreamReader reader = new StreamReader(FileManager.GetComicsFilePath(letter)))
            {
                while (!reader.EndOfStream && !flagFoundID)
                {
                    input = reader.ReadLine();
                    if (!input.Equals(String.Empty))
                    {
                        comicsInfo = input.Split(',').ToList<string>();
                        flagFoundID = comicsInfo[0].Equals(id);
                    }
                }
            }

            if (flagFoundID)
            {
                comicsInfo.RemoveAt(0);
                comics = new HashSet<string>(comicsInfo);
            }
            else
            {
                Console.WriteLine("WARNING! Could not find the requested ID.");
            }

            return comics;
        }

        public static ComicKey GetKeyByComicName(string comicName)
        {
            ComicKey requestedKey = null;

            using (StreamReader reader = new StreamReader(Path.Combine(FileManager.DEFAULT_DATA_DIR, DEFAULT_KEY_PATH)))
            {
                bool flagFoundKey = false;

                while ((!reader.EndOfStream) && (!flagFoundKey))
                {
                    string input = reader.ReadLine();

                    if (!input.Equals(String.Empty))
                    {
                        string[] nextKeyParts = input.Split(',');
                        if (flagFoundKey = comicName.Equals(nextKeyParts[0]))
                        {
                            requestedKey = new ComicKey(nextKeyParts[0], nextKeyParts[1]);
                        }
                    }
                }
            }

            return requestedKey;
        }

        public static void WriteToCSV(List<HashSet<string>> dataToWrite, string pathWrittenFile)
        {
            using (StreamWriter writer = new StreamWriter(pathWrittenFile, false))
            {
                foreach (HashSet<string> listLinks in dataToWrite)
                {
                    bool flagFirst = true;
                    foreach (string str in listLinks)
                    {
                        if (flagFirst)
                        {
                            writer.Write(str);
                            flagFirst = false;
                        }
                        else
                        {
                            writer.Write(String.Format(",{0}", str));
                        }
                    }
                    writer.WriteLine();
                }
            }
        }

        public static void WriteToCSV(List<List<string>> dataToWrite, string pathWrittenFile)
        {
            using (StreamWriter writer = new StreamWriter(pathWrittenFile, false))
            {
                foreach (List<string> listLinks in dataToWrite)
                {
                    writer.Write(listLinks[0]);
                    for (int i = 1; i < listLinks.Count; ++i)
                    {
                        writer.Write(String.Format(",{0}", listLinks[i]));
                    }
                    writer.WriteLine();
                }
            }
        }

        public static void WriteToCSV(List<CharacterComics> dataToWrite, string pathWrittenFile)
        {
            using (StreamWriter writer = new StreamWriter(pathWrittenFile, false))
            {
                foreach (CharacterComics character in dataToWrite)
                {
                    writer.WriteLine(character.ToCSV());
                }
            }
        }

        public static void WriteToCSV(SeriesSalesData dataToWrite, string pathWrittenFile, bool groupBySeries)
        {
            using (StreamWriter writer = new StreamWriter(pathWrittenFile, false))
            {
                foreach (KeyValuePair<string, ComicsSalesData> seriesSales in dataToWrite.Sales)
                {
                    if (groupBySeries)
                    {
                        string allComicsAsString = String.Empty;
                        int totalSales = 0;

                        writer.Write(seriesSales.Key);

                        foreach (KeyValuePair<string, int> comicSales in seriesSales.Value.Sales)
                        {
                            allComicsAsString += "," + comicSales.Key;
                            totalSales += comicSales.Value;
                        }

                        writer.Write("," + totalSales);
                        writer.WriteLine(allComicsAsString);
                    }
                    else
                    {
                        foreach (KeyValuePair<string, int> comicSales in seriesSales.Value.Sales)
                        {
                            writer.WriteLine(String.Format("{0},{1},{2}", seriesSales.Key, comicSales.Key, comicSales.Value));
                        }
                    }
                }
            }
        }

        // Fill input_pages.txt with all MCP pages we need to get data from.
        // input_pages.txt will be in the same directory as the EXE file
        public static void FillInputPagesFile()
        {
            Console.WriteLine("Creating the input file . . .");

            using (StreamWriter writer = new StreamWriter(SYSTEM_INPUT_PAGES))
            {
                for (char c = 'a'; c < 'z'; ++c)
                {
                    writer.WriteLine("http://www.chronologyproject.com/" + c + ".php");
                }

                // Write instead of WriteLine in order to avoid empty line in the end
                writer.Write("http://www.chronologyproject.com/z.php");
            }

            Console.WriteLine("Input file creation finished.");
        }

        /// <summary>
        /// Returns number of entries (lines) in CSV file.
        /// </summary>
        /// <param name="pathCSVFile"></param>
        /// <returns></returns>
        public static int GetNumLinesInCSV(string pathCSVFile)
        {
            if (!File.Exists(pathCSVFile)) IOFail();

            int numLines = 0;

            using (StreamReader reader = new StreamReader(pathCSVFile))
            {
                while (!reader.EndOfStream)
                {
                    if (!reader.ReadLine().Equals(String.Empty))
                    {
                        ++numLines;
                    }
                }
            }

            return numLines;
        }

        /// <summary>
        /// Reads all data from CSV file into a 2-dimentional array
        /// </summary>
        /// <param name="pathFileToRead"></param>
        /// <returns></returns>
        public static List<List<string>> ReadFromCSV(string pathFileToRead)
        {
            if (!File.Exists(pathFileToRead)) IOFail();

            List<List<string>> readData = new List<List<string>>();

            using (StreamReader reader = new StreamReader(pathFileToRead))
            {
                while (!reader.EndOfStream)
                {
                    string input = reader.ReadLine();

                    if (!input.Equals(String.Empty))
                    {
                        readData.Add(new List<string>(input.Split(',')));
                    }
                }
            }

            return readData;
        }

        public static void ReadSalesDataFromCSV(string pathDataDir, ref SeriesSalesData seriesSalesData)
        {
            using (StreamReader reader = new StreamReader(pathDataDir))
            {
                while (!reader.EndOfStream)
                {
                    string input = reader.ReadLine();

                    if (!input.Equals(String.Empty))
                    {
                        string[] data = input.Split(',');

                        seriesSalesData.Add(data[0], new Comic(data[1], Int32.Parse(data[2])));
                    }
                }
            }
        }

        public static CharacterCharactersData ReadCharacterCharactersDataFromCSV(string pathDataDir)
        {
            Console.WriteLine("Loading character-characters data . . .");

            CharacterCharactersData data = new CharacterCharactersData();

            using (StreamReader reader = new StreamReader(GetPathResultCharChars(pathDataDir)))
            {
                while (!reader.EndOfStream)
                {
                    string input = reader.ReadLine();

                    if (!input.Equals(String.Empty))
                    {
                        string[] inputParts = input.Split(',');
                        HashSet<string> characters = new HashSet<string>();

                        for (int i = 1; i < inputParts.Length; ++i)
                        {
                            characters.Add(inputParts[i]);
                        }

                        data.Add(inputParts[0], characters);
                    }
                }
            }

            Console.WriteLine("Finished character-characters data loading.");

            return data;
        }

        public static CharacterCharactersData ReadCharacterCharactersDataFromCSV(string pathDataDir, out List<string> characterIds)
        {
            Console.WriteLine("Loading character-characters data . . .");

            CharacterCharactersData data = new CharacterCharactersData();
            characterIds = new List<string>();

            using (StreamReader reader = new StreamReader(GetPathResultCharChars(pathDataDir)))
            {
                while (!reader.EndOfStream)
                {
                    string input = reader.ReadLine();

                    if (!input.Equals(String.Empty))
                    {
                        string[] inputParts = input.Split(',');
                        HashSet<string> characters = new HashSet<string>();

                        for (int i = 1; i < inputParts.Length; ++i)
                        {
                            characters.Add(inputParts[i]);
                        }

                        data.Add(inputParts[0], characters);
                        characterIds.Add(inputParts[0]);
                    }
                }
            }

            Console.WriteLine("Finished character-characters data loading.");

            return data;
        }

        public static CharacterComicsData ReadCharacterComicsDataFromCSV(string pathDataDir)
        {
            Console.WriteLine("Loading character-comics data . . .");

            CharacterComicsData data = new CharacterComicsData();

            using (StreamReader reader = new StreamReader(GetPathResultCharComics(pathDataDir)))
            {
                while (!reader.EndOfStream)
                {
                    string input = reader.ReadLine();

                    if (!input.Equals(String.Empty))
                    {
                        string[] inputParts = input.Split(',');
                        HashSet<string> comics = new HashSet<string>();

                        for (int i = 1; i < inputParts.Length; ++i)
                        {
                            comics.Add(inputParts[i]);
                        }

                        data.Add(inputParts[0], comics);
                    }
                }
            }

            Console.WriteLine("Finished character-comics data loading.");

            return data;
        }

        public static CharacterSeriesData ReadCharacterSeriesDataFromCSV(string pathDataDir)
        {
            Console.WriteLine("Loading character-series data . . .");

            CharacterSeriesData data = new CharacterSeriesData();

            using (StreamReader reader = new StreamReader(GetPathResultCharSeries(pathDataDir)))
            {
                while (!reader.EndOfStream)
                {
                    string input = reader.ReadLine();

                    if (!input.Equals(String.Empty))
                    {
                        string[] inputParts = input.Split(',');
                        HashSet<string> series = new HashSet<string>();

                        for (int i = 1; i < inputParts.Length; ++i)
                        {
                            series.Add(inputParts[i]);
                        }

                        data.Add(inputParts[0], series);
                    }
                }
            }

            Console.WriteLine("Finished character-series data loading.");

            return data;
        }

        /// <summary>
        /// Reads the MCP KEY from CSV file.
        /// </summary>
        /// <param name="pathDataDir"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ReadKeyFromCSV(string pathDataDir)
        {
            Dictionary<string, string> key = new Dictionary<string,string>();

            using (StreamReader reader = new StreamReader(pathDataDir))
            {
                while (!reader.EndOfStream)
                {
                    string input = reader.ReadLine();

                    if (!input.Equals(String.Empty))
                    {
                        string[] dataParts = input.Split(',');

                        key.Add(dataParts[0], dataParts[1]);
                    }
                }
            }

            return key;
        }

        private static void IOFail()
        {
            Console.WriteLine("ERROR! I/O error orccured.");
            Environment.Exit(-1);
        }
    }
}
