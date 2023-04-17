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
        private const int MAX_DISTANCE = 1000000000;
		private const int MIN_NAME = 1;
		private const int MAX_NAME = 50;
		private const int KILOMETER = 1000;
		private const int MINUTE = 60;

        public int Id { get; set; }
        public string Departure { get; set; }
        public int DepartureStationId { get; set; }
        public string DepartureStationName { get; set; }

        public string Return { get; set; }
        public int ReturnStationId { get; set; }
        public string ReturnStationName { get; set; }
        public float CoveredDistance { get; set; }
        public int Duration { get; set; }

        public Journey()
        {			
			this.Departure = "";
			this.Return = "";
			this.DepartureStationId = 0;
			this.DepartureStationName = "";
			this.ReturnStationId = 0;
			this.ReturnStationName = "";
			this.CoveredDistance = 0;
			this.Duration = 0;
        }

		public Journey? InitJourney(Journey journey, string rowData)
		{
			string[] data = rowData.Split(',');
			if (data == null)
				return null;

			journey.Departure = ParseName(data[0]);
			if (journey.Departure == "")
				return null;
			journey.Return = ParseName(data[1]);
			if (journey.Return == "")
				return null;

			journey.DepartureStationId = ParseNumber(data[2]);
			if (journey.DepartureStationId == 0)
				return null;

			journey.DepartureStationName = ParseName(data[3]);
			if (journey.DepartureStationName == "")
				return null;

			journey.ReturnStationId = ParseNumber(data[4]);
			if (journey.ReturnStationId == 0)
				return null;
			
			journey.ReturnStationName = ParseName(data[5]);
			if (journey.ReturnStationName == "")
				return null;

			journey.CoveredDistance = ParseNumber(data[6]);
			if (journey.CoveredDistance == 0)
				return null;
			journey.CoveredDistance /= KILOMETER;

            journey.Duration = ParseNumber(data[7]);
			if (journey.Duration == 0)
				return null;
			journey.Duration /= MINUTE;

            return journey;
		}

		private string ParseName(string name)
		{
			string result;
			result = name;
			if(result.Length > MIN_NAME && result.Length < MAX_NAME && result != null)
			{
				return result;
			}
			return "";
		}
		private int ParseNumber(string name)
		{
			int result = 0;
			if(int.TryParse(name, out result))
			{
				if(result > MIN_DISTANCE && result < MAX_DISTANCE)
				{
					return result;
				}
			}
			return 0;
		}

		public override string ToString()
			
		{
            string str = $"{Departure} {Return}: " + $"{DepartureStationId} {DepartureStationName} " + $"{ReturnStationId} {ReturnStationName} " + $"{CoveredDistance} {Duration}";

			return str;
		}
	}
}
