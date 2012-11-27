using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Marvel_Research_Tool.Utils;

namespace Marvel_Research_Tool
{
    public class Linker
    {
        public static void LinkGraphData(string pathDataDir)
        {
            /* HOW TO
             * 1. Construct a linkage list
             * 2. Keep a HashSet<string> of IDs that we need to delete
             * 3. Keep a HashTable<string, HashSet<string>> of IDs which comics we need to update
             * 4. Foreach link download comics for all IDs in the link. Update the 1st ID, delete other IDs.
             * 5. Rewrite names and comics files skipping IDs that we need to delete and updating IDs that we need to update.
             */
            List<HashSet<string>> linkageList = ConstructLinkageList(pathDataDir, true);

            HashSet<string> setIDsToDelete = new HashSet<string>();
            Dictionary<string, HashSet<string>> dictIDsToUpdate = new Dictionary<string, HashSet<string>>();

            foreach (HashSet<string> setLinkedIDs in linkageList)
            {
                bool flagFirst = true;
                string keyID = String.Empty;
                foreach (string id in setLinkedIDs)
                {
                    HashSet<string> curLinkComics = FileManager.LoadComicsById(id);

                    if (curLinkComics != null)
                    {
                        if (flagFirst)
                        {
                            keyID = id;
                            flagFirst = false;
                            dictIDsToUpdate.Add(id, curLinkComics);
                        }
                        else
                        {
                            HashSet<string> setTemp;
                            if (dictIDsToUpdate.TryGetValue(keyID, out setTemp))
                            {
                                setTemp.UnionWith(curLinkComics);
                                setIDsToDelete.Add(id);
                            }
                            else
                            {
                                Console.WriteLine("WARNING! Could not find a key in the dictionary with IDs to update.");
                            }
                        }
                    }                   
                }
            }

            /* TODO
             * Rewrite all the file using information in setIDsToDelete and dictIDsToUpdate
             */
        }

        /// <summary>
        /// Constructs a linkage graph that further will be used to link characters' data.
        /// </summary>
        /// <param name="pathDataDir">
        /// Path to the directory with links data.
        /// </param>
        /// <param name="createFileWithResults">
        /// TRUE if you want to write the result graph into a file.
        /// FALSE otherwise.
        /// </param>
        /// <returns>
        /// List of links between characters. Use this information to link characters' data.
        /// </returns>
        private static List<HashSet<string>> ConstructLinkageList(string pathDataDir, bool createFileWithResults)
        {
            Console.WriteLine("Start constructing the linkage graph . . .");

            List<HashSet<string>> listLinkedIDs = new List<HashSet<string>>();

            foreach (string pathLetterDir in Directory.EnumerateDirectories(pathDataDir, "?", SearchOption.TopDirectoryOnly))
            {
                using (StreamReader reader = new StreamReader(FileManager.GetLinksFilePath(pathLetterDir)))
                {
                    while (!reader.EndOfStream)
                    {
                        string input = reader.ReadLine();
                        if (!input.Equals(String.Empty))
                        {
                            string[] listTestLinks = input.Split(',');
                            listLinkedIDs.Add(new HashSet<string>(listTestLinks));
                        }
                    }
                }
            }

            LinkList(ref listLinkedIDs);

            if (createFileWithResults)
            {
                Console.WriteLine("Saving the linkage graph . . .");
                FileManager.WriteToCSV(listLinkedIDs, FileManager.SYSTEM_LINKAGE_GRAPH);
            }

            Console.WriteLine("Finished the linkage graph construction.");

            return listLinkedIDs;
        }

        /// <summary>
        /// Given a list with links it combines all the links that intersects.
        /// </summary>
        /// <param name="listToLink">
        /// Input list that needs to be linked.
        /// </param>
        private static void LinkList(ref List<HashSet<string>> listToLink)
        {
            int startListSize;

            do
            {
                startListSize = listToLink.Count;
                bool exitFlag = false;
                int indexSource = 0;
                int indexTest = 0;

                while ((!exitFlag) && (indexSource < listToLink.Count))
                {
                    foreach (string testLink in listToLink[indexSource])
                    {
                        indexTest = indexSource + 1;
                        while ((!exitFlag) && (indexTest < listToLink.Count))
                        {
                            if (listToLink[indexTest].Contains(testLink))
                            {
                                exitFlag = true;
                            }

                            ++indexTest;
                        }

                        // Exit foreach loop if exitFlag == true
                        if (exitFlag) break;
                    }

                    ++indexSource;
                }

                if (exitFlag)
                {
                    listToLink[indexSource - 1].UnionWith(listToLink[indexTest - 1]);
                    listToLink.RemoveAt(indexTest - 1);
                }

            } while (listToLink.Count < startListSize);
        }

        // 1. Load each file with links in memory
        // 2. Filter the links
        // 3. Write the filtered links to the same file (overwrite)
        // This methid works only for raw web links. If it is used twice in a row, then results will be empty.
        public static void FilterLinks(string pathDataDir)
        {
            Console.WriteLine("Starting links filtering . . .");

            IEnumerable<string> letterDirectories = Directory.EnumerateDirectories(pathDataDir, "?", SearchOption.TopDirectoryOnly);
            foreach (string pathLetterDir in letterDirectories)
            {
                /* 1. Get ch_links_?.csv from the current directory
                 * 2. Load all information from there into a Dictionary<Guid, List<string>>
                 * 3. Filter all the links for each entry in the dictionary, delete entry if list is empty
                 */

                Console.WriteLine("Filtering links in " + pathLetterDir);

                List<List<string>> characterLinks = new List<List<string>>();

                string pathChLinks = FileManager.GetLinksFilePath(pathLetterDir);
                using (StreamReader reader = new StreamReader(pathChLinks))
                {
                    while (!reader.EndOfStream)
                    {
                        string input = reader.ReadLine();

                        if (!input.Equals(String.Empty))
                        {
                            string[] linksInfo = input.Split(',');
                            List<string> newLinksInfo = new List<string>();
                            // Add ID to the new list of links
                            newLinksInfo.Add(linksInfo[0]);
                            // Add all links
                            for (int i = 1; i < linksInfo.Length; ++i)
                            {
                                string linkToTest = linksInfo[i];
                                if (IsValidLink(linkToTest))
                                {
                                    newLinksInfo.Add(IDHelper.GetIDFromLink(linkToTest, Path.GetFileName(pathLetterDir)));
                                }
                            }
                            characterLinks.Add(newLinksInfo);
                        }
                    }
                }


                // 4. Write all the links to ch_links_?.csv
                using (StreamWriter writer = new StreamWriter(pathChLinks))
                {
                    foreach (List<string> listLinks in characterLinks)
                    {
                        if (listLinks.Count > 1)
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
            }

            Console.WriteLine("Finished links filtering.");
        }

        /// <summary>
        /// Determines wether an input RAW link is a valid link to another character.
        /// </summary>
        /// <param name="link">
        /// Link to analyze.
        /// </param>
        /// <returns>
        /// TRUE if the input link points to another character.
        /// FALSE otherwise.
        /// </returns>
        private static bool IsValidLink(string link)
        {
            if (link.StartsWith("#") || link.Remove(0, 1).StartsWith(".php#")) return true;
            return false;
        }

        public static void LinkGraphWithSales(string pathDataDir)
        {
            Console.WriteLine("ERROR! Method is not implemeted yet.");
        }
    }
}
