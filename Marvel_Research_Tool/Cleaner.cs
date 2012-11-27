using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Marvel_Research_Tool.Utils;
using Marvel_Research_Tool.Data_Model;

namespace Marvel_Research_Tool
{
    public class Cleaner
    {
        public static void CleanSalesData(string pathDataDir)
        {
            Console.WriteLine("Starting cleaning sales data . . .");

            string pathSalesData = Path.Combine(pathDataDir, FileManager.DEFAULT_SALES_DIR);

            foreach (string pathSalesFile in Directory.EnumerateFiles(pathSalesData, "*", SearchOption.TopDirectoryOnly))
            {
                Console.WriteLine(String.Format("Cleaning data in {0}.", pathSalesFile));

                List<List<string>> salesData = new List<List<string>>();

                // Uncomment this line to do the intial clean of raw data.
                salesData = CleanSalesFilterMarvel(CleanSalesNumSold(FileManager.ReadFromCSV(pathSalesFile)));

                //salesData = FileManager.ReadFromCSV(pathSalesFile);

                foreach (List<string> comicSales in salesData)
                {
                    for (int i = 0; i < comicSales.Count; ++i)
                    {
                        comicSales[i] = CleanSalesComplete(comicSales[i]);
                    }
                }

                FileManager.WriteToCSV(salesData, pathSalesFile);
            }

            Console.WriteLine("Finished sales data cleaning.");
        }

        private static string CleanSalesComplete(string data)
        {
            string cleanedData = CleanSalesTrimAndLow(data);

            return cleanedData;
        }

        /// <summary>
        /// NOTE: Use only on data provided by CleanSalesTrimAndLow because cleaning depends on the data position in file.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static List<List<string>> CleanSalesFilterMarvel(List<List<string>> data)
        {
            List<List<string>> filteredData = data.Where(x => x[2].ToLower().Equals("marvel"))
                                                  .ToList<List<string>>();

            foreach (List<string> comicSales in filteredData)
            {
                comicSales.RemoveAt(2);
            }

            return filteredData;
        }

        private static string CleanSalesTrimAndLow(string data)
        {
            return data.Trim('"', ' ').ToLower().Replace("  ", " ");
        }

        /// <summary>
        /// Transforms sales data from "235,540" format (that came from Excel) into just int number 235540.
        /// NOTE: Use it before any other cleaning because it depends on the indexes in the raw data.
        /// </summary>
        /// <param name="comicSales"></param>
        /// <returns></returns>
        private static List<List<string>> CleanSalesNumSold(List<List<string>> data)
        {
            List<List<string>> cleanedData = new List<List<string>>(data);

            foreach (List<string> comicSales in cleanedData)
            {
                for (int i = 4; i < comicSales.Count; ++i)
                {
                    comicSales[3] += comicSales[i];
                }

                if (comicSales.Count > 4)
                {
                    comicSales.RemoveRange(4, comicSales.Count - 4);
                }
                comicSales[3] = comicSales[3].Trim(' ', '"');
            }

            return cleanedData;
        }

        public static void CleanKey(string pathDataDir)
        {
            Console.WriteLine("Staring to clean the key . . .");

            string pathKey = Path.Combine(pathDataDir, FileManager.DEFAULT_KEY_PATH);
            List<List<string>> key = FileManager.ReadFromCSV(pathKey);

            for (int i = 0; i < key.Count; ++i)
            {
                for (int j = 0; j < key[i].Count; ++j)
                {
                    key[i][j] = key[i][j].ToLower().Trim();
                }
            }

            Console.WriteLine("Saving the cleaned data . . .");

            FileManager.WriteToCSV(key, pathKey);

            Console.WriteLine("Finished cleaning data in the key.");
        }

        public static void CleanGraphData(string pathDataDir)
        {
            Console.WriteLine("Starting to clean data . . .");

            foreach (string pathLetterDir in Directory.EnumerateDirectories(pathDataDir, "?", SearchOption.TopDirectoryOnly))
            {
                Console.WriteLine(String.Format("Cleaning data in {0}.", pathLetterDir));

                List<CharacterComics> allComics = new List<CharacterComics>();

                using (StreamReader reader = new StreamReader(FileManager.GetComicsFilePath(pathLetterDir)))
                {
                    while (!reader.EndOfStream)
                    {
                        string input = reader.ReadLine();
                        if (!input.Equals(String.Empty))
                        {
                            string[] listComics = input.Split(',');
                            CharacterComics curCharComics = new CharacterComics(listComics[0]);
                            for (int i = 1; i < listComics.Length; ++i)
                            {
                                curCharComics.AddComic(CleanComicsComplete(listComics[i]));
                            }
                            allComics.Add(curCharComics);
                        }
                    }
                }

                Console.WriteLine("Saving data . . .");
                FileManager.WriteToCSV(allComics, FileManager.GetComicsFilePath(pathLetterDir));
            }

            Console.WriteLine("Finished data cleaning.");
        }

        private static List<string> CleanComicsComplete(string comic)
        {
            List<string> cleanedComics = CleanComicsStep2Split(CleanComicsStep1SimpleReplacements(comic))
                                         .ToList<string>();

            for (int i = 0; i < cleanedComics.Count; ++i)
            {
                cleanedComics[i] = CleanComicsStep3Brackets(cleanedComics[i]);
                cleanedComics[i] = CleanComicsStep4Slash(cleanedComics[i]);
                cleanedComics[i] = CleanComicsStep5Letters(cleanedComics[i]);
            }

            CleanComicsStep6Delete(ref cleanedComics);

            return cleanedComics;
        }

        private static string CleanComicsStep1SimpleReplacements(string comic)
        {
            string cleanedComic = comic.Replace("-fb", "");
            cleanedComic = cleanedComic.Replace("-bts", "");
            cleanedComic = cleanedComic.Replace("-vo", "");
            cleanedComic = cleanedComic.Replace("-op", "");
            cleanedComic = cleanedComic.Replace("&amp;", "&");
            cleanedComic = cleanedComic.Replace("&heart;", "love");

            return cleanedComic;
        }

        private static string[] CleanComicsStep2Split(string comic)
        { 
            string[] splitedData = comic.Replace("| cf.", "")
                                        .Replace("| cf", "")
                                        .Split('~', '=', '%');

            for (int i = 0; i < splitedData.Length; ++i)
            {
                splitedData[i] = splitedData[i].Trim();
            }

            return splitedData;
        }

        private static string CleanComicsStep3Brackets(string comic)
        {
            int openBracket = comic.IndexOf("(");
            int closeBracket = comic.IndexOf(")");

            while ((openBracket > -1) && (closeBracket > -1) && (closeBracket > openBracket))
            {
                comic = comic.Remove(openBracket, closeBracket - openBracket + 1);
                openBracket = comic.IndexOf("(");
                closeBracket = comic.IndexOf(")");
            }

            return comic.Trim().TrimStart('{', '[').TrimEnd('}', ']');
        }

        // Also replaces "//" with "/"
        private static string CleanComicsStep4Slash(string comic)
        {
            string[] stringParts = comic.Split('/');

            for (int i = 1; i < stringParts.Length; ++i)
            {
                if (stringParts[i].Length > 0)
                {
                    int indexStr = 0;
                    char nextToCheck = stringParts[i][indexStr];
                    while ((indexStr < stringParts[i].Length) && (nextToCheck >= '0') && (nextToCheck <= '9'))
                    {
                        ++indexStr;
                        if (indexStr < stringParts[i].Length)
                        {
                            nextToCheck = stringParts[i][indexStr];
                        }
                    }

                    stringParts[i] = stringParts[i].Remove(0, indexStr);
                    if (indexStr == 0)
                    {
                        stringParts[i - 1] = stringParts[i - 1] + "/";
                    }
                }
            }

            string newComic = String.Empty;
            for (int i = 0; i < stringParts.Length; ++i)
            {
                newComic += stringParts[i];
            }

            return newComic;
        }

        private static string CleanComicsStep5Letters(string comic)
        {
            string[] comicParts = comic.Trim().Split(' ');

            if (comicParts.Length == 2)
            {
                comicParts[1] = comicParts[1].TrimEnd('a', 'b');
            }

            if (comicParts.Length < 1) return String.Empty;

            string cleanedData = comicParts[0];
            for (int i = 1; i < comicParts.Length; ++i)
            {
                cleanedData += String.Format(" {0}", comicParts[i]);
            }

            return cleanedData;
        }

        private static void CleanComicsStep6Delete(ref List<string> comics)
        {
            comics = comics.Select(x => x.Trim(' ', '\n'))
                           .Where(x => !x.StartsWith("see ") && !x.StartsWith("from "))
                           .ToList<string>();
        }
    }
}
