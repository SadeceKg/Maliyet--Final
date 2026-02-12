using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HakedisYonetimSistemi.Data;
using HakedisYonetimSistemi.Models;
using Microsoft.AspNetCore.Authorization;

namespace HakedisYonetimSistemi.Controllers
{
    [Authorize]
    public class HakedisController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HakedisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Hakedis
        public async Task<IActionResult> Index()
        {
            var hakedisler = await _context.Hakedisler
                .Include(h => h.Proje)
                .Include(h => h.HakedisDetaylari)
                .ThenInclude(hd => hd.MaliyetKalemi)
                .ToListAsync();
            return View(hakedisler);
        }

        // GET: Hakedis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hakedis = await _context.Hakedisler
                .Include(h => h.Proje)
                .Include(h => h.HakedisDetaylari)
                .ThenInclude(hd => hd.MaliyetKalemi)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (hakedis == null)
            {
                return NotFound();
            }

            return View(hakedis);
        }

        // GET: Hakedis/Create
        public IActionResult Create(int? projeId)
        {
            ViewBag.Projeler = _context.Projeler.Where(p => p.Durum == ProjeDurum.Aktif).ToList();
            
            var hakedis = new Hakedis { 
                HakedisTarihi = DateTime.Today,
                KdvOrani = 20,
                Durum = HakedisDurum.Hazirlaniyor,
                HakedisDetaylari = new List<HakedisDetay>()
            };
            
            if (projeId.HasValue)
            {
                hakedis.ProjeId = projeId.Value;
            }
            
            return View(hakedis);
        }

        // POST: Hakedis/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hakedis hakedis)
        {
            // Model Binding'den gelen HakedisDetaylari'yı filtrele (boş olanları çıkar)
            if (hakedis.HakedisDetaylari != null)
            {
                hakedis.HakedisDetaylari = hakedis.HakedisDetaylari
                    .Where(hd => !string.IsNullOrWhiteSpace(hd.Aciklama) || hd.Miktar > 0 || hd.BirimFiyat > 0)
                    .ToList();
            }

            if (ModelState.IsValid)
            {
                // Doğrulama
                if (hakedis.HakedisDetaylari == null || hakedis.HakedisDetaylari.Count == 0)
                {
                    ModelState.AddModelError("", "Lütfen en az bir kalem ekleyiniz!");
                    ViewBag.Projeler = _context.Projeler.Where(p => p.Durum == ProjeDurum.Aktif).ToList();
                    return View(hakedis);
                }

                // HakedisTutari (Ara Toplam) hesapla - ToplamTutar computed property olduğu için direkt hesaplayoruz
                decimal araToplam = 0;
                foreach (var detay in hakedis.HakedisDetaylari)
                {
                    // ToplamTutar property'sinden otomatik olarak hesaplanıyor (Miktar * BirimFiyat)
                    araToplam += detay.Miktar * detay.BirimFiyat;
                }
                hakedis.HakedisTutari = araToplam;

                // Sistem tarafından ayarlanacak alanlar
                hakedis.OlusturulmaTarihi = DateTime.Now;
                hakedis.Durum = HakedisDurum.Hazirlaniyor;

                _context.Add(hakedis);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = $"Hakediş '{hakedis.HakedisNo}' başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Details), new { id = hakedis.Id });
            }

            ViewBag.Projeler = _context.Projeler.Where(p => p.Durum == ProjeDurum.Aktif).ToList();
            return View(hakedis);
        }

        // GET: Hakedis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hakedis = await _context.Hakedisler
                .Include(h => h.HakedisDetaylari)
                .FirstOrDefaultAsync(h => h.Id == id);
            
            if (hakedis == null)
            {
                return NotFound();
            }

            // Onaylandı ve Ödendi durumundaki hakediş'ler düzenlenemez
            if (hakedis.Durum == HakedisDurum.Onaylandi || hakedis.Durum == HakedisDurum.Odendi)
            {
                TempData["Error"] = "Onaylanan ve ödenen hakediş'ler düzenlenemez!";
                return RedirectToAction(nameof(Details), new { id });
            }

            ViewBag.Projeler = _context.Projeler.Where(p => p.Durum == ProjeDurum.Aktif).ToList();
            return View(hakedis);
        }

        // POST: Hakedis/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Hakedis hakedis)
        {
            if (id != hakedis.Id)
            {
                return NotFound();
            }

            var existingHakedis = await _context.Hakedisler
                .Include(h => h.HakedisDetaylari)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (existingHakedis == null)
            {
                return NotFound();
            }

            // Onaylandı ve Ödendi durumundaki hakediş'ler düzenlenemez
            if (existingHakedis.Durum == HakedisDurum.Onaylandi || existingHakedis.Durum == HakedisDurum.Odendi)
            {
                TempData["Error"] = "Onaylanan ve ödenen hakediş'ler düzenlenemez!";
                return RedirectToAction(nameof(Details), new { id });
            }

            // Model Binding'den gelen HakedisDetaylari'yı filtrele
            if (hakedis.HakedisDetaylari != null)
            {
                hakedis.HakedisDetaylari = hakedis.HakedisDetaylari
                    .Where(hd => !string.IsNullOrWhiteSpace(hd.Aciklama) || hd.Miktar > 0 || hd.BirimFiyat > 0)
                    .ToList();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Mevcut detayları sil ve yenileriyle değiştir
                    _context.HakedisDetaylari.RemoveRange(existingHakedis.HakedisDetaylari);

                    // HakedisTutari hesapla - ToplamTutar computed property olduğu için direkt hesaplayoruz
                    decimal araToplam = 0;
                    if (hakedis.HakedisDetaylari != null)
                    {
                        foreach (var detay in hakedis.HakedisDetaylari)
                        {
                            // ToplamTutar property'sinden otomatik olarak hesaplanıyor (Miktar * BirimFiyat)
                            araToplam += detay.Miktar * detay.BirimFiyat;
                        }
                    }
                    hakedis.HakedisTutari = araToplam;

                    // Önemli alanları koru
                    hakedis.OlusturulmaTarihi = existingHakedis.OlusturulmaTarihi;
                    hakedis.OnayTarihi = existingHakedis.OnayTarihi;
                    hakedis.OdemeTarihi = existingHakedis.OdemeTarihi;

                    _context.Update(hakedis);
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "Hakediş başarıyla güncellendi.";
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
            }

            ViewBag.Projeler = _context.Projeler.Where(p => p.Durum == ProjeDurum.Aktif).ToList();
            return View(hakedis);
        }

        // GET: Hakedis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hakedis = await _context.Hakedisler
                .Include(h => h.Proje)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hakedis == null)
            {
                return NotFound();
            }

            return View(hakedis);
        }

        // POST: Hakedis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hakedis = await _context.Hakedisler.FindAsync(id);
            if (hakedis != null)
            {
                _context.Hakedisler.Remove(hakedis);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Hakediş başarıyla silindi.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Hakedis/Onayla/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Onayla(int id)
        {
            var hakedis = await _context.Hakedisler.FindAsync(id);
            if (hakedis != null && hakedis.Durum == HakedisDurum.Onayda)
            {
                hakedis.Durum = HakedisDurum.Onaylandi;
                hakedis.OnayTarihi = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Hakediş başarıyla onaylandı.";
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Hakedis/Reddet/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reddet(int id)
        {
            var hakedis = await _context.Hakedisler.FindAsync(id);
            if (hakedis != null && hakedis.Durum == HakedisDurum.Onayda)
            {
                hakedis.Durum = HakedisDurum.Reddedildi;
                await _context.SaveChangesAsync();
                TempData["Warning"] = "Hakediş reddedildi.";
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        private bool HakedisExists(int id)
        {
            return _context.Hakedisler.Any(e => e.Id == id);
        }
    }
}