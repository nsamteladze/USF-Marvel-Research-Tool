using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marvel_Research_Tool.Data_Model
{
    public class Comic
    {
        public string Name { get; set; }
        public int NumSold { get; set; }

        public Comic()
        {
            this.Name = String.Empty;
            this.NumSold = 0;
        }

        public Comic(string name, int numSold)
        {
            this.Name = name;
            this.NumSold = numSold;
        }
    }
}
