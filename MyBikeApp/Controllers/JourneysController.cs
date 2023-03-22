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


        public void CsvHelperReader()
        {
            using(var streamReader = new StreamReader(@"C:\Users\Omistaja\source\repos\MyBikeApp\MyBikeApp\csv\shortlist.csv"))
            {
                using(var csvreader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    var record = csvreader.GetRecord<dynamic>();

                }
            }
        }

		public async Task ReadCsv()
		{
            Task task = Task.Run(() =>
            {
                // Dont read first line
                for (int i = 1; i < csvLines.Length; i++)
                {
                    Journey journey = new Journey();
                    journey = journey.InitStation(journey, csvLines[i]);
                    journeys.Add(journey);
                }
                

            });
			await task;
		}

		

		public void  WriteStationConsole()
		{
			for (int i = 0; i < journeys.Count; i++)
			{
				Console.WriteLine(journeys[i]);
			}
		}



		// GET: Stations
		public async Task<IActionResult> Index()
        {
			await ReadCsv();
			WriteStationConsole();
			return View(journeys);
        }

		

		// GET: Stations/ShowSearchForm
		public async Task<IActionResult> ShowSearchForm()
		{
           
			return 	View();
		}

       


		// GET: Stations/ShowSearchResults
		public async Task<IActionResult> ShowSearchResults(String SearchPhrase)
		{
			
            return View("Index", await _context.Station.Where( j => j.Departure.ToString().Contains(SearchPhrase)).ToListAsync());
            

		}

		// GET: Stations/Details/5
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

            return View(station);
        }

        [Authorize]
        // GET: Stations/Create
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
        public async Task<IActionResult> Create([Bind("Id,StartStations,EndStation")] Journey station)
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
        [Authorize]
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
        [Authorize]
        // POST: Stations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartStations,EndStation")] Journey station)
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
        [Authorize]
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
        [Authorize]
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
