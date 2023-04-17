using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using MyBikeApp.Data;
using MyBikeApp.Models;
using CsvHelper;
using System.IO;
using System.Globalization;
using System.Linq;
using X.PagedList;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace MyBikeApp.Controllers
{
    public class JourneysController : Controller
    {
        String[] files = System.IO.Directory.GetFiles(@"C:\Users\Omistaja\source\repos\MyBikeApp\MyBikeApp\csv\");
		List<Journey> journeys = new List<Journey>();

        private readonly ApplicationDbContext _context;

        public JourneysController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CsvFile(string file)
        {
			int AddCount = 0;
			int ErrorCount = 0; ;
			string[] lines = System.IO.File.ReadAllLines(file);
			for (int i = 1; i < lines.Length; i++)
			{
				Journey journey = new Journey();
				journey = journey.InitJourney(journey, lines[i]);

				if (journey != null)
				{
					_context.Add(journey);
					AddCount++;
				}
				else
					ErrorCount++;
			}
			Console.WriteLine("added " + AddCount + " journeys to SQL database");
			Console.WriteLine("There was an error in " + ErrorCount + " journeys and they were not added to the database");
			await _context.SaveChangesAsync();
		}

        public async Task<ActionResult> ReadCsv()
        {
            
            Task task = Task.Run(async () =>
            {
                foreach (var file in files)
                {
					await CsvFile(file);
                }
            });
            await task;
            return View();
        }

    
        public async Task<ActionResult> ClearDatabase()
        {
            _context.Journey.RemoveRange(_context.Journey.Where(x=>x.DepartureStationName != ""));
            _context.SaveChanges();
            return View("ShowDatabase");
        }


        public void WriteJourneyConsole()
        {
            for (int i = 0; i < journeys.Count; i++)
            {
                Console.WriteLine(journeys[i]);
            }
        }

        public async Task<ActionResult<List<Journey>>> GetJourneys()
        {
            if(_context.Journey == null)
                return NotFound();

            var m_Journeys = await _context.Journey.ToListAsync();
            return Ok(m_Journeys);
        }     
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DepartureSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.ReturnSortParm = String.IsNullOrEmpty(sortOrder) ? "return_name_desc" : "";
            ViewBag.DurationSortParm = String.IsNullOrEmpty(sortOrder) ? "Duration" : "";
            ViewBag.LenghtSortParm = String.IsNullOrEmpty(sortOrder) ? "Lenght" : "";
            ViewBag.ReturnTimeSortParm = String.IsNullOrEmpty(sortOrder) ? "Return Time" : "";
            ViewBag.DepartureTimeSortParm = String.IsNullOrEmpty(sortOrder) ? "Departure Time" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            
            var m_Journeys = from s in _context.Journey
                             select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                m_Journeys = m_Journeys.Where(s => s.DepartureStationName.Contains(searchString)
                                       || s.ReturnStationName.Contains(searchString));
            }
            
            switch (sortOrder)
            {
                case "return_name_desc":
                    m_Journeys = m_Journeys.OrderBy(s => s.ReturnStationName);
                    break;
                case "name_desc":
                    m_Journeys = m_Journeys.OrderByDescending(s => s.DepartureStationName);
                    break;
                case "date_desc":
                    m_Journeys = m_Journeys.OrderByDescending(s => s.Departure);
                    break;
                case "Lenght":
                    m_Journeys = m_Journeys.OrderByDescending(s => s.CoveredDistance);
                    break;
                case "Duration":
                    m_Journeys = m_Journeys.OrderByDescending(s => s.Duration);
                    Console.WriteLine("sorting by duration");
                    break;
                case "Return Time":
                    m_Journeys = m_Journeys.OrderByDescending(s => s.Return);
                    break;
                case "Departure Time":
                    m_Journeys = m_Journeys.OrderByDescending(s => s.Departure);
                    break;
                default:
                    m_Journeys = m_Journeys.OrderBy(s => s.DepartureStationName);
                    break;
            }
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(m_Journeys.ToPagedList(pageNumber, pageSize));
        }

        // GET: Stations/ShowSearchForm
        public async Task<IActionResult> ShowSearchForm()
		{
           
			return 	View();
		}
        public async Task<IActionResult> ShowDatabase()
        {

            return View();
        }



        // GET: Stations/ShowSearchResults top 5
        public async Task<ViewResult> ShowSearchResults(String searchString)
		{
			var result = _context.Journey
			  .Where(d => d.DepartureStationName.Contains(searchString))
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
			return View("Index2", newList);
        }

        // GET: Journey/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Journey == null)
            {
                return NotFound();
            }

            var journey = await _context.Journey
                .FirstOrDefaultAsync(m => m.Id == id);
            if (journey == null)
            {
                return NotFound();
            }

            return View(journey);
        }

        [Authorize]
        // GET: Journey/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Stations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]

       
        public async Task<IActionResult> Create([Bind("DepartureStationName, Departure, Return")] Journey journey)
        {
            if (ModelState.IsValid)
            {
                _context.Add(journey);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(journey);
        }

        // GET: Stations/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Journey == null)
            {
                return NotFound();
            }

            var journey = await _context.Journey.FindAsync(id);
            if (journey == null)
            {
                return NotFound();
            }
            return View(journey);
        }
        [Authorize]
        // POST: Stations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DepartureStationName, Departure, Return")] Journey journey)
        {
            if (id != journey.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(journey);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JourneyExists(journey.Id))
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
            return View(journey);
        }
        [Authorize]
        // GET: Journeys/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Journey == null)
            {
                return NotFound();
            }

            var journey = await _context.Journey
                .FirstOrDefaultAsync(m => m.Id == id);
            if (journey == null)
            {
                return NotFound();
            }

            return View(journey);
        }
        [Authorize]
        // POST: Journeys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Journey == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Journey'  is null.");
            }
            var journey = await _context.Journey.FindAsync(id);
            if (journey != null)
            {
                _context.Journey.Remove(journey);
            }
            else
                Console.WriteLine("ERROR: delete journey: jorney null");
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JourneyExists(int id)
        {
          return (_context.Journey?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
