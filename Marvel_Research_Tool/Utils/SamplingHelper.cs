using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marvel_Research_Tool.Data_Model;

namespace Marvel_Research_Tool.Utils
{
    public class SamplingHelper
    {
        public static SetType GetSetTypeOrDefault(string setType)
        {
            switch (setType)
            {
                case "balanced":
                    return SetType.Balanced;
                case "proportional":
                    return SetType.Proportional;
                case "nodes":
                    return SetType.Nodes;
                default:
                    return SetType.Balanced;
            }
        }
    }
}
