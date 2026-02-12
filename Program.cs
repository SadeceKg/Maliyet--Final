using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HakedisYonetimSistemi.Data;
using HakedisYonetimSistemi.Models;
using HakedisYonetimSistemi.Services;
using HakedisYonetimSistemi.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=hakedis.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => 
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// AutoMapper Registration
builder.Services.AddAutoMapper(typeof(MappingProfile));

// FluentValidation Registration
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProjeValidator>();

// Financial Service Registration
builder.Services.AddScoped<IFinancialService, FinancialService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // Veritabanı ve migration'ları oluştur
    await db.Database.MigrateAsync();
    await SeedDatabaseAsync(db);
}

app.Run();

// Seed Data Method
static async Task SeedDatabaseAsync(ApplicationDbContext db)
{
    try
    {
        // Var olan Hakediş'leri sil
        var existingHakedisler = db.Hakedisler.ToList();
        if (existingHakedisler.Any())
        {
            db.Hakedisler.RemoveRange(existingHakedisler);
            await db.SaveChangesAsync();
        }

        // Projeler var mı kontrol et
        var projeler = db.Projeler.Where(p => p.Durum == HakedisYonetimSistemi.Models.ProjeDurum.Aktif).ToList();
        if (!projeler.Any())
        {
            // Example Proje oluştur
            var proje = new HakedisYonetimSistemi.Models.Proje
            {
                ProjeAdi = "Ofis Binası Renovasyonu",
                MusteriAdi = "ABC İnşaat Ltd.",
                Butce = 500000,
                BaslangicTarihi = new DateTime(2025, 11, 01),
                BitisTarihi = new DateTime(2026, 06, 30),
                Durum = HakedisYonetimSistemi.Models.ProjeDurum.Aktif,
                OlusturulmaTarihi = DateTime.Now
            };
            db.Projeler.Add(proje);
            await db.SaveChangesAsync();
            projeler.Add(proje);
        }

        var proje1 = projeler.FirstOrDefault();
        if (proje1 == null) return;

        // MaliyetKalemi'leri oluştur (referans için)
        var existingKalemler = db.MaliyetKalemleri.Where(m => m.ProjeId == proje1.Id).ToList();
        if (!existingKalemler.Any())
        {
            var kalemler = new List<HakedisYonetimSistemi.Models.MaliyetKalemi>
            {
                new HakedisYonetimSistemi.Models.MaliyetKalemi
                {
                    ProjeId = proje1.Id,
                    KalemAdi = "İnşaat Malzeme ve İşçilik",
                    Aciklama = "Genel inşaat malzeme ve işçilik",
                    Birim = "Lump Sum",
                    BirimFiyat = 1000,
                    ToplamMiktar = 1,
                    Kategori = HakedisYonetimSistemi.Models.MaliyetKategori.Malzeme,
                    Aktif = true,
                    OlusturulmaTarihi = DateTime.Now
                }
            };
            db.MaliyetKalemleri.AddRange(kalemler);
            await db.SaveChangesAsync();
        }

        var maliyetKalemiId = db.MaliyetKalemleri.Where(m => m.ProjeId == proje1.Id).First().Id;

        // 3 Example Hakediş oluştur
        var hakedisler = new List<HakedisYonetimSistemi.Models.Hakedis>
        {
            new HakedisYonetimSistemi.Models.Hakedis
            {
                HakedisNo = "2026-001",
                HakedisTarihi = new DateTime(2025, 12, 15),
                DonemBaslangic = new DateTime(2025, 11, 01),
                DonemBitis = new DateTime(2025, 11, 30),
                KdvOrani = 20,
                ProjeId = proje1.Id,
                Durum = HakedisYonetimSistemi.Models.HakedisDurum.Hazirlaniyor,
                OlusturulmaTarihi = new DateTime(2025, 12, 15, 09, 30, 0),
                Aciklama = "Kasım ayı işçilik ve malzeme bedeli",
                HakedisDetaylari = new List<HakedisYonetimSistemi.Models.HakedisDetay>
                {
                    new HakedisYonetimSistemi.Models.HakedisDetay
                    {
                        Aciklama = "Dış cephe onarımı",
                        Miktar = 150,
                        BirimFiyat = 850,
                        MaliyetKalemiId = maliyetKalemiId
                    },
                    new HakedisYonetimSistemi.Models.HakedisDetay
                    {
                        Aciklama = "İç boyama işi",
                        Miktar = 200,
                        BirimFiyat = 450,
                        MaliyetKalemiId = maliyetKalemiId
                    }
                }
            },
            new HakedisYonetimSistemi.Models.Hakedis
            {
                HakedisNo = "2026-002",
                HakedisTarihi = new DateTime(2026, 01, 20),
                DonemBaslangic = new DateTime(2025, 12, 01),
                DonemBitis = new DateTime(2025, 12, 31),
                KdvOrani = 20,
                ProjeId = proje1.Id,
                Durum = HakedisYonetimSistemi.Models.HakedisDurum.Onayda,
                OlusturulmaTarihi = new DateTime(2026, 01, 20, 14, 15, 0),
                Aciklama = "Aralık ayı kısmi ödeme",
                HakedisDetaylari = new List<HakedisYonetimSistemi.Models.HakedisDetay>
                {
                    new HakedisYonetimSistemi.Models.HakedisDetay
                    {
                        Aciklama = "Elektrik tesisatı kurulumu",
                        Miktar = 80,
                        BirimFiyat = 1200,
                        MaliyetKalemiId = maliyetKalemiId
                    },
                    new HakedisYonetimSistemi.Models.HakedisDetay
                    {
                        Aciklama = "Su tesisatı kurulumu",
                        Miktar = 60,
                        BirimFiyat = 950,
                        MaliyetKalemiId = maliyetKalemiId
                    },
                    new HakedisYonetimSistemi.Models.HakedisDetay
                    {
                        Aciklama = "Kapı ve pencere takılması",
                        Miktar = 35,
                        BirimFiyat = 1500,
                        MaliyetKalemiId = maliyetKalemiId
                    }
                }
            },
            new HakedisYonetimSistemi.Models.Hakedis
            {
                HakedisNo = "2026-003",
                HakedisTarihi = new DateTime(2026, 01, 25),
                DonemBaslangic = new DateTime(2026, 01, 01),
                DonemBitis = new DateTime(2026, 01, 31),
                KdvOrani = 20,
                ProjeId = proje1.Id,
                Durum = HakedisYonetimSistemi.Models.HakedisDurum.Onaylandi,
                OlusturulmaTarihi = new DateTime(2026, 01, 25, 11, 45, 0),
                OnayTarihi = new DateTime(2026, 02, 01, 10, 00, 0),
                Aciklama = "Ocak ayı tamamlama işi",
                HakedisDetaylari = new List<HakedisYonetimSistemi.Models.HakedisDetay>
                {
                    new HakedisYonetimSistemi.Models.HakedisDetay
                    {
                        Aciklama = "Çatı onarımı ve izolasyon",
                        Miktar = 120,
                        BirimFiyat = 1800,
                        MaliyetKalemiId = maliyetKalemiId
                    },
                    new HakedisYonetimSistemi.Models.HakedisDetay
                    {
                        Aciklama = "Son dönem denetim ve ayarlar",
                        Miktar = 40,
                        BirimFiyat = 800,
                        MaliyetKalemiId = maliyetKalemiId
                    }
                }
            }
        };

        // Hesapla HakedisTutari
        foreach (var hakedis in hakedisler)
        {
            hakedis.HakedisTutari = hakedis.HakedisDetaylari.Sum(hd => hd.Miktar * hd.BirimFiyat);
        }

        db.Hakedisler.AddRange(hakedisler);
        await db.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Seed Data Error: {ex.Message}");
    }
}
