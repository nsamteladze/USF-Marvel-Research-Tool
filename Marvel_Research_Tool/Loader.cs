using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.IO;
using Marvel_Research_Tool.Utils;

namespace Marvel_Research_Tool
{
    /// <summary>
    /// Handles all operations that require extracting data
    /// from the Internet.
    /// </summary>
    public class Loader
    {
        /// <summary>
        /// Loads data from the Marvel Chronology Project (MCP) site
        /// and saves it into the specified directory. Note that KEY is
        /// not extracted, you have to get it manually.
        /// </summary>
        /// <param name="pathResultsDir">
        /// Path to the directory where the results will be saved.
        /// </param>
        public static void LoadGraphFromInternet(string pathResultsDir)
        {
            Console.WriteLine("Starting to extract data from Marvel Chronology Project . . .");

            HashSet<string> setCharactersID = new HashSet<string>();

            List<string> inputPages = ReadInputPages();

            int i = 1;
            foreach (string page in inputPages)
            {
                // Load html page
                Console.WriteLine("Loading page " + i + " of " + inputPages.Count + " . . .");
                HtmlWeb htmlWeb = new HtmlWeb();
                HtmlDocument htmlPage = htmlWeb.Load(page);

                // Start data extraction
                Console.WriteLine("Extracting data from " + page + " . . .");
                var tagTitle = htmlPage.DocumentNode.SelectNodes("html/head/title");
                foreach (HtmlNode node in tagTitle)
                {
                    Console.WriteLine("Page title: " + node.InnerText);
                }

                List<HtmlNode> chrons = htmlPage.DocumentNode.Descendants("div").Where(x => x.Id == "chrons").ToList();

                if (chrons.Count > 1)
                {
                    Console.WriteLine("ERROR! More than 1 element with ID = \"chrons\" has been found.");
                    return;
                }

                var charactersInfo = from HtmlNode pTag in chrons[0].Descendants("p")
                                     where (pTag.ChildNodes.Where(x => x.Name == "span").Count() == 2)
                                     select new
                                     {
                                         Name = (((from HtmlNode spanChar in pTag.Descendants("span")
                                                   where (spanChar.Attributes["class"].Value == "char")
                                                   select spanChar.Descendants("b"))
                                                 .ElementAt(0).ElementAt(0) as HtmlNode)
                                                 .InnerText as string)
                                                 .Trim().ToLower()
                                                 .Replace(',', ' ').Replace('\n', ' ').Replace("  ", " "),

                                         Id = (pTag.Id as string)
                                             .Trim().ToLower(),

                                         Links = (from HtmlNode tagA in pTag.Descendants("a")
                                                  where (tagA.Attributes["href"] != null)
                                                  select tagA.Attributes["href"].Value)
                                                  .Select(x => x.Trim().ToLower())
                                                  .ToList<string>(),

                                         Comics = ((from HtmlNode spanChron in pTag.Descendants("span")
                                                    where (spanChron.Attributes["class"].Value == "chron")
                                                    select spanChron.InnerText)
                                                     .ElementAt(0) as string)
                                                     .Trim().ToLower()
                                                     .Replace(',', ' ').Replace("  ", " ")
                                                     .Split('\n')
                                                     .ToList<string>()
                                     };

                Console.WriteLine("Saving data . . .");

                // Example: MAIN_DIR\a. Needed letter is the 5th character from the end because 
                // every URL ends with ?.php. UGLY
                string currentLetter = page[page.Length - 5].ToString();
                string pathTargetDir = Path.Combine(pathResultsDir, currentLetter);
                if (!Directory.Exists(pathTargetDir))
                {
                    Directory.CreateDirectory(pathTargetDir);
                }

                using (StreamWriter writerChNames = new StreamWriter(FileManager.GetNamesFilePath(pathTargetDir), false))
                using (StreamWriter writerChComics = new StreamWriter(FileManager.GetComicsFilePath(pathTargetDir), false))
                using (StreamWriter writerChLinks = new StreamWriter(FileManager.GetLinksFilePath(pathTargetDir), false))
                {
                    foreach (var character in charactersInfo)
                    {
                        string uniqueCharacterId = character.Id;

                        if (uniqueCharacterId.Equals(String.Empty))
                        {
                            uniqueCharacterId = Guid.NewGuid().ToString();
                            Console.WriteLine(String.Format("WARNING! Empty ID was detected. Unique GUID was generated instead. GUID: {0}. Character's Name: {1}.",
                                                uniqueCharacterId, character.Name));
                        }
                        else
                        {
                            uniqueCharacterId = IDHelper.AddIDParentLetter(IDHelper.NormalizeID(character.Id), currentLetter);

                            int index = 1;
                            while (setCharactersID.Contains(uniqueCharacterId))
                            {
                                uniqueCharacterId = IDHelper.AddIDParentLetter(IDHelper.NormalizeID(character.Id), currentLetter) + String.Format("({0})", index);
                                ++index;
                            }

                            setCharactersID.Add(uniqueCharacterId);
                        }

                        // Write names
                        writerChNames.WriteLine(String.Format("{0},{1}", uniqueCharacterId, character.Name));

                        // Write comics
                        writerChComics.Write(uniqueCharacterId);
                        foreach (string comic in character.Comics)
                        {
                            writerChComics.Write(String.Format(",{0}", comic));
                        }
                        writerChComics.WriteLine();

                        //// Write links
                        if ((character.Links != null) && (character.Links.Count() > 0))
                        {
                            writerChLinks.Write(uniqueCharacterId);
                            foreach (string link in character.Links)
                            {
                                writerChLinks.Write(String.Format(",{0}", link));
                            }
                            writerChLinks.WriteLine();
                        }
                    }
                }

                Console.WriteLine("Finished processing " + page);

                ++i;
            }

            Console.WriteLine("Data extraction finished.");
        }

        public static void LoadSalesFromInternet(string pathResultsDir)
        {
            Console.WriteLine("ERROR! Method is not implemeted yet.");
        }

        /// <summary>
        /// Reads input pages (pages we need to extract data from)
        /// from the default file.
        /// </summary>
        /// <returns>
        /// List of pages to extract data from.
        /// </returns>
        private static List<string> ReadInputPages()
        {
            List<string> inputPages = new List<string>();

            using (StreamReader reader = new StreamReader(FileManager.SYSTEM_INPUT_PAGES))
            {
                while (!reader.EndOfStream)
                {
                    inputPages.Add(reader.ReadLine());
                }
            }

            return inputPages;
        }
    }
}
