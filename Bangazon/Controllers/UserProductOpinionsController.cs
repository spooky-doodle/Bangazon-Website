using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bangazon.Data;
using Bangazon.Models;

namespace Bangazon
{
    public class UserProductOpinionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserProductOpinionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserProductOpinions
        public async Task<IActionResult> Index()
        {
            return View(await _context.UserProductOpinions.ToListAsync());
        }

        // GET: UserProductOpinions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProductOpinion = await _context.UserProductOpinions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userProductOpinion == null)
            {
                return NotFound();
            }

            return View(userProductOpinion);
        }

        // GET: UserProductOpinions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserProductOpinions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Opinion,UserId,ProductId")] UserProductOpinion userProductOpinion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userProductOpinion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userProductOpinion);
        }

        // GET: UserProductOpinions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProductOpinion = await _context.UserProductOpinions.FindAsync(id);
            if (userProductOpinion == null)
            {
                return NotFound();
            }
            return View(userProductOpinion);
        }

        // POST: UserProductOpinions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Opinion,UserId,ProductId")] UserProductOpinion userProductOpinion)
        {
            if (id != userProductOpinion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userProductOpinion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserProductOpinionExists(userProductOpinion.Id))
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
            return View(userProductOpinion);
        }

        // GET: UserProductOpinions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProductOpinion = await _context.UserProductOpinions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userProductOpinion == null)
            {
                return NotFound();
            }

            return View(userProductOpinion);
        }

        // POST: UserProductOpinions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userProductOpinion = await _context.UserProductOpinions.FindAsync(id);
            _context.UserProductOpinions.Remove(userProductOpinion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserProductOpinionExists(int id)
        {
            return _context.UserProductOpinions.Any(e => e.Id == id);
        }
    }
}
