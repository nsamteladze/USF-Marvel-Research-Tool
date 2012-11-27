using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marvel_Research_Tool.Data_Model
{
    public class ComicKey
    {
        public string Key { get; set; }
        public string FullName { get; set; }

        public ComicKey()
        {
            Key = String.Empty;
            FullName = String.Empty;
        }

        public ComicKey(string key, string fullName)
            : this()
        {
            this.Key = key;
            this.FullName = fullName;
        }
    }
}
