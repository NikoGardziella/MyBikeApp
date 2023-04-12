using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MyBikeApp.Data;
using Newtonsoft.Json.Linq;
using static System.Collections.Specialized.BitVector32;

// my Google api key AIzaSyCzqA-6S-F8Ka-EvrKoluV6UMnBDQC0aKg

namespace MyBikeApp.Models
{
    public class Journey
    {
		private const int MIN_DISTANCE = 10;
        private const int MIN_DURATION= 10;

        public int Id { get; set; }
        public string Departure { get; set; }
        public int DepartureStationId { get; set; }
        public string DepartureStationName { get; set; }

        public string Return { get; set; }
        public int ReturnStationId { get; set; }
        public string ReturnStationName { get; set; }
        public float CoveredDistanceM { get; set; }
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
			if (data == null)
				return null;
			for (int i = 0; i < data.Length; i++)
			{
				if (data[i] == null)
					return null;
			}
			float distance;
			journey.Departure = data[0];
			journey.Return = data[1];
			journey.DepartureStationId = int.Parse(data[2]);
			journey.DepartureStationName = data[3];
            int returnStationId;
            if (int.TryParse(data[4], out returnStationId))
            {
                journey.ReturnStationId = returnStationId;
            }
            journey.ReturnStationName = data[5];

			distance = CheckDistance(data[6]);
			if (distance > MIN_DISTANCE)
				journey.CoveredDistanceM = distance;
			else
				return null;
            
			journey.DurationSec = int.Parse(data[7]);
			return journey;
		}

		float CheckDistance(string data)
        {
			float result;
            if (float.TryParse(data, out result))
            {
                if (result > MIN_DISTANCE)
                    return result;
            }

			return 0;
		}


		public override string ToString()
			
		{
            string str = $"{Departure} {Return}: " + $"{DepartureStationId} {DepartureStationName} " + $"{ReturnStationId} {ReturnStationName} " + $"{CoveredDistanceM} {DurationSec}";

			return str;
		} 

	}
}
