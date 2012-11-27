using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marvel_Research_Tool.Data_Model
{
    public class CharactersTreeNode
    {
        private CharactersTreeNode _parent;
        private HashSet<CharactersTreeNode> _childred;
        private string _id;
        private HashSet<string> _path;

        public CharactersTreeNode Parent
        {
            get
            {
                return _parent;
            }
        }

        public HashSet<CharactersTreeNode> Children
        {
            get
            {
                return _childred;
            }
        }

        public string ID
        {
            get
            {
                return _id;
            }
        }

        public HashSet<string> Path
        {
            get
            {
                return _path;
            }
        }

        public CharactersTreeNode()
        {
            _id = Guid.NewGuid().ToString();
            _parent = null;
            _childred = new HashSet<CharactersTreeNode>();
            _path = new HashSet<string>();
            _path.Add(_id);
        }

        public CharactersTreeNode(string id, CharactersTreeNode parent)
        {
            _id = id;
            _parent = parent;
            _childred = new HashSet<CharactersTreeNode>();
            _path = new HashSet<string>();
            if (parent != null)
            {
                foreach (string pathNode in parent.Path)
                {
                    _path.Add(pathNode);
                }
            }
            _path.Add(_id);
        }

        public CharactersTreeNode(string id, CharactersTreeNode parent, IEnumerable<CharactersTreeNode> children)
        {
            _id = id;
            _parent = parent;
            _childred = new HashSet<CharactersTreeNode>(children);
            _path = new HashSet<string>();
            if (parent != null)
            {
                foreach (string pathNode in parent.Path)
                {
                    _path.Add(pathNode);
                }
            }
            _path.Add(_id);
        }

        public int ChildrenCount()
        {
            return _childred.Count();
        }

        public void LoadChildren(HashSet<string> connections)
        {
            foreach (string connectedCharacterId in connections)
            {
                if (!_path.Contains(connectedCharacterId))
                {
                    _childred.Add(new CharactersTreeNode(connectedCharacterId, this));
                }
            }
        }

        public int PathLength()
        {
            return (_path.Count - 1);
        }

    }
}
