using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HakedisYonetimSistemi.Data;
using HakedisYonetimSistemi.Models;
using Microsoft.AspNetCore.Authorization;

namespace HakedisYonetimSistemi.Controllers
{
    [Authorize]
    public class MaliyetKalemiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MaliyetKalemiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MaliyetKalemi
        public async Task<IActionResult> Index(int? projeId)
        {
            var maliyetKalemleri = _context.MaliyetKalemleri.Include(m => m.Proje).AsQueryable();
            
            if (projeId.HasValue)
            {
                maliyetKalemleri = maliyetKalemleri.Where(m => m.ProjeId == projeId.Value);
                ViewBag.ProjeId = projeId.Value;
                ViewBag.ProjeAdi = await _context.Projeler
                    .Where(p => p.Id == projeId.Value)
                    .Select(p => p.ProjeAdi)
                    .FirstOrDefaultAsync();
            }
            
            ViewData["Projeler"] = new SelectList(_context.Projeler, "Id", "ProjeAdi", projeId);
            
            return View(await maliyetKalemleri.ToListAsync());
        }

        // GET: MaliyetKalemi/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maliyetKalemi = await _context.MaliyetKalemleri
                .Include(m => m.Proje)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (maliyetKalemi == null)
            {
                return NotFound();
            }

            return View(maliyetKalemi);
        }

        // GET: MaliyetKalemi/Create
        public IActionResult Create(int? projeId)
        {
            ViewData["ProjeId"] = new SelectList(_context.Projeler.Where(p => p.Durum == ProjeDurum.Aktif), "Id", "ProjeAdi", projeId);
            
            var maliyetKalemi = new MaliyetKalemi();
            if (projeId.HasValue)
            {
                maliyetKalemi.ProjeId = projeId.Value;
            }
            
            return View(maliyetKalemi);
        }

        // POST: MaliyetKalemi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjeId,KalemAdi,Aciklama,Birim,BirimFiyat,ToplamMiktar,Kategori,Aktif")] MaliyetKalemi maliyetKalemi)
        {
            // Debug için ModelState hatalarını logla
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Key: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
            }
            
            if (ModelState.IsValid)
            {
                maliyetKalemi.OlusturulmaTarihi = DateTime.Now;
                _context.Add(maliyetKalemi);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Maliyet kalemi başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index), new { projeId = maliyetKalemi.ProjeId });
            }
            ViewData["ProjeId"] = new SelectList(_context.Projeler.Where(p => p.Durum == ProjeDurum.Aktif), "Id", "ProjeAdi", maliyetKalemi.ProjeId);
            return View(maliyetKalemi);
        }

        // GET: MaliyetKalemi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maliyetKalemi = await _context.MaliyetKalemleri.FindAsync(id);
            if (maliyetKalemi == null)
            {
                return NotFound();
            }
            ViewData["ProjeId"] = new SelectList(_context.Projeler, "Id", "ProjeAdi", maliyetKalemi.ProjeId);
            return View(maliyetKalemi);
        }

        // POST: MaliyetKalemi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjeId,KalemAdi,Aciklama,Birim,BirimFiyat,ToplamMiktar,Kategori,Aktif,OlusturulmaTarihi")] MaliyetKalemi maliyetKalemi)
        {
            if (id != maliyetKalemi.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(maliyetKalemi);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Maliyet kalemi başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaliyetKalemiExists(maliyetKalemi.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { projeId = maliyetKalemi.ProjeId });
            }
            ViewData["ProjeId"] = new SelectList(_context.Projeler, "Id", "ProjeAdi", maliyetKalemi.ProjeId);
            return View(maliyetKalemi);
        }

        // GET: MaliyetKalemi/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maliyetKalemi = await _context.MaliyetKalemleri
                .Include(m => m.Proje)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (maliyetKalemi == null)
            {
                return NotFound();
            }

            return View(maliyetKalemi);
        }

        // POST: MaliyetKalemi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var maliyetKalemi = await _context.MaliyetKalemleri.FindAsync(id);
            if (maliyetKalemi != null)
            {
                var projeId = maliyetKalemi.ProjeId;
                _context.MaliyetKalemleri.Remove(maliyetKalemi);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Maliyet kalemi başarıyla silindi.";
                return RedirectToAction(nameof(Index), new { projeId });
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MaliyetKalemiExists(int id)
        {
            return _context.MaliyetKalemleri.Any(e => e.Id == id);
        }
    }
}