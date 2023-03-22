using Microsoft.CodeAnalysis.CSharp.Syntax;
using static System.Collections.Specialized.BitVector32;


namespace MyBikeApp.Models
{
    public class Journey
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


        public Journey()
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

		public Journey InitJourney(Journey journey, string rowData)
		{
			string[] data = rowData.Split(',');
			journey.Departure = data[0];
			journey.Return = data[1];
			journey.DepartureStationId = int.Parse(data[2]);
			journey.DepartureStationName = data[3];
			journey.ReturnStationId = int.Parse(data[4]);
			journey.ReturnStationName = data[5];
			journey.CoveredDistanceM = int.Parse(data[6]);
			journey.DurationSec = int.Parse(data[7]);
			return journey;
		} 


		public override string ToString()
			
		{
            string str = $"{Departure} {Return}: " + $"{DepartureStationId} {DepartureStationName} " + $"{ReturnStationId} {ReturnStationName} " + $"{CoveredDistanceM} {DurationSec}";

			return str;
		} 

	}
}
