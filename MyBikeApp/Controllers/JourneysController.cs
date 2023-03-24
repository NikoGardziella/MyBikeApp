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

namespace MyBikeApp.Controllers
{
    public class JourneysController : Controller
    {

        String[] csvLines = System.IO.File.ReadAllLines(@"C:\Users\Omistaja\source\repos\MyBikeApp\MyBikeApp\csv\shortlist.csv");

        List<Journey> journeys = new List<Journey>();

        private readonly ApplicationDbContext _context;

        public JourneysController(ApplicationDbContext context)
        {
            _context = context;
        }


     /*   public void CsvHelperReader()
        {
            using (var streamReader = new StreamReader(@"C:\Users\Omistaja\source\repos\MyBikeApp\MyBikeApp\csv\shortlist.csv"))
            {
                using (var csvreader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    var record = csvreader.GetRecord<dynamic>();

                }
            }
        } */

        public async Task<ActionResult> ReadCsv()
        {
            Console.WriteLine("Reading csv");
            Task task = Task.Run(async () =>
            {
                // Dont read first line
                for (int i = 1; i < csvLines.Length; i++)
                {
                    Console.WriteLine("journey: " + i);
                    Journey journey = new Journey();
                    journey = journey.InitJourney(journey, csvLines[i]);
                    journeys.Add(journey);
                  //  _context.Add(journey);
                  if(journey != null)
                    {
                        await Create(journey);
                    }
                   // _context.SaveChanges();

                }
                Console.WriteLine("added "+ csvLines.Length +" journeys");
                await _context.SaveChangesAsync();
            });
            await task;
            return View();
        }

    
        public async Task<ActionResult> ClearDatabase()
        {
            int beforeCount = _context.Journey.Count();
            Console.WriteLine("before clear:" + beforeCount);           
            
            _context.Journey.RemoveRange(_context.Journey.Where(x=>x.DepartureStationName != ""));
            _context.SaveChanges();

            int afterCount = _context.Journey.Count();
            Console.WriteLine("after clear: " + afterCount);
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


        // source: https://learn.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application
        public async Task<ViewResult> IndexAsync(string sortOrder, string currentFilter, string searchString, int? page)
        {
            // ReadCsv();
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

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
                case "name_desc":
                    m_Journeys = m_Journeys.OrderByDescending(s => s.ReturnStationName);
                    break;
                case "Date":
                    m_Journeys = m_Journeys.OrderBy(s => s.Departure);
                    break;
                case "date_desc":
                    m_Journeys = m_Journeys.OrderByDescending(s => s.Departure);
                    break;
                default:
                    m_Journeys = m_Journeys.OrderBy(s => s.Departure);
                    break;
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(m_Journeys.ToPagedList(pageNumber, pageSize));
            
        }

        // GET: Journeys
        /*	public async Task<IActionResult> Index(int pageNumber = 1)
            {
                await ReadCsv();
                WriteJourneyConsole();

                return View(await PaginatedList<Journey>.CreateASync(_context.Journey, pageNumber,5));
              //  return View(journeys);
            }*/



        // GET: Stations/ShowSearchForm
        public async Task<IActionResult> ShowSearchForm()
		{
           
			return 	View();
		}
        public async Task<IActionResult> ShowDatabase()
        {

            return View();
        }



        // GET: Stations/ShowSearchResults
        public async Task<IActionResult> ShowSearchResults(String SearchPhrase)
		{
			
            return View("Index", await _context.Journey.Where( j => j.Departure.ToString().Contains(SearchPhrase)).ToListAsync());
            

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
                return RedirectToAction(nameof(IndexAsync));
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
                return RedirectToAction(nameof(IndexAsync));
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
            return RedirectToAction(nameof(IndexAsync));
        }

        private bool JourneyExists(int id)
        {
          return (_context.Journey?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
