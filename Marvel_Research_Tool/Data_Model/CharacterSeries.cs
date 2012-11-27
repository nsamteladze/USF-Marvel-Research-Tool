using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marvel_Research_Tool.Data_Model
{
    public class CharacterSeries
    {
        private HashSet<string> _series;

        public string ID { get; set; }
        public HashSet<string> Series
        {
            get
            {
                return _series;
            }
        }

        public CharacterSeries()
        {
            this.ID = Guid.NewGuid().ToString();
            _series = new HashSet<string>();
        }

        public CharacterSeries(string characterId)
        {
            this.ID = characterId;
            _series = new HashSet<string>();
        }

        public CharacterSeries(string characterId, IEnumerable<string> series)
        {
            this.ID = characterId;
            _series = new HashSet<string>(series);
        }

        public void AddSeries(string series)
        {
            _series.Add(series);
        }

        public void AddSeries(IEnumerable<string> series)
        {
            foreach (string singleSeries in series)
            {
                _series.Add(singleSeries);
            }
        }

        public int Count()
        {
            return _series.Count;
        }

        public bool Contains(string series)
        {
            return _series.Contains(series);
        }

        public string ToCSV()
        {
            string csvString = ID;

            foreach (string series in _series)
            {
                csvString += String.Format(",{0}", series);
            }

            return csvString;
        }
    }
}
