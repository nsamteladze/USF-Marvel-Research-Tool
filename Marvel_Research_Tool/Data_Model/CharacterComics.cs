using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marvel_Research_Tool.Data_Model
{
    public class CharacterComics
    {
        private HashSet<string> _comics;

        public string ID { get; set; }
        public HashSet<string> Comics
        {
            get
            {
                return _comics;
            }
        }

        public CharacterComics()
        {
            this.ID = Guid.NewGuid().ToString();
            _comics = new HashSet<string>();
        }

        public CharacterComics(string id)
        {
            this.ID = id;
            _comics = new HashSet<string>();
        }

        public CharacterComics(IEnumerable<string> comicsCollection)
        {
            this.ID = Guid.NewGuid().ToString();
            _comics = new HashSet<string>(comicsCollection);
        }

        public CharacterComics(string id, IEnumerable<string> comicsCollection)
        {
            this.ID = id;
            _comics = new HashSet<string>(comicsCollection);
        }

        public void AddComic(string comic)
        {
            _comics.Add(comic);
        }

        public void AddComic(string[] comics)
        {
            foreach (string entry in comics)
            {
                _comics.Add(entry);
            }
        }

        public void AddComic(List<string> comics)
        {
            foreach (string entry in comics)
            {
                _comics.Add(entry);
            }
        }

        public string ToCSV()
        {
            string csvString = ID;

            foreach (string comic in _comics)
            {
                csvString += String.Format(",{0}", comic);
            }

            return csvString;
        }
    }
}
