using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marvel_Research_Tool.Data_Model;
using Marvel_Research_Tool.Utils;

namespace Marvel_Research_Tool
{
    /* TODO
     * Compute for a pair of nodes in the graph:
     * 1. Sum of number of their neighbors.
     * 2. Difference in the number of neighbors.
     * 3. Product of number of neighbors.
     * 
     * 4. Number of common neighbors.
     * 5. Shortest path length (don't count the connection between nodes if one exists).
     * 
     * 6-8. Sum, difference and product of number of comics.
     * 9-11. Sum, difference and product of number of series.
     */

    public class FeaturesExtractor
    {
        public const int LEGTH_THRESHOLD = 3;
        public const int NO_PATH_VALUE = LEGTH_THRESHOLD;

        private const double KATZ_SCORE_B = 0.5;

        private string _pathDataDir;
        private CharacterCharactersData _characterCharactersData;
        private CharacterSeriesData _characterSeriesData;
        private CharacterComicsData _characterComicsData;
        private List<string> _characterIds;

        public FeaturesExtractor(string pathDataDir)
        {
            _pathDataDir = pathDataDir;
            _characterCharactersData = null;
            _characterComicsData = null;
            _characterSeriesData = null;
            _characterIds = null;
        }

        #region Load/Realease Data Methods

        public void LoadCharacterCharactersData()
        {
            _characterCharactersData = FileManager.ReadCharacterCharactersDataFromCSV(_pathDataDir, out _characterIds);
        }

        public void LoadCharacterComicsData()
        {
            _characterComicsData = FileManager.ReadCharacterComicsDataFromCSV(_pathDataDir);
        }

        public void LoadCharacterSeriesData()
        {
            _characterSeriesData = FileManager.ReadCharacterSeriesDataFromCSV(_pathDataDir);
        }

        public void ReleaseCharacterCharactersData()
        {
            _characterCharactersData = null;
            _characterIds = null;

            Console.WriteLine("Character-Characters data released.");
        }

        public void ReleaseCharacterComicsData()
        {
            _characterComicsData = null;

            Console.WriteLine("Character-Comics data released.");
        }

        public void ReleaseCharacterSeriesData()
        {
            _characterSeriesData = null;

            Console.WriteLine("Character-Series data released.");
        }

        #endregion Load/Realease Data Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetCharacterIdByIndex(int index)
        {
            if (index > _characterIds.Count) return null;

            return _characterIds[index];
        }

        #region Connections-Based Features

        public double GetJakardConnections(string characterId_1, string characterId_2)
        {
            HashSet<string> connections_1 = new HashSet<string>(GetConnections(characterId_1));
            HashSet<string> connections_2 = new HashSet<string>(GetConnections(characterId_2));

            if ((connections_1 == null) || (connections_2 == null)) return -1;

            HashSet<string> unionConnections = new HashSet<string>(connections_1);
            unionConnections.UnionWith(connections_1);
            connections_1.IntersectWith(connections_2);

            return ((double)connections_1.Count / unionConnections.Count);
        }

        /// <summary>
        /// NOTE: Load Character-Characters data first.
        /// </summary>
        /// <param name="characterId_1"></param>
        /// <param name="characterId_2"></param>
        /// <returns></returns>
        public int GetNumCommonConnections(string characterId_1, string characterId_2)
        {
            HashSet<string> connections_1 = new HashSet<string>(GetConnections(characterId_1));
            HashSet<string> connections_2 = new HashSet<string>(GetConnections(characterId_2));

            if ((connections_1 == null) || (connections_2 == null)) return -1;

            connections_1.IntersectWith(connections_2);

            return connections_1.Count;
        }

        /// <summary>
        /// NOTE: Load Character-Characters data first.
        /// </summary>
        /// <param name="characterId_1"></param>
        /// <param name="characterId_2"></param>
        /// <returns></returns>
        public int GetSumConnections(string characterId_1, string characterId_2)
        {
            int numConnections_1 = GetNumConnections(characterId_1);
            int numConnections_2 = GetNumConnections(characterId_2);

            if ((numConnections_1 == -1) || (numConnections_2 == -1)) return -1;

            return (numConnections_1 + numConnections_2);
        }

        /// <summary>
        /// NOTE: Load Character-Characters data first.
        /// </summary>
        /// <param name="characterId_1"></param>
        /// <param name="characterId_2"></param>
        /// <returns></returns>
        public int GetDifferenceConnections(string characterId_1, string characterId_2)
        {
            int numConnections_1 = GetNumConnections(characterId_1);
            int numConnections_2 = GetNumConnections(characterId_2);

            if ((numConnections_1 == -1) || (numConnections_2 == -1)) return -1;

            return Math.Abs(numConnections_1 - numConnections_2);
        }

        /// <summary>
        /// NOTE: Load Character-Characters data first.
        /// </summary>
        /// <param name="characterId_1"></param>
        /// <param name="characterId_2"></param>
        /// <returns></returns>
        public int GetProductConnections(string characterId_1, string characterId_2)
        {
            int numConnections_1 = GetNumConnections(characterId_1);
            int numConnections_2 = GetNumConnections(characterId_2);

            if ((numConnections_1 == -1) || (numConnections_2 == -1)) return -1;

            return (numConnections_1 * numConnections_2);
        } 

        /// <summary>
        /// NOTE: Load Character-Characters data first.
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        public int GetNumConnections(string characterId)
        {
            HashSet<string> connections;

            if (!_characterCharactersData.TryGetValue(characterId, out connections)) return -1;

            return connections.Count;
        }

        public HashSet<string> GetConnections(string characterId)
        {
            HashSet<string> connections;

            if (!_characterCharactersData.TryGetValue(characterId, out connections)) return null;

            return connections;
        }

        public int GetTotalNumConnections()
        {
            return _characterCharactersData.Count();
        }

        #endregion Connections-Based Features

        #region Comics-Based Features

        public double GetJakardComics(string characterId_1, string characterId_2)
        {
            HashSet<string> comics_1 = new HashSet<string>(GetComics(characterId_1));
            HashSet<string> comics_2 = new HashSet<string>(GetComics(characterId_2));

            if ((comics_1 == null) || (comics_2 == null)) return -1;

            HashSet<string> unionComics = new HashSet<string>(comics_1);
            unionComics.UnionWith(comics_1);
            comics_1.IntersectWith(comics_2);

            return ((double)comics_1.Count / unionComics.Count);
        }

        public HashSet<string> GetComics(string characterId)
        {
            HashSet<string> comics;

            if (!_characterComicsData.TryGetValue(characterId, out comics)) return null;

            return comics;
        }

        public int GetNumCommonComics(string characterId_1, string characterId_2)
        {
            HashSet<string> comics_1 = new HashSet<string>(GetComics(characterId_1));
            HashSet<string> comics_2 = new HashSet<string>(GetComics(characterId_2));

            if ((comics_1 == null) || (comics_2 == null)) return -1;

            comics_1.IntersectWith(comics_2);

            return comics_1.Count;
        }

        /// <summary>
        /// NOTE: Load Character-Comics data first.
        /// </summary>
        /// <param name="characterId_1"></param>
        /// <param name="characterId_2"></param>
        /// <returns></returns>
        public int GetSumComics(string characterId_1, string characterId_2)
        {
            int numComics_1 = GetNumComics(characterId_1);
            int numComics_2 = GetNumComics(characterId_2);

            if ((numComics_1 == -1) || (numComics_2 == -1)) return -1;

            return (numComics_1 + numComics_2);
        }

        /// <summary>
        /// NOTE: Load Character-Comics data first.
        /// </summary>
        /// <param name="characterId_1"></param>
        /// <param name="characterId_2"></param>
        /// <returns></returns>
        public int GetDifferenceComics(string characterId_1, string characterId_2)
        {
            int numComics_1 = GetNumComics(characterId_1);
            int numComics_2 = GetNumComics(characterId_2);

            if ((numComics_1 == -1) || (numComics_2 == -1)) return -1;

            return Math.Abs(numComics_1 - numComics_2);
        }

        /// <summary>
        /// NOTE: Load Character-Comics data first.
        /// </summary>
        /// <param name="characterId_1"></param>
        /// <param name="characterId_2"></param>
        /// <returns></returns>
        public int GetProductComics(string characterId_1, string characterId_2)
        {
            int numComics_1 = GetNumComics(characterId_1);
            int numComics_2 = GetNumComics(characterId_2);

            if ((numComics_1 == -1) || (numComics_2 == -1)) return -1;

            return (numComics_1 * numComics_2);
        }

        /// <summary>
        /// NOTE: Load Character-Comics data first.
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        public int GetNumComics(string characterId)
        {
            HashSet<string> comics;

            if (!_characterComicsData.TryGetValue(characterId, out comics)) return -1;

            return comics.Count;
        }

        #endregion Comics-Based Features

        #region Series-Based Features

        public double GetJakardSeries(string characterId_1, string characterId_2)
        {
            HashSet<string> series_1 = new HashSet<string>(GetSeries(characterId_1));
            HashSet<string> series_2 = new HashSet<string>(GetSeries(characterId_2));

            if ((series_1 == null) || (series_2 == null)) return -1;

            HashSet<string> unionSeries = new HashSet<string>(series_1);
            unionSeries.UnionWith(series_1);
            series_1.IntersectWith(series_2);

            return ((double) series_1.Count / unionSeries.Count);
        }

        public HashSet<string> GetSeries(string characterId)
        {
            HashSet<string> series;

            if (!_characterSeriesData.TryGetValue(characterId, out series)) return null;

            return series;
        }

        public int GetNumCommonSeries(string characterId_1, string characterId_2)
        {
            HashSet<string> series_1 = new HashSet<string>(GetSeries(characterId_1));
            HashSet<string> series_2 = new HashSet<string>(GetSeries(characterId_2));

            if ((series_1 == null) || (series_2 == null)) return -1;

            series_1.IntersectWith(series_2);

            return series_1.Count;
        }

        /// <summary>
        /// NOTE: Load Character-Series data first.
        /// </summary>
        /// <param name="characterId_1"></param>
        /// <param name="characterId_2"></param>
        /// <returns></returns>
        public int GetSumSeries(string characterId_1, string characterId_2)
        {
            int numSeries_1 = GetNumSeries(characterId_1);
            int numSeries_2 = GetNumSeries(characterId_2);

            if ((numSeries_1 == -1) || (numSeries_2 == -1)) return -1;

            return (numSeries_1 + numSeries_2);
        }

        /// <summary>
        /// NOTE: Load Character-Series data first.
        /// </summary>
        /// <param name="characterId_1"></param>
        /// <param name="characterId_2"></param>
        /// <returns></returns>
        public int GetDifferenceSeries(string characterId_1, string characterId_2)
        {
            int numSeries_1 = GetNumSeries(characterId_1);
            int numSeries_2 = GetNumSeries(characterId_2);

            if ((numSeries_1 == -1) || (numSeries_2 == -1)) return -1;

            return Math.Abs(numSeries_1 - numSeries_2);
        }

        /// <summary>
        /// NOTE: Load Character-Series data first.
        /// </summary>
        /// <param name="characterId_1"></param>
        /// <param name="characterId_2"></param>
        /// <returns></returns>
        public int GetProductSeries(string characterId_1, string characterId_2)
        {
            int numSeries_1 = GetNumSeries(characterId_1);
            int numSeries_2 = GetNumSeries(characterId_2);

            if ((numSeries_1 == -1) || (numSeries_2 == -1)) return -1;

            return (numSeries_1 * numSeries_2);
        }

        /// <summary>
        /// NOTE: Load Character-Series data first.
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        public int GetNumSeries(string characterId)
        {
            HashSet<string> series;

            if (!_characterSeriesData.TryGetValue(characterId, out series)) return -1;

            return series.Count;
        }

        #endregion Series-Based Features

        #region Path-Based Features

        /// <summary>
        /// Implements BFS to find all the path from the start node to the end node.
        /// NOTE: Needs Character-Characters data to be loaded.
        /// </summary>
        /// <param name="characterStartId"></param>
        /// <param name="characterEnd"></param>
        /// <returns></returns>
        private List<HashSet<string>> FindAllPaths(string characterStartId, string characterEndId)
        {
            if (_characterCharactersData == null) return null;

            List<HashSet<string>> allPaths = new List<HashSet<string>>();

            HashSet<CharactersTreeNode> nodesToExplore = new HashSet<CharactersTreeNode>();
            CharactersTreeNode nodeStart = new CharactersTreeNode(characterStartId, null);
            nodesToExplore.Add(nodeStart);
            CharactersTreeNode curNode = null;

            while (nodesToExplore.Count > 0)
            {
                foreach (CharactersTreeNode node in nodesToExplore)
                {
                    curNode = node;
                    break;
                }

                if (curNode.ID.Equals(characterEndId) && (curNode.PathLength() > 1))
                {
                    allPaths.Add(curNode.Path);
                }
                else if (curNode.PathLength() < LEGTH_THRESHOLD)
                {
                    curNode.LoadChildren(GetConnections(curNode.ID));
                    foreach (CharactersTreeNode child in curNode.Children)
                    {
                        nodesToExplore.Add(child);
                    }
                }

                nodesToExplore.Remove(curNode);
            }

            return allPaths;
        }

        // Shortest, 2nd shortest, average, Katz
        public double[] GetPathBasedFeatures(string characterStartId, string characterEndId)
        {
            List<HashSet<string>> allPaths = FindAllPaths(characterStartId, characterEndId);

            // Special cases
            if (allPaths.Count == 0) return new double[4] { NO_PATH_VALUE, NO_PATH_VALUE, NO_PATH_VALUE, NO_PATH_VALUE };
            if (allPaths.Count == 1) return new double[4] { allPaths[0].Count - 1, NO_PATH_VALUE, allPaths[0].Count - 1, allPaths[0].Count - 1 };

            double[] featuresValues = new double[4];
            double shortestPathLength = allPaths[0].Count;
            double secondShortestPathLength = shortestPathLength;
            Dictionary<int, int> dictPathsLength = new Dictionary<int, int>();

            foreach (HashSet<string> path in allPaths)
            {
                int pathLength = path.Count;

                if (pathLength < shortestPathLength)
                {
                    secondShortestPathLength = shortestPathLength;
                    shortestPathLength = pathLength;
                }
                else if (pathLength < secondShortestPathLength)
                {
                    secondShortestPathLength = pathLength;
                }

                // Update dictionary with paths length counters
                int pathLengthCount;
                if (!dictPathsLength.TryGetValue(pathLength, out pathLengthCount))
                {
                    dictPathsLength.Add(pathLength, 1);
                }
                else
                {
                    ++pathLengthCount;
                }
            }

            featuresValues[0] = shortestPathLength;
            featuresValues[1] = secondShortestPathLength;

            double katzScore = 0;
            int totalLength = 0;
            int countNumPaths = 0;

            foreach (int key in dictPathsLength.Keys)
            {
                int count;
                dictPathsLength.TryGetValue(key, out count);

                countNumPaths += count;
                totalLength += key * count;
                katzScore += Math.Pow(KATZ_SCORE_B, key) * count;
            }

            featuresValues[2] = totalLength / countNumPaths;
            featuresValues[3] = katzScore;

            return featuresValues;
        }

        #endregion Path-Based Features
    }
}
