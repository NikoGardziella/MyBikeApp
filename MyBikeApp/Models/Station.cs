namespace MyBikeApp.Models
{
    public class Station
    {
        public int Id { get; set; }

		public string Name { get; set; }
        public string Address { get; set; }


        public Station()
        {
          //  this. = 0;
            this.Name = "";
            this.Address = "";
        }

        public Station InitStation(Station station, string rowData)
        {
            string[] data = rowData.Split(',');
            if (data == null)
                return null;

           // station.Id = int.Parse(data[1]);
            station.Name = data[2];
            station.Address = data[5];
            return station;
        }
    }
}

