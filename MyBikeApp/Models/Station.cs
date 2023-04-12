using System.Globalization;
using System.Xml;

namespace MyBikeApp.Models
{
    public class Station
    {
        public int Id { get; set; }

		public string Name { get; set; }
        public string Address { get; set; }

        public string Lat { get; set; }
		public string Lon { get; set; }

        readonly System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
		public Station()
        {
          //  this. = 0;
            this.Name = "";
            this.Address = "";
            this.Lat = "";
            this.Lon = "";
            

        }

     
        public Station InitStation(Station station, string rowData)
        {
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
           // System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            string[] data = rowData.Split(',');
            if (data == null)
                return null;
            
            // station.Id = int.Parse(data[1]);
            station.Name = data[2];
            station.Address = data[5];
            station.Lon = data[11];
            station.Lat = data[12];

            return station;
        }
    }
}

