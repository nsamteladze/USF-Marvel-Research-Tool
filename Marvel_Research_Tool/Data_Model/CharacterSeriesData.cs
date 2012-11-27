using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marvel_Research_Tool.Data_Model
{
    public class CharacterSeriesData
    {
        private Dictionary<string, HashSet<string>> _data;

        public Dictionary<string, HashSet<string>> Data
        {
            get
            {
                return _data;
            }
        }

        public CharacterSeriesData()
        {
            _data = new Dictionary<string, HashSet<string>>();
        }

        public CharacterSeriesData(IDictionary<string, HashSet<string>> data)
        {
            _data = new Dictionary<string, HashSet<string>>(data);
        }

        public void Add(string characterId, IEnumerable<string> series)
        {
            if (!_data.ContainsKey(characterId))
            {
                _data.Add(characterId, new HashSet<string>(series));
            }
        }

        public int Count()
        {
            return _data.Count;
        }

        public bool Contains(string characterId)
        {
            return _data.ContainsKey(characterId);
        }

        public bool TryGetValue(string characterId, out HashSet<string> series)
        {
            return _data.TryGetValue(characterId, out series);
        }
    }
}
