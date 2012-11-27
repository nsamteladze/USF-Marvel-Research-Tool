using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marvel_Research_Tool.Data_Model
{
    public class CharacterCharacters
    {
        private HashSet<string> _characters;

        public string ID { get; set; }
        public HashSet<string> Characters
        {
            get
            {
                return _characters;
            }
        }

        public CharacterCharacters()
        {
            this.ID = Guid.NewGuid().ToString();
            _characters = new HashSet<string>();
        }

        public CharacterCharacters(string characterId)
        {
            this.ID = characterId;
            _characters = new HashSet<string>();
        }

        public CharacterCharacters(string characterId, IEnumerable<string> characters)
        {
            this.ID = characterId;
            _characters = new HashSet<string>(characters);
        }

        public void AddCharacters(string character)
        {
            _characters.Add(character);
        }

        public void AddCharacters(IEnumerable<string> characters)
        {
            foreach (string singleCharacter in characters)
            {
                _characters.Add(singleCharacter);
            }
        }

        public int Count()
        {
            return _characters.Count;
        }

        public bool Contains(string character)
        {
            return _characters.Contains(character);
        }

        public string ToCSV()
        {
            string csvString = ID;

            foreach (string character in _characters)
            {
                csvString += String.Format(",{0}", character);
            }

            return csvString;
        }
    }
}
