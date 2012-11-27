using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Collections;
using Marvel_Research_Tool.Utils;

namespace Marvel_Research_Tool
{
    public class Program
    {
        public static readonly string CURRENT_DATA_DIR = @"D:\Sandbox\mcp";

        // Run options
        private const string OPTION_LOAD = "-load_mcp";
        private const string OPTION_FILL = "-fill_input";
        private const string OPTION_FILTER_LINKS = "-filter_links";
        private const string OPTION_LINK_MCP = "-link_mcp"; // Nor ready yet
        private const string OPTION_TEST_ID = "-test_id";
        private const string OPTION_TEST_LINKAGE_GRAPH = "-test_linkage";
        private const string OPTION_CLEAN_MCP = "-clean_mcp";
        private const string OPTION_TEST_MAPPING = "-test_mapping";
        private const string OPTION_CLEAN_KEY = "-clean_key";
        private const string OPTION_CLEAN_SALES = "-clean_sales";
        private const string OPTION_ASSEMBLE_SALES = "-asm_sales";
        private const string OPTION_STAT_MCP = "-stat_mcp";
        private const string OPTION_ASSEMBLE_MCP = "-asm_mcp";
        private const string OPTION_CREATE_TRAINING = "-create_training";
        private const string OPTION_CREATE_TEST = "-create_test";
        private const string OPTION_DATA_PATH = "-path";
        private const string SUBOPTION_SET_TYPE_BALANCED = "balanced";

        public static void Main(string[] args)
        {
            List<string> commands = args.Where(x => x.StartsWith("-")).ToList();
            List<string> arguments = args.Where(x => !x.StartsWith("-")).ToList();


            if (commands.Contains(OPTION_FILL))
            {
                if ((arguments.Count != 0) || (commands.Count != 1)) QuickFail();

                FileManager.FillInputPagesFile();
            }
            else if (commands.Contains(OPTION_TEST_ID))
            {
                if ((arguments.Count > 1) || (commands.Count != 1)) QuickFail();

                if (arguments.Count == 1) TestManager.TestID(arguments[0]);
                else TestManager.TestID(FileManager.DEFAULT_DATA_DIR);
            }
            else if (commands.Contains(OPTION_TEST_LINKAGE_GRAPH))
            {
                if ((arguments.Count > 1) || (commands.Count != 1)) QuickFail();

                if (arguments.Count == 1) TestManager.TestLinkageGraph(arguments[0]);
                else TestManager.TestLinkageGraph(FileManager.SYSTEM_LINKAGE_GRAPH);
            }
            else if (commands.Contains(OPTION_TEST_MAPPING))
            {
                if ((arguments.Count > 1) || (commands.Count != 1)) QuickFail();

                if (arguments.Count == 1) TestManager.TestMapping(arguments[0]);
                else TestManager.TestMapping(FileManager.DEFAULT_DATA_DIR);
            }
            else if (commands.Contains(OPTION_STAT_MCP))
            {
                if ((arguments.Count > 1) || (commands.Count != 1)) QuickFail();

                if (arguments.Count == 1) TestManager.GetStatisticsMCP(arguments[0]);
                else TestManager.GetStatisticsMCP(FileManager.DEFAULT_DATA_DIR);
            }
            else QuickFail();
        }

        private static void QuickFail()
        {
            Console.WriteLine("ERROR! Illegal arguments.");
            Environment.Exit(-1);
        }
    }
}
