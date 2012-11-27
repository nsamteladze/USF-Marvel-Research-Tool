using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marvel_Research_Tool.Utils
{
    public class IDHelper
    {
        public static string NormalizeID(string id)
        {
            return id.Replace(',', '_').Replace(' ', '_').Replace("__", "_");
        }

        public static string AddIDParentLetter(string id, string letter)
        {
            return String.Format("{0}_{1}", letter, id);
        }

        public static string GetIDFromLink(string link, string currentLetter)
        {
            string id = String.Empty;

            string[] linkParts = link.Split('#');
            if (linkParts.Length > 2)
            {
                Console.WriteLine(String.Format("WARNING! Link has more than 2 parts. Link: {0}. Letter: {1}.", link, currentLetter));
            }

            if (linkParts[0].Equals(String.Empty))
            {
                id = String.Format("{0}_{1}", currentLetter, linkParts[1]);
            }
            else
            {
                id = String.Format("{0}_{1}", linkParts[0][0], linkParts[1]);
            }

            return id;
        }
    }
}
