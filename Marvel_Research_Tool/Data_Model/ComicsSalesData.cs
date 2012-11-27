using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marvel_Research_Tool.Data_Model
{
    public class ComicsSalesData
    {
        public Dictionary<string, int> Sales
        {
            get;
            private set;
        }

        public ComicsSalesData()
        {
            Sales = new Dictionary<string, int>();
        }

        public ComicsSalesData(Comic comic)
            : this()
        {
            this.Sales.Add(comic.Name, comic.NumSold);
        }

        public ComicsSalesData(IDictionary<string, int> sales)
        {
            this.Sales = new Dictionary<string, int>(sales);
        }

        public void Add(string comic, int numSold)
        {
            int tempSales;

            if (Sales.TryGetValue(comic, out tempSales))
            {
                tempSales += numSold;
            }
            else
            {
                Sales.Add(comic, numSold);
            }
        }

        public void Add(ComicsSalesData data)
        {
            int tempSales;

            foreach (KeyValuePair<string, int> comicSales in data.Sales)
            {
                if (Sales.TryGetValue(comicSales.Key, out tempSales))
                {
                    tempSales += comicSales.Value;
                }
                else
                {
                    Sales.Add(comicSales.Key, comicSales.Value);
                }
            }
        }

        public void Add(Comic data)
        {
            int tempSales;

            if (Sales.TryGetValue(data.Name, out tempSales))
            {
                tempSales += data.NumSold;
            }
            else
            {
                Sales.Add(data.Name, data.NumSold);
            }
        }

        public bool Contains(string comic)
        {
            return Sales.ContainsKey(comic);
        }

        public int Count()
        {
            return Sales.Count;
        }
    }
}
