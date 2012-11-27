using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Command_Line_Parser;
using System.IO;
using Marvel_Research_Tool.Data_Model;
using Marvel_Research_Tool.Utils;

namespace Marvel_Research_Tool.Command_Line
{
    public class ApplicationCommandLine : CommandLine
    {
        private const int MAX_SIZE = 50000;
        private const int MIN_SIZE = 1;

        public ApplicationCommandLine(string pathCommandLineConfigFile)
            :base(pathCommandLineConfigFile) { }

        public override void RunCommand(CalledCommand calledCommand)
        {
            string pathDataDir;
            if (!GetPathOrDefault(calledCommand, out pathDataDir)) FailCommand("Invalid parameter value: path.");

            switch (calledCommand.Name)
            {
                case "load":
                    {
                        switch (calledCommand.MandatoryParametersValues[0])
                        {
                            case "graph":
                                Loader.LoadGraphFromInternet(pathDataDir);
                                break;
                            case "sales":
                                Loader.LoadSalesFromInternet(pathDataDir);
                                break;
                        }
                        break;
                    }
                case "clean":
                    {
                        switch (calledCommand.MandatoryParametersValues[0])
                        {
                            case "graph":
                                Cleaner.CleanGraphData(pathDataDir);
                                Cleaner.CleanKey(pathDataDir);
                                break;
                            case "sales":
                                Cleaner.CleanSalesData(pathDataDir);
                                break;
                        }
                        break;
                    }
                case "filter":
                    {
                        switch (calledCommand.MandatoryParametersValues[0])
                        {
                            case "graph_links":
                                Linker.FilterLinks(pathDataDir);
                                break;
                        }
                        break;
                    }
                case "link":
                    {
                        switch (calledCommand.MandatoryParametersValues[0])
                        {
                            case "graph":
                                Linker.LinkGraphData(pathDataDir);
                                break;
                            case "sales":
                                Linker.LinkGraphWithSales(pathDataDir);
                                break;
                        }
                        break;
                    }
                case "asm":
                    {
                        switch (calledCommand.MandatoryParametersValues[0])
                        {
                            case "graph":
                                Assembler.AssembleGraphData(pathDataDir);
                                break;
                            case "sales":
                                Assembler.AssembleSalesData(pathDataDir);
                                break;
                        }
                        break;
                    }
                case "sample":
                    {
                        SetType trainingSetType = SamplingHelper.GetSetTypeOrDefault(calledCommand.MandatoryParametersValues[0]);
                        int trainingSetSize;
                        if (!IsValidSetSize(calledCommand.MandatoryParametersValues[1], out trainingSetSize))
                            FailCommand("Invalid parameter value: training set size.");
                        SetType testingSetType = SamplingHelper.GetSetTypeOrDefault(calledCommand.MandatoryParametersValues[2]);
                        int testingSetSize;
                        if (!IsValidSetSize(calledCommand.MandatoryParametersValues[3], out testingSetSize))
                            FailCommand("Invalid parameter value: testing set size.");
                        int testingDataAmount = 100;
                        if (calledCommand.OptionalParametersValues.Count > 0)
                            if (!IsValidPercentage(calledCommand.OptionalParametersValues[0], out testingDataAmount))
                                FailCommand("Invalid parameter value: testing set size.");

                        DataSampler.CreateTrainingAndTestSets(pathDataDir, trainingSetType, testingSetType,
                                                              trainingSetSize, testingSetSize, testingDataAmount);
                        break;
                    }
                case "test":
                    {
                        switch (calledCommand.MandatoryParametersValues[0])
                        {
                            case "graph_linkage":
                                TestManager.TestLinkageGraph(pathDataDir);
                                break;
                            case "graph_mapping":
                                TestManager.TestMapping(pathDataDir);
                                break;
                            case "graph_id":
                                TestManager.TestID(pathDataDir);
                                break;
                        }

                        break;
                    }
                case "stat":
                    break;
                default:
                    FailCommand("Cannot recognize the input commands.");
                    break;
            }
        }

        private bool IsValidPath(string commandParameter)
        {
            foreach (char invalidPathChar in Path.GetInvalidPathChars())
            {
                if (commandParameter.Contains(invalidPathChar)) return false;
            }

            return true;
        }

        private bool IsValidSetSize(string commandParameter, out int sampleSize)
        {
            return ((Int32.TryParse(commandParameter, out sampleSize)) &&
                    (sampleSize >= MIN_SIZE) && (sampleSize <= MAX_SIZE));
        }

        private bool IsValidPercentage(string commandParameter, out int percentage)
        {
            return ((Int32.TryParse(commandParameter, out percentage)) && 
                    (percentage >= 0) && (percentage <= 100));
        }

        private bool GetPathOrDefault(CalledCommand mainCommand, out string path)
        {
            CalledCommand extraCommand;
            if (mainCommand.TryGetExtraCommand("path", out extraCommand))
            {
                path = extraCommand.MandatoryParametersValues[0];
                return (IsValidPath(path));
            }
            else
            {
                path = Program.CURRENT_DATA_DIR;
                return true;
            }
        }
    }
}
