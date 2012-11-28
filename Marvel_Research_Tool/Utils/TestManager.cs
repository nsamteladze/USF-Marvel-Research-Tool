using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Marvel_Research_Tool.Utils
{
    public class TestManager
    {
        public static void GetStatisticsGraph(string pathDataDir)
        {
            Console.WriteLine("Starting statistics collection . . .");

            int totalCharacters = 0;

            foreach (string pathLetterDir in Directory.EnumerateDirectories(pathDataDir, "?", SearchOption.TopDirectoryOnly))
            {
                int numCharacters = FileManager.GetNumLinesInCSV(FileManager.GetComicsFilePath(pathLetterDir));
                Console.WriteLine(String.Format("Letter: {0} | # Characters: {1}",
                                  Path.GetFileName(pathLetterDir), numCharacters));
                totalCharacters += numCharacters;
            }

            Console.WriteLine(String.Format("---=== RESULTS ===---\nTotal # characters: {0}", totalCharacters));
            Console.WriteLine("Finished statistics collection.");
        }

        public static void TestLinkageGraph(string pathLinkageGraph)
        {
            Console.WriteLine("Start linkage graph testing . . .");

            HashSet<string> setIDs = new HashSet<string>();

            using (StreamReader reader = new StreamReader(pathLinkageGraph))
            {
                while (!reader.EndOfStream)
                {
                    string input = reader.ReadLine();
                    if (!input.Equals(String.Empty))
                    {
                        string[] links = input.Split(',');

                        foreach (string link in links)
                        {
                            if (setIDs.Contains(link))
                            {
                                Console.WriteLine(String.Format("WARNING! Repeated link. Link: {0}.", link));
                            }
                            else
                            {
                                setIDs.Add(link);
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Finished linkage graph checking.");
        }

        /// <summary>
        /// Tests IDs of all the existing characters and checks that every character has
        /// one and that each ID is unique.
        /// </summary>
        /// <param name="pathDataDir"></param>
        public static void TestID(string pathDataDir)
        {
            Console.WriteLine("Starting IDs check . . .");

            HashSet<string> setCharacterID = new HashSet<string>();

            // Load ID into a HashSet
            foreach (string pathLetterDir in Directory.EnumerateDirectories(pathDataDir, "?", SearchOption.TopDirectoryOnly))
            {
                string pathChNames = Path.Combine(pathLetterDir, FileManager.RESULT_CH_NAMES + Path.GetFileName(pathLetterDir) + FileManager.CSV);
                using (StreamReader reader = new StreamReader(pathChNames))
                {
                    while (!reader.EndOfStream)
                    {
                        string input = reader.ReadLine();

                        // Due to possible empty row in the end of a file
                        if (!input.Equals(String.Empty))
                        {
                            // File format: ID, Names
                            string characterId = input.Split(',')[0];

                            if (characterId.Equals(String.Empty))
                            {
                                Console.WriteLine(String.Format("Empty ID. Letter: {0}. Name: {1}", Path.GetFileName(pathLetterDir), input.Split(',')[1]));
                            }
                            else if (setCharacterID.Contains(characterId))
                            {
                                Console.WriteLine(String.Format("Repeated ID. Letter: {0}. ID: {1}.", Path.GetFileName(pathLetterDir), characterId));
                            }
                            else
                            {
                                setCharacterID.Add(characterId);
                            }
                        }
                    }
                }
            }

            Console.WriteLine("IDs testing finished.");
        }

        public static bool TestOneMapping(string comic)
        {
            int temp;
            string[] comicParts = comic.Split(' ');

            string fullComicName = String.Empty;
            if (comicParts.Length > 0)
            {
                fullComicName += comicParts[0];
                for (int j = 1; j < comicParts.Length - 1; ++j)
                {
                    fullComicName += String.Format(" {0}", comicParts[j]);
                }
            }

            if ((FileManager.GetKeyByComicName(fullComicName) == null) ||
                (!Int32.TryParse(comicParts[comicParts.Length - 1], out temp)))
            {
                if ((FileManager.GetKeyByComicName(fullComicName.TrimEnd('@')) != null) &&
                    (Int32.TryParse(comicParts[comicParts.Length - 1], out temp)))
                {
                    return true;
                }
                else if ((FileManager.GetKeyByComicName(fullComicName) != null) &&
                         (Int32.TryParse(comicParts[comicParts.Length - 1].TrimStart('\''), out temp)))
                {
                    return true;
                }
                else
                {
                    // This is when we are sure that 
                    return false;
                }
            }

            return true;
        }

        public static void TestMapping(string pathDataDir)
        {
            Console.WriteLine("Starting mapping testing . . .");

            int countGlobalAll = 0;
            int countGlobalBad = 0;

            using (StreamWriter writer = new StreamWriter(FileManager.SYSTEM_BAD_MAPPING, false))
            {
                foreach (string pathLetterDir in Directory.EnumerateDirectories(pathDataDir, "?", SearchOption.TopDirectoryOnly))
                {
                    Console.WriteLine(String.Format("Testing data in {0}.", pathLetterDir));

                    List<List<string>> allComics = FileManager.ReadFromCSV(FileManager.GetComicsFilePath(pathLetterDir));
                    List<List<string>> key = FileManager.ReadFromCSV(Path.Combine(pathDataDir, FileManager.DEFAULT_KEY_PATH));

                    int countAll = 0;
                    int countBad = 0;
                    int temp;

                    foreach (List<string> charComics in allComics)
                    {
                        for (int i = 1; i < charComics.Count; ++i)
                        {
                            ++countAll;
                            string[] comicParts = charComics[i].Split(' ');

                            string fullComicName = String.Empty;
                            if (comicParts.Length > 0)
                            {
                                fullComicName += comicParts[0];
                                for (int j = 1; j < comicParts.Length - 1; ++j)
                                {
                                    fullComicName += String.Format(" {0}", comicParts[j]);
                                }
                            }

                            if ((FileManager.GetKeyByComicName(fullComicName) == null) ||
                                (!Int32.TryParse(comicParts[comicParts.Length - 1], out temp)))
                            {
                                if ((FileManager.GetKeyByComicName(fullComicName.TrimEnd('@')) != null) &&
                                    (Int32.TryParse(comicParts[comicParts.Length - 1], out temp)))
                                {
                                    // Everything OK, do nothing
                                }
                                else if ((FileManager.GetKeyByComicName(fullComicName) != null) &&
                                         (Int32.TryParse(comicParts[comicParts.Length - 1].TrimStart('\''), out temp)))
                                {
                                    // Everything OK, do nothing
                                }
                                else
                                {
                                    // Now it is totally BAD
                                    ++countBad;
                                    writer.WriteLine(charComics[i]);
                                }
                            }
                        }
                    }

                    countGlobalAll += countAll;
                    countGlobalBad += countBad;

                    Console.WriteLine(String.Format("Mapping success rate: {0:0.####}%\n# Unmapped comics: {1}",
                                                (double)(countAll - countBad) * 100 / countAll, countBad));
                }
            }

            Console.WriteLine("Mapping testing finished.");
            Console.WriteLine(String.Format("---=== FINAL RESULTS ===--- \nMapping success rate: {0:0.####}%\n# Unmapped comics: {1}",
                                            (double)(countGlobalAll - countGlobalBad) * 100 / countGlobalAll, countGlobalBad));
        }
    }
}
