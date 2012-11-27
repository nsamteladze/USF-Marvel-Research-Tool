using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Marvel_Research_Tool.Utils;
using Marvel_Research_Tool.Data_Model;

namespace Marvel_Research_Tool
{
    public class DataSampler
    {
        private const string FEATURES_NAMES = "ID,Connected,Sum_Connections,Difference_Connections,Product_Connections," +
                                               "Common_Connections,Jaccard_Connections,Sum_Comics,Difference_Comics,Product_Comics," +
                                               "Sum_Series,Difference_Series,Product_Series";

        /* LOGIC
         * 1. Randomly select one node in the graph.
         * 2. Randonly select the second node from the first node's connections.
         * 3. Compute all the features for this pair of connected nodes.
         * 4. Randomly select one node from the nodes that are not connected to
         *    the first node.
         * 5. Compute all the features for the selected pair of disconnected nodes.
         * 6. Repeat 5000 times. 
         * Note: when selecting a pair, make sure that it is not already in the training set.
         */
        public static void CreateTrainingAndTestSets(string pathDataDir, SetType trainingTestType, SetType testingSetType,
                                                     int trainingSetSize, int testingSetSize, int testingDataAmount)
        {
            Console.WriteLine("Starting creation of training and testing subsets . . .");
            FeaturesExtractor featuresExtractor = new FeaturesExtractor(pathDataDir);
            featuresExtractor.LoadCharacterCharactersData();
            featuresExtractor.LoadCharacterComicsData();
            featuresExtractor.LoadCharacterSeriesData();
            HashSet<string> usedExamples = new HashSet<string>();

            // Training data
            Console.WriteLine("Creating training subset . . .");
            Console.WriteLine(String.Format("Training set type: {0}", trainingTestType));
            CreateAndSaveRandomSubset(FileManager.GetPathResultTrainingSet(pathDataDir), featuresExtractor,
                                      usedExamples, trainingSetSize);
            // Testing data
            Console.WriteLine("Creating testing subset . . .");
            Console.WriteLine(String.Format("Testing set type: {0}", testingSetType));
            CreateAndSaveRandomSubset(FileManager.GetPathResultTestSet(pathDataDir), featuresExtractor,
                                      usedExamples, testingSetSize);

            Console.WriteLine("Finished creation of training and testing subsets.");
        }

        // id, value, connections: {sum, diff, product, common, jakard}, comics: {sum, diff, product }, series: {sum, diff, product }
        private static string GetFeaturesString(string characterId_1, string characterId_2, FeaturesExtractor featuresExtractor, int classValue)
        {
            string featuresString = String.Format("{0}-{1}", characterId_1, characterId_2);
            featuresString += String.Format(",{0}", classValue);

            // Connections-based
            featuresString += String.Format(",{0}", featuresExtractor.GetSumConnections(characterId_1, characterId_2));
            featuresString += String.Format(",{0}", featuresExtractor.GetDifferenceConnections(characterId_1, characterId_2));
            featuresString += String.Format(",{0}", featuresExtractor.GetProductConnections(characterId_1, characterId_2));
            featuresString += String.Format(",{0}", featuresExtractor.GetNumCommonConnections(characterId_1, characterId_2));
            featuresString += String.Format(",{0:0.#####}", featuresExtractor.GetJakardConnections(characterId_1, characterId_2));

            // Comics-based
            featuresString += String.Format(",{0}", featuresExtractor.GetSumComics(characterId_1, characterId_2));
            featuresString += String.Format(",{0}", featuresExtractor.GetDifferenceComics(characterId_1, characterId_2));
            featuresString += String.Format(",{0}", featuresExtractor.GetProductComics(characterId_1, characterId_2));

            // Series-based
            featuresString += String.Format(",{0}", featuresExtractor.GetSumSeries(characterId_1, characterId_2));
            featuresString += String.Format(",{0}", featuresExtractor.GetDifferenceSeries(characterId_1, characterId_2));
            featuresString += String.Format(",{0}", featuresExtractor.GetProductSeries(characterId_1, characterId_2));

            return featuresString;
        }

        private static void CreateAndSaveRandomSubset(string pathSavingFile, FeaturesExtractor featuresExtractor,
                                                      HashSet<string> usedExamples, int numExtracted)
        {
            Random randomizer = new Random();
            int numberOfCharacters = featuresExtractor.GetTotalNumConnections();

            using (StreamWriter writer = new StreamWriter(pathSavingFile, false))
            {
                writer.WriteLine(FEATURES_NAMES);
                int numIteration = 0;

                while (numIteration < numExtracted)
                {
                    Console.WriteLine(String.Format("Processing node {0} of {1}", numIteration + 1, numExtracted));

                    // Choose 1st node
                    SampleCharacter targetCharacter = new SampleCharacter();
                    targetCharacter.Index = randomizer.Next(numberOfCharacters - 1);
                    targetCharacter.ID = featuresExtractor.GetCharacterIdByIndex(targetCharacter.Index);
                    targetCharacter.ConnectionsAsHashSet = featuresExtractor.GetConnections(targetCharacter.ID);
                    while (targetCharacter.ConnectionsAsHashSet.Count < 1)
                    {
                        targetCharacter.Index = randomizer.Next(numberOfCharacters - 1);
                        targetCharacter.ID = featuresExtractor.GetCharacterIdByIndex(targetCharacter.Index);
                        targetCharacter.ConnectionsAsHashSet = featuresExtractor.GetConnections(targetCharacter.ID);
                    }
                    targetCharacter.ConnectionsAsList = targetCharacter.ConnectionsAsHashSet.ToList<string>();

                    // Choose a connected node to get a positive example
                    SampleCharacter connectedCharacter = new SampleCharacter();
                    connectedCharacter.Index = randomizer.Next(targetCharacter.ConnectionsAsList.Count - 1);
                    connectedCharacter.ID = targetCharacter.ConnectionsAsList[connectedCharacter.Index];
                    int i = 0;
                    while (((usedExamples.Contains(String.Format("{0}-{1}", targetCharacter.ID, connectedCharacter.ID))) ||
                            (usedExamples.Contains(String.Format("{1}-{0}", targetCharacter.ID, connectedCharacter.ID)))) &&
                           (i < 10))
                    {
                        connectedCharacter.Index = randomizer.Next(targetCharacter.ConnectionsAsList.Count - 1);
                        connectedCharacter.ID = targetCharacter.ConnectionsAsList[connectedCharacter.Index];
                        ++i;
                    }
                    // Give up because the chosen node has small number of connections
                    if (i == 10) continue;
                    usedExamples.Add(String.Format("{0}-{1}", targetCharacter.ID, connectedCharacter.ID));

                    // Choose a disconnected node to get a negative example
                    SampleCharacter disconnectedCharacter = new SampleCharacter();
                    // Not the same as the target node
                    while ((disconnectedCharacter.Index = randomizer.Next(numberOfCharacters - 1)) == targetCharacter.Index) { };
                    disconnectedCharacter.ID = featuresExtractor.GetCharacterIdByIndex(disconnectedCharacter.Index);
                    // Is disconnected
                    while (targetCharacter.ConnectionsAsHashSet.Contains(disconnectedCharacter.ID))
                    {
                        while ((disconnectedCharacter.Index = randomizer.Next(numberOfCharacters - 1)) == targetCharacter.Index) { };
                        disconnectedCharacter.ID = featuresExtractor.GetCharacterIdByIndex(disconnectedCharacter.Index);
                    }
                    // Did not select the same example before
                    while ((usedExamples.Contains(String.Format("{0}-{1}", targetCharacter.ID, disconnectedCharacter.ID))) ||
                           (usedExamples.Contains(String.Format("{1}-{0}", targetCharacter.ID, disconnectedCharacter.ID))))
                    {
                        while ((disconnectedCharacter.Index = randomizer.Next(numberOfCharacters - 1)) == targetCharacter.Index) { };
                        disconnectedCharacter.ID = featuresExtractor.GetCharacterIdByIndex(disconnectedCharacter.Index);
                        while (targetCharacter.ConnectionsAsHashSet.Contains(disconnectedCharacter.ID))
                        {
                            while ((disconnectedCharacter.Index = randomizer.Next(numberOfCharacters - 1)) == targetCharacter.Index) { };
                            disconnectedCharacter.ID = featuresExtractor.GetCharacterIdByIndex(disconnectedCharacter.Index);
                        }
                    }
                    usedExamples.Add(String.Format("{0}-{1}", targetCharacter.ID, disconnectedCharacter.ID));

                    // Write 
                    writer.WriteLine(GetFeaturesString(targetCharacter.ID, connectedCharacter.ID, featuresExtractor, 1));
                    writer.WriteLine(GetFeaturesString(targetCharacter.ID, disconnectedCharacter.ID, featuresExtractor, 0));

                    ++numIteration;
                }
            }
        }
    }
}
