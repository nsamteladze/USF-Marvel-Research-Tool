using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marvel_Research_Tool.Utils;
using System.IO;

namespace Marvel_Research_Tool
{
    public class StatisticsCollector
    {
        public static void GetConnectivityStatistics(string pathDataDir)
        {
            int numNodes = FileManager.GetNumLinesInCSV(FileManager.GetPathResultCharChars(pathDataDir));
            long totalConnected = 0;

            using (StreamReader reader = new StreamReader(FileManager.GetPathResultCharChars(pathDataDir)))
            {
                while (!reader.EndOfStream)
                {
                    string input = reader.ReadLine();
                    if (input.Equals(String.Empty)) continue;

                    totalConnected += input.Split(',').Length - 1;
                }
            }

            totalConnected /= 2;
            long totalPossibleConnections = (numNodes * (numNodes - 1)) / 2;
            long totalDisconnected = totalPossibleConnections - totalConnected;

            Console.WriteLine("---=== RESULTS ===---");
            Console.WriteLine(String.Format("# of Nodes: {0}\nPossible connections: {1}", numNodes, totalPossibleConnections));
            Console.WriteLine(String.Format("Connected: {0} ({1:0.####}%)\nDisconnected: {2} ({3:0.####}%)",
                                            totalConnected, (double) totalConnected / totalPossibleConnections,
                                            totalDisconnected, (double) totalDisconnected / totalPossibleConnections));
            Console.WriteLine(String.Format("Proportion Connected/Disconnected: 1:{0:0.##}", (double) totalDisconnected/totalConnected));
        }
    }
}
