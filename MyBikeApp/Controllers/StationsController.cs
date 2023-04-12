using System;
using System.Collections.Generic;

using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyBikeApp.Data;
using MyBikeApp.Models;
using Newtonsoft.Json.Linq;
using X.PagedList;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;

namespace MyBikeApp.Controllers
{
    public class StationsController : Controller
    {
		private StationsController _Instance = null; public StationsController Instance { get { return _Instance; } set { _Instance = value; } }
        

		private readonly ApplicationDbContext _context;
		String[] csvLines = System.IO.File.ReadAllLines(@"C:\Users\Omistaja\source\repos\MyBikeApp\MyBikeApp\csv\Helsingin_ja_Espoon_kaupunkipyöräasemat_avoin.csv");


		public StationsController(ApplicationDbContext context)
        {
            _context = context;
        }

		public async Task<ActionResult> ReadCsv()
		{
		//	Console.WriteLine("Reading csv");
			Task task = Task.Run(async () =>
			{
				// Dont read first line
				for (int i = 1; i < csvLines.Length; i++)
				{
			//		Console.WriteLine("station: " + i);
                    Station station = new Station();
					station = station.InitStation(station, csvLines[i]);
					if (station != null)
					{
						await Create(station);
					}
					// _context.SaveChanges();

				}
			//	Console.WriteLine("added " + csvLines.Length + " stations");
				await _context.SaveChangesAsync();
			});

			await task;
			return View();
		}

		public async Task<ActionResult> ClearDatabase()
		{
	//		int beforeCount = _context.Station.Count();
	//		Console.WriteLine("before clear:" + beforeCount);
            if(_context.Station != null)
			   _context.Station.RemoveRange(_context.Station.Where(x => x.Name != ""));
			_context.SaveChanges();

		//	int afterCount = _context.Station.Count();
		//	Console.WriteLine("after clear: " + afterCount);
			return View("ShowDatabase");
		}

		public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
		{
			ViewBag.CurrentSort = sortOrder;
			ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
			ViewBag.AddressSortParm = String.IsNullOrEmpty(sortOrder) ? "return_name_desc" : "";

		
			if (searchString != null)
			{
				page = 1;
			}
			else
			{
				searchString = currentFilter;
			}

			ViewBag.CurrentFilter = searchString;

			var m_Stations = from s in _context.Station
							 select s;


			if (!String.IsNullOrEmpty(searchString))
			{
				m_Stations = m_Stations.Where(s => s.Name.Contains(searchString)
									   || s.Address.Contains(searchString));
			}
			switch (sortOrder)
			{
				case "return_name_desc":
					m_Stations = m_Stations.OrderBy(s => s.Name);
					break;
				case "name_desc":
					m_Stations = m_Stations.OrderByDescending(s => s.Address);
					break;
                default:
					m_Stations = m_Stations.OrderBy(s => s.Name);
					break;
			}
			int pageSize = 20;
			int pageNumber = (page ?? 1);
			return View(m_Stations.ToPagedList(pageNumber, pageSize));
		}

		// GET: Stations
	/*	public async Task<IActionResult> Index()
        {
              return _context.Station != null ? 
                          View(await _context.Station.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Station'  is null.");
        } */

        // GET: Stations/Details/5 Show station details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Station == null)
            {
                return NotFound();
            }

            var station = await _context.Station
                .FirstOrDefaultAsync(m => m.Id == id);
            if (station == null)
            {
                return NotFound();
            }

            SharedViewModel StationInfo = new SharedViewModel();

            GetTopFiveDestinations(StationInfo, station);
			GetTopFiveDepartureStations(StationInfo, station);
			GetAverageDistances(StationInfo, station);
			GetGoogleMapsCoordinates(StationInfo, station);
			
            return View("Details", StationInfo);
		}
		public void GetGoogleMapsCoordinates(SharedViewModel StationInfo, Station station)
		{
			var _lat = _context.Station
				 .Where(d => d.Name.Contains(station.Name)).Select(g => g.Lat).FirstOrDefault();
			var _lng = _context.Station
				 .Where(d => d.Name.Contains(station.Name)).Select(g => g.Lon).FirstOrDefault();
			var culture = CorrectNumberFormat();

			string temp_lat = _lat.ToString();
			string temp_lng = _lng.ToString();
			// Console.WriteLine(temp_lat);
			// Console.WriteLine(temp_lng);

			if (!double.TryParse(temp_lat, System.Globalization.NumberStyles.Any, culture, out StationInfo.lat))
				Console.WriteLine("Error parsin lat");
			if (!double.TryParse(temp_lng, System.Globalization.NumberStyles.Any, culture, out StationInfo.lng))
				Console.WriteLine("Error parsin long");
			//Console.Write("lat: " + StationInfo.lat + " lng: " + StationInfo.lng);
		}
		public void GetAverageDistances(SharedViewModel StationInfo, Station station)
        {
			StationInfo.TripsFromThisStation = _context.Journey
			  .Where(d => d.DepartureStationName.Contains(station.Name))
			  .ToList()
			  .Count;

			StationInfo.TripsToThisStation = _context.Journey
		   .Where(d => d.ReturnStationName.Contains(station.Name))
		   .ToList()
		   .Count;

			StationInfo.AverageDistanceFromStation = ((int)_context.Journey
			  .Where(d => d.DepartureStationName.Contains(station.Name))
			  .Select(d => d.CoveredDistanceM).Sum());
			if(StationInfo.TripsFromThisStation > 0)
				StationInfo.AverageDistanceFromStation = StationInfo.AverageDistanceFromStation / StationInfo.TripsFromThisStation;
			//Console.Write("AverageDistanceFromStation: " + StationInfo.AverageDistanceFromStation);

			StationInfo.AverageDistanceToStation = ((int)_context.Journey
			  .Where(d => d.ReturnStationName.Contains(station.Name))
			  .Select(d => d.CoveredDistanceM).Sum());
			if(StationInfo.TripsToThisStation > 0)
				StationInfo.AverageDistanceToStation = StationInfo.AverageDistanceToStation / StationInfo.TripsToThisStation;
			//Console.Write("AverageDistanceToStation: " + StationInfo.AverageDistanceToStation);
		}

		public void GetTopFiveDestinations(SharedViewModel StationInfo, Station station)
		{
			List<Journey> newList = new List<Journey>();
			var result = _context.Journey
			  .Where(d => d.DepartureStationName.Contains(station.Name))
			  .GroupBy(q => q.ReturnStationName)
			  .OrderByDescending(g => g.Count())
			  .Take(5)
			  .Select(g => g.Key)
			  .ToList();

			foreach (var item in result)
			{
				MyBikeApp.Models.Journey listItem = new MyBikeApp.Models.Journey();
				listItem.ReturnStationName = item;
				StationInfo.JourneysFromStation.Add(listItem);
			}
		}
        public void GetTopFiveDepartureStations(SharedViewModel StationInfo, Station station)
        {
			var result = _context.Journey
			  .Where(d => d.ReturnStationName.Contains(station.Name))
			  .GroupBy(q => q.DepartureStationName)
			  .OrderByDescending(g => g.Count())
			  .Take(5)
			  .Select(g => g.Key)
			  .ToList();

			foreach (var item in result)
			{
				MyBikeApp.Models.Journey listItem = new MyBikeApp.Models.Journey();
				listItem.DepartureStationName = item;
				StationInfo.JourneysToStation.Add(listItem);
			}
		}

		
       

		public static CultureInfo CorrectNumberFormat()
		{
			var culture = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
			if (culture.NumberFormat.NumberDecimalSeparator != ".")
			{
				culture.NumberFormat.NumberDecimalSeparator = ".";
				culture.NumberFormat.CurrencyDecimalSeparator = ".";
				culture.NumberFormat.PercentDecimalSeparator = ".";
				CultureInfo.DefaultThreadCurrentCulture = culture;
				CultureInfo.DefaultThreadCurrentUICulture = culture;

				Thread.CurrentThread.CurrentCulture = culture;
				Thread.CurrentThread.CurrentUICulture = culture;
			}
            return culture;
		}

		// GET: Stations/Create
		public IActionResult Create()
        {
            return View();
        }

        // POST: Stations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address")] Station station)
        {
            if (ModelState.IsValid)
            {
                _context.Add(station);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(station);
        }

        // GET: Stations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Station == null)
            {
                return NotFound();
            }

            var station = await _context.Station.FindAsync(id);
            if (station == null)
            {
                return NotFound();
            }
            return View(station);
        }

        // POST: Stations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address")] Station station)
        {
            if (id != station.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(station);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StationExists(station.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(station);
        }

        // GET: Stations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Station == null)
            {
                return NotFound();
            }

            var station = await _context.Station
                .FirstOrDefaultAsync(m => m.Id == id);
            if (station == null)
            {
                return NotFound();
            }

            return View(station);
        }

        // POST: Stations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Station == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Station'  is null.");
            }
            var station = await _context.Station.FindAsync(id);
            if (station != null)
            {
                _context.Station.Remove(station);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StationExists(int id)
        {
          return (_context.Station?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
