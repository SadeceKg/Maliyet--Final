using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HakedisYonetimSistemi.Data;
using HakedisYonetimSistemi.Models;
using Microsoft.AspNetCore.Authorization;

namespace HakedisYonetimSistemi.Controllers
{
    [Authorize]
    public class ProjeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Proje
        public async Task<IActionResult> Index()
        {
            var projeler = await _context.Projeler
                .Include(p => p.Hakedisler)
                .Include(p => p.MaliyetKalemleri)
                .ToListAsync();
            return View(projeler);
        }

        // GET: Proje/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proje = await _context.Projeler
                .Include(p => p.Hakedisler)
                .Include(p => p.MaliyetKalemleri)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (proje == null)
            {
                return NotFound();
            }

            return View(proje);
        }

        // GET: Proje/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Proje/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjeAdi,Aciklama,BaslangicTarihi,BitisTarihi,Butce,MusteriAdi,Adres,Durum")] Proje proje)
        {
            if (ModelState.IsValid)
            {
                proje.OlusturulmaTarihi = DateTime.Now;
                _context.Add(proje);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Proje başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            return View(proje);
        }

        // GET: Proje/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proje = await _context.Projeler.FindAsync(id);
            if (proje == null)
            {
                return NotFound();
            }
            return View(proje);
        }

        // POST: Proje/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjeAdi,Aciklama,BaslangicTarihi,BitisTarihi,Butce,MusteriAdi,Adres,Durum,OlusturulmaTarihi")] Proje proje)
        {
            if (id != proje.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(proje);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Proje başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjeExists(proje.Id))
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
            return View(proje);
        }

        // GET: Proje/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proje = await _context.Projeler
                .FirstOrDefaultAsync(m => m.Id == id);
            if (proje == null)
            {
                return NotFound();
            }

            return View(proje);
        }

        // POST: Proje/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var proje = await _context.Projeler.FindAsync(id);
            if (proje != null)
            {
                _context.Projeler.Remove(proje);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Proje başarıyla silindi.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProjeExists(int id)
        {
            return _context.Projeler.Any(e => e.Id == id);
        }

        // GET: Proje/MaliyetHesaplama/5
        public async Task<IActionResult> MaliyetHesaplama(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proje = await _context.Projeler
                .Include(p => p.MaliyetKalemleri)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (proje == null)
            {
                return NotFound();
            }

            return View(proje);
        }
    }
}