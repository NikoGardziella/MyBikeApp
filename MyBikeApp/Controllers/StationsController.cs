using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyBikeApp.Data;
using MyBikeApp.Models;
using X.PagedList;

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

            var result = _context.Journey
              .Where(d => d.DepartureStationName.Contains(station.Name))
              .GroupBy(q => q.ReturnStationName)
              .OrderByDescending(g => g.Count())
              .Take(5)
              .Select(g => g.Key)
              .ToList();

            List<MyBikeApp.Models.Journey> newList = new List<MyBikeApp.Models.Journey>();
            foreach (var item in result)
            {
                MyBikeApp.Models.Journey listItem = new MyBikeApp.Models.Journey();
                listItem.ReturnStationName = item;
                newList.Add(listItem);
            }
            return View("Details", newList);


         //   return View(station);
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
