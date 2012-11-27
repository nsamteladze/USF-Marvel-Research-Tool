using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marvel_Research_Tool.Data_Model
{
    public class SeriesSalesData
    {
        public Dictionary<string, ComicsSalesData> Sales
        {
            get;
            private set;
        }

        public SeriesSalesData()
        {
            this.Sales = new Dictionary<string, ComicsSalesData>();
        }

        public SeriesSalesData(IDictionary<string, ComicsSalesData> data)
        {
            this.Sales = new Dictionary<string, ComicsSalesData>(data);
        }

        public void Add(string series, ComicsSalesData comicsSales)
        {
            ComicsSalesData tempComicsSales;

            if (Sales.TryGetValue(series, out tempComicsSales))
            {
                tempComicsSales.Add(comicsSales);
            }
            else
            {
                Sales.Add(series, comicsSales);
            }
        }

        public void Add(string series, Comic comic)
        {
            ComicsSalesData tempComicsSales;

            if (Sales.TryGetValue(series, out tempComicsSales))
            {
                tempComicsSales.Add(comic);
            }
            else
            {
                Sales.Add(series, new ComicsSalesData(comic));
            }
        }

        public int Count()
        {
            return Sales.Count();
        }

        public bool Contains(string series)
        {
            return Sales.ContainsKey(series);
        }
    }
}
