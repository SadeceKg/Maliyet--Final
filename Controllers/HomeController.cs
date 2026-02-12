using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using HakedisYonetimSistemi.Models;
using HakedisYonetimSistemi.Data;

namespace HakedisYonetimSistemi.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            // Dashboard verileri
            var toplamProjeSayisi = await _context.Projeler.CountAsync();
            var aktifProjeSayisi = await _context.Projeler.CountAsync(p => p.Durum == ProjeDurum.Aktif);
            var toplamHakedisSayisi = await _context.Hakedisler.CountAsync();
            var bekleyenHakedisSayisi = await _context.Hakedisler.CountAsync(h => h.Durum == HakedisDurum.Onayda);
            
            var onaylananHakedisler = await _context.Hakedisler
                .Where(h => h.Durum == HakedisDurum.Onaylandi || h.Durum == HakedisDurum.Odendi)
                .ToListAsync();
            
            var toplamHakedisTutari = onaylananHakedisler.Sum(h => h.ToplamTutar);

            var sonHakedisler = await _context.Hakedisler
                .Include(h => h.Proje)
                .OrderByDescending(h => h.OlusturulmaTarihi)
                .Take(5)
                .ToListAsync();

            ViewBag.ToplamProjeSayisi = toplamProjeSayisi;
            ViewBag.AktifProjeSayisi = aktifProjeSayisi;
            ViewBag.ToplamHakedisSayisi = toplamHakedisSayisi;
            ViewBag.BekleyenHakedisSayisi = bekleyenHakedisSayisi;
            ViewBag.ToplamHakedisTutari = toplamHakedisTutari;
            ViewBag.SonHakedisler = sonHakedisler;
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [Authorize]
    public IActionResult Dashboard()
    {
        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
