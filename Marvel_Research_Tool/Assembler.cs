using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marvel_Research_Tool.Data_Model;
using System.IO;
using Marvel_Research_Tool.Utils;

namespace Marvel_Research_Tool
{
    public class Assembler
    {
        // Create 2 files
        // 1. Series - Books
        // 2. Books - Sales
        public static void AssembleSalesData(string pathDataDir)
        {
            Console.WriteLine("Starting to assemble comics sales data . . .");

            SeriesSalesData seriesSales = new SeriesSalesData();
            string pathSalesData = Path.Combine(pathDataDir, FileManager.DEFAULT_SALES_DIR);

            foreach (string pathSalesFile in Directory.EnumerateFiles(pathSalesData, "*", SearchOption.TopDirectoryOnly))
            {
                Console.WriteLine(String.Format("Processing data from {0}.", pathSalesFile));
                FileManager.ReadSalesDataFromCSV(pathSalesFile, ref seriesSales);
            }

            Console.WriteLine("Saving assembled data . . .");

            string pathResultsDir = Path.Combine(pathDataDir, FileManager.DEFAULT_RESULTS_DIR);
            FileManager.WriteToCSV(seriesSales, Path.Combine(pathResultsDir, FileManager.RESULT_SERIES_COMICS_SALES), true);
            FileManager.WriteToCSV(seriesSales, Path.Combine(pathResultsDir, FileManager.RESULT_COMICS_SALES), false);

            Console.WriteLine(String.Format("---=== Statistics ===---\n# Series: {0}",
                              seriesSales.Count()));

            Console.WriteLine("Finished comics sales data assembling.");
        }

        /// <summary>
        /// Assembles data from MCP. Creates files that can be used for features extraction.
        /// </summary>
        /// <param name="pathDataDir"></param>
        public static void AssembleGraphData(string pathDataDir)
        {
            Console.WriteLine("Starting MCP data assembling . . .");

            CombineMCPCharactersData(pathDataDir);
            AssembleMCPCharacterSeries(pathDataDir);
            AssembleMCPCharacterCharacters(pathDataDir);

            Console.WriteLine("Finished MCP data assembling.");
        }

        /// <summary>
        /// NOTE: Always use first when assembling MCP data.
        /// </summary>
        /// <param name="pathDataDir"></param>
        private static void CombineMCPCharactersData(string pathDataDir)
        {
            Console.WriteLine("Starting characters data combining . . .");

            using (StreamWriter writer = new StreamWriter(FileManager.GetPathResultCharComics(pathDataDir), false))
            {
                foreach (string pathLetterDir in Directory.EnumerateDirectories(pathDataDir, "?", SearchOption.TopDirectoryOnly))
                {
                    Console.WriteLine(String.Format("Processing data in {0}", pathLetterDir));

                    using (StreamReader reader = new StreamReader(FileManager.GetComicsFilePath(pathLetterDir)))
                    {
                        while (!reader.EndOfStream)
                        {
                            string input = reader.ReadLine();

                            if (!input.Equals(String.Empty))
                            {
                                string[] charComics = input.Split(',');

                                writer.Write(charComics[0]);
                                for (int i = 1; i < charComics.Length; ++i)
                                {
                                    if (TestManager.TestOneMapping(charComics[i]))
                                    {
                                        writer.Write(String.Format(",{0}", charComics[i]));
                                    }
                                }
                                writer.WriteLine();
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Finished characters data combining . . .");
        }

        /// <summary>
        /// NOTE: Relies on the results from CombineMCPComicsData
        /// </summary>
        /// <param name="pathDataDir"></param>
        private static void AssembleMCPCharacterSeries(string pathDataDir)
        {
            Console.WriteLine("Starting character-series data assembling . . .");

            using (StreamWriter writer = new StreamWriter(FileManager.GetPathResultCharSeries(pathDataDir), false))
            using (StreamReader reader = new StreamReader(FileManager.GetPathResultCharComics(pathDataDir)))
            {
                while (!reader.EndOfStream)
                {
                    string input = reader.ReadLine();

                    if (!input.Equals(String.Empty))
                    {
                        string[] charComics = input.Split(',');

                        writer.Write(charComics[0]);
                        HashSet<string> comicSeries = new HashSet<string>();
                        for (int i = 1; i < charComics.Length; ++i)
                        {
                            comicSeries.Add(ComicsHelper.GetSeriesByComic(charComics[i]));
                        }

                        foreach (string series in comicSeries)
                        {
                            writer.Write(String.Format(",{0}", series));
                        }
                        writer.WriteLine();
                    }
                }
            }

            Console.WriteLine("Finished character-series data assembling.");
        }

        /// <summary>
        /// NOTE: Relies on the results from CombineMCPComicsData
        /// </summary>
        /// <param name="pathDataDir"></param>
        private static void AssembleMCPCharacterCharacters(string pathDataDir)
        {
            Console.WriteLine("Starting character-characters data assembling . . .");

            int countProcessedCharacter = 0;
            int totalNumCharacters = FileManager.GetNumLinesInCSV(FileManager.GetPathResultCharComics(pathDataDir));

            using (StreamWriter writer = new StreamWriter(FileManager.GetPathResultCharChars(pathDataDir), false))
            {
                string pathCharComics = FileManager.GetPathResultCharComics(pathDataDir);
                FileStream outerFileStream = new FileStream(pathCharComics, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);
                using (StreamReader outerReader = new StreamReader(outerFileStream))
                {
                    while (!outerReader.EndOfStream)
                    {
                        string outerInput = outerReader.ReadLine();

                        if (!outerReader.Equals(String.Empty))
                        {
                            Console.WriteLine(String.Format("Processing character #{0} of {1}", countProcessedCharacter, totalNumCharacters));
                            ++countProcessedCharacter;

                            string[] testCharData = outerInput.Split(',');
                            string testCharId = testCharData[0];
                            HashSet<string> testCharComics = new HashSet<string>();
                            for (int i = 1; i < testCharData.Length; ++i)
                            {
                                testCharComics.Add(testCharData[i]);
                            }

                            writer.Write(testCharId);

                            // Check overlapping with all characters
                            FileStream innerFileStream = new FileStream(pathCharComics, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);
                            using (StreamReader innerReader = new StreamReader(innerFileStream))
                            {
                                while (!innerReader.EndOfStream)
                                {
                                    string innerInput = innerReader.ReadLine();

                                    if (!innerInput.Equals(String.Empty))
                                    {
                                        string[] nextCharData = innerInput.Split(',');
                                        string nextCharId = nextCharData[0];

                                        if (!nextCharId.Equals(testCharId))
                                        {
                                            HashSet<string> nextCharComics = new HashSet<string>();
                                            for (int i = 1; i < nextCharData.Length; ++i)
                                            {
                                                nextCharComics.Add(nextCharData[i]);
                                            }

                                            if (testCharComics.Overlaps(nextCharComics))
                                            {
                                                writer.Write(String.Format(",{0}", nextCharId));
                                            }
                                        }
                                    }
                                }
                            }

                            writer.WriteLine();
                        }
                    }
                }
            }

            Console.WriteLine("Finished character-characters data assembling.");
        }
    }
}
