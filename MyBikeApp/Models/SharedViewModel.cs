using JetBrains.Annotations;

namespace MyBikeApp.Models
{
	public class SharedViewModel
	{

		public SharedViewModel()
		{
			m_Journey = new Journey();
		}
		public Journey m_Journey { get; set; }
		public Station m_Station { get; set; }

		public int TripsStart()
		{
			int count = 0;

			return count;
		}

	}
}
