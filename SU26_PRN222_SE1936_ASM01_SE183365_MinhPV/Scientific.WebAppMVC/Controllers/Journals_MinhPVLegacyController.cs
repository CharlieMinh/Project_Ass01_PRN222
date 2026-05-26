using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Scientific.Entities.Models;
using Scientific.Services;
using Scientific.WebAppMVC.Authorization;

namespace Scientific.WebAppMVC.Controllers
{
    [Authorize(Policy = AppPolicies.AdminOnly)]
    public class Journals_MinhPVLegacyController : Controller
    {
        // private readonly ScientificJournalTrendDBContext _context;
        private readonly IJournalsMinhPvService _journalsMinhPvService;
        public Journals_MinhPVLegacyController(IJournalsMinhPvService journalsMinhPvService)
        {
            _journalsMinhPvService = journalsMinhPvService;
        }

        // GET: JournalsMinhPvs
        public async Task<IActionResult> Index()
        {
            var scientificJournalTrendDBContext = await _journalsMinhPvService.GetAllAsync();
            return View(scientificJournalTrendDBContext);
        }
        /*
        // GET: JournalsMinhPvs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journalsMinhPv = await _context.JournalsMinhPvs
                .Include(j => j.Publisher)
                .FirstOrDefaultAsync(m => m.JournalIdMinhPv == id);
            if (journalsMinhPv == null)
            {
                return NotFound();
            }

            return View(journalsMinhPv);
        }

        // GET: JournalsMinhPvs/Create
        public IActionResult Create()
        {
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PublisherName");
            return View();
        }

        // POST: JournalsMinhPvs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("JournalIdMinhPv,JournalName,Issn,Eissn,PublisherId,Country,WebsiteUrl,ImpactFactor,Ranking,Description,IsActive,CreatedAt")] JournalsMinhPv journalsMinhPv)
        {
            if (ModelState.IsValid)
            {
                _context.Add(journalsMinhPv);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PublisherName", journalsMinhPv.PublisherId);
            return View(journalsMinhPv);
        }

        // GET: JournalsMinhPvs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journalsMinhPv = await _context.JournalsMinhPvs.FindAsync(id);
            if (journalsMinhPv == null)
            {
                return NotFound();
            }
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PublisherName", journalsMinhPv.PublisherId);
            return View(journalsMinhPv);
        }

        // POST: JournalsMinhPvs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("JournalIdMinhPv,JournalName,Issn,Eissn,PublisherId,Country,WebsiteUrl,ImpactFactor,Ranking,Description,IsActive,CreatedAt")] JournalsMinhPv journalsMinhPv)
        {
            if (id != journalsMinhPv.JournalIdMinhPv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(journalsMinhPv);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JournalsMinhPvExists(journalsMinhPv.JournalIdMinhPv))
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
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PublisherName", journalsMinhPv.PublisherId);
            return View(journalsMinhPv);
        }

        // GET: JournalsMinhPvs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journalsMinhPv = await _context.JournalsMinhPvs
                .Include(j => j.Publisher)
                .FirstOrDefaultAsync(m => m.JournalIdMinhPv == id);
            if (journalsMinhPv == null)
            {
                return NotFound();
            }

            return View(journalsMinhPv);
        }

        // POST: JournalsMinhPvs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var journalsMinhPv = await _context.JournalsMinhPvs.FindAsync(id);
            if (journalsMinhPv != null)
            {
                _context.JournalsMinhPvs.Remove(journalsMinhPv);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JournalsMinhPvExists(int id)
        {
            return _context.JournalsMinhPvs.Any(e => e.JournalIdMinhPv == id);
        }
        */
    }


}
