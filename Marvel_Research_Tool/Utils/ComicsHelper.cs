using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marvel_Research_Tool.Utils
{
    public class ComicsHelper
    {
        public static string GetSeriesByComic(string comic)
        {
            string[] comicParts = comic.Split(' ');

            if (comicParts.Length < 2) return null;

            string series = comicParts[0];
            for (int i = 1; i < comicParts.Length - 1; ++i)
            {
                series += String.Format(" {0}", comicParts[i]);
            }

            return series.TrimEnd('@'); ;
        }
    }
}
