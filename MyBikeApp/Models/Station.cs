using System.Globalization;
using System.Xml;

namespace MyBikeApp.Models
{
    public class Station
    {
		private const int MIN_DISTANCE = 10;
		private const int MAX_DISTANCE = 1000000000;
		private const int MIN_NAME = 1;
		private const int MAX_NAME = 100;
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

     
        public Station? InitStation(Station station, string rowData)
        {
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            string[] data = rowData.Split(',');
            if (data == null)
                return null;
            
            station.Name = ParseName(data[2]);
			if (station.Name == "")
				return null;
			station.Address = ParseName(data[5]);
			if (station.Address == "")
				return null;
			station.Lon = ParseName(data[11]);
			if (station.Lon == "")
				return null;
			station.Lat = ParseName(data[12]);
			if (station.Lat == "")
				return null;
			return station;
        }

		private string ParseName(string name)
		{
			string result;
			result = name;
			if (result.Length > MIN_NAME && result.Length < MAX_NAME && result != null)
			{
				return result;
			}
			return "";
		}
		private int ParseNumber(string name)
		{
			int result = 0;
			if (int.TryParse(name, out result))
			{
				if (result > MIN_DISTANCE && result < MAX_DISTANCE)
				{
					return result;
				}
			}
			return 0;
		}
	}
}

