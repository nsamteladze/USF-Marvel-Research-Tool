using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marvel_Research_Tool.Data_Model
{
    public class SampleCharacter
    {
        public string ID { get; set; }
        public int Index { get; set; }
        public List<string> ConnectionsAsList { get; set; }
        public HashSet<string> ConnectionsAsHashSet { get; set; }
    }
}
