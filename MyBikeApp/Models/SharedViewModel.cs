using JetBrains.Annotations;

namespace MyBikeApp.Models
{
	public class SharedViewModel
	{

		public SharedViewModel()
		{
			JourneysFromStation = new List<MyBikeApp.Models.Journey>();
			JourneysToStation = new List<MyBikeApp.Models.Journey>();

		}
		public List<MyBikeApp.Models.Journey> JourneysFromStation { get; set; }
		public List<MyBikeApp.Models.Journey> JourneysToStation { get; set; }

		public Station m_Station { get; set; }

		public int TripsFromThisStation = 0;
        public int TripsToThisStation = 0;
		public string StationName = "";
        public string Address = "";
		public double lat;
        public double lng;
		public int AverageDistanceToStation = 0;
		public int AverageDistanceFromStation = 0;
		public int TripsStart()
		{
			int count = 0;

			return count;
		}

	}
}
