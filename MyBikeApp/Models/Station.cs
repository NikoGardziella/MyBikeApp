using Microsoft.CodeAnalysis.CSharp.Syntax;
using static System.Collections.Specialized.BitVector32;

namespace MyBikeApp.Models
{
    public class Station
    {
        public int Id { get; set; }
        public string Departure { get; set; }
        public int DepartureStationId { get; set; }
        public string DepartureStationName { get; set; }

        public string Return { get; set; }
        public int ReturnStationId { get; set; }
        public string ReturnStationName { get; set; }
        public int CoveredDistanceM { get; set; }
        public int DurationSec { get; set; }


        public Station()
        {
			
			this.Departure = "";
			this.Return = "";
			this.DepartureStationId = 0;
			this.DepartureStationName = "";
			this.ReturnStationId = 0;
			this.ReturnStationName = "";
			this.CoveredDistanceM = 0;
			this.DurationSec = 0; 
		}

		public Station InitStation(Station station, string rowData)
		{
			string[] data = rowData.Split(',');
			this.Departure = data[0];
			this.Return = data[1];
			this.DepartureStationId = int.Parse(data[2]);
			this.DepartureStationName = data[3];
			this.ReturnStationId = int.Parse(data[4]);
			this.ReturnStationName = data[5];
			this.CoveredDistanceM = int.Parse(data[6]);
			this.DurationSec = int.Parse(data[7]);
			return station;
		} 


		/*public override string ToString()
			
		{
            string str = $"{Departure} {Return}: " + $"{DepartureStationId} {DepartureStationName} " + $"{ReturnStationId} {ReturnStationName} " + $"{CoveredDistanceM} {DurationSec}";

			return str;
		} */

	}
}
