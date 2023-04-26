using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyBikeApp.Data;
using MyBikeApp.Models;

namespace MyBikeApp.Controllers
{
    public class LoginViewModelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginViewModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Login(LoginViewModel vm)
        {
            if(string.IsNullOrEmpty(vm.Password) || string.IsNullOrEmpty(vm.Username))
            {
                return View();
            }
            else
            {
                bool  IsFind = SignInMethod(vm.Username, vm.Password);
                if(IsFind == true)
                {
                    return RedirectToAction("Index", "Default");
                }
                return View("Login");
            }
        }

        private bool SignInMethod(string userName, string password)
        {
            bool flag = false;
            var users = from s in _context.LoginViewModel
                     select s;
            var user = users.Where(s => s.Username == userName);
            string m_UserName = user.First().ToString();
            if (m_UserName == null)
                return false;
            if (user.First().Password == password)
            {
                if(m_UserName == "admin")
                {
                    HttpContext.Session.SetString("Role", "admin");
                    HttpContext.Session.SetString("UserName", m_UserName);
                }
                else
                {
                    HttpContext.Session.SetString("Role", "user");
                    HttpContext.Session.SetString("UserName", m_UserName);
                }
                return true;
            }
            else 
                return false;
          //  string query = $"select  * from [users] where  Username='{userName}' and Password='{password}'";
            
        }

        


        // GET: LoginViewModels
        public async Task<IActionResult> Index()
        {
              return _context.LoginViewModel != null ? 
                          View(await _context.LoginViewModel.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.LoginViewModel'  is null.");
        }

        // GET: LoginViewModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.LoginViewModel == null)
            {
                return NotFound();
            }

            var loginViewModel = await _context.LoginViewModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loginViewModel == null)
            {
                return NotFound();
            }

            return View(loginViewModel);
        }

        // GET: LoginViewModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LoginViewModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,Password")] LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loginViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loginViewModel);
        }

        // GET: LoginViewModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.LoginViewModel == null)
            {
                return NotFound();
            }

            var loginViewModel = await _context.LoginViewModel.FindAsync(id);
            if (loginViewModel == null)
            {
                return NotFound();
            }
            return View(loginViewModel);
        }

        // POST: LoginViewModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserName,Password")] LoginViewModel loginViewModel)
        {
            if (id != loginViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loginViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoginViewModelExists(loginViewModel.Id))
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
            return View(loginViewModel);
        }

        // GET: LoginViewModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.LoginViewModel == null)
            {
                return NotFound();
            }

            var loginViewModel = await _context.LoginViewModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loginViewModel == null)
            {
                return NotFound();
            }

            return View(loginViewModel);
        }

        // POST: LoginViewModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.LoginViewModel == null)
            {
                return Problem("Entity set 'ApplicationDbContext.LoginViewModel'  is null.");
            }
            var loginViewModel = await _context.LoginViewModel.FindAsync(id);
            if (loginViewModel != null)
            {
                _context.LoginViewModel.Remove(loginViewModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoginViewModelExists(int id)
        {
          return (_context.LoginViewModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
