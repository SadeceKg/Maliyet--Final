using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HakedisYonetimSistemi.Models;

namespace HakedisYonetimSistemi.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Proje> Projeler { get; set; }
        public DbSet<Hakedis> Hakedisler { get; set; }
        public DbSet<HakedisDetay> HakedisDetaylari { get; set; }
        public DbSet<MaliyetKalemi> MaliyetKalemleri { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Proje konfigürasyonu
            modelBuilder.Entity<Proje>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProjeAdi).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Butce).HasColumnType("decimal(18,2)");
                entity.Property(e => e.MusteriAdi).HasMaxLength(200);
                entity.Property(e => e.Aciklama).HasMaxLength(1000);
            });

            // Hakediş konfigürasyonu
            modelBuilder.Entity<Hakedis>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.HakedisNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.HakedisTutari).HasColumnType("decimal(18,2)");
                entity.Property(e => e.KdvOrani).HasColumnType("decimal(5,2)");
                entity.Property(e => e.Aciklama).HasMaxLength(1000);
                
                entity.HasOne(e => e.Proje)
                    .WithMany(p => p.Hakedisler)
                    .HasForeignKey(e => e.ProjeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Maliyet Kalemi konfigürasyonu
            modelBuilder.Entity<MaliyetKalemi>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.KalemAdi).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Birim).IsRequired().HasMaxLength(50);
                entity.Property(e => e.BirimFiyat).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ToplamMiktar).HasColumnType("decimal(18,3)");
                entity.Property(e => e.Aciklama).HasMaxLength(1000);
                
                entity.HasOne(e => e.Proje)
                    .WithMany(p => p.MaliyetKalemleri)
                    .HasForeignKey(e => e.ProjeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Hakediş Detay konfigürasyonu
            modelBuilder.Entity<HakedisDetay>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Miktar).HasColumnType("decimal(18,3)");
                entity.Property(e => e.BirimFiyat).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Aciklama).HasMaxLength(500);
                
                entity.HasOne(e => e.Hakedis)
                    .WithMany(h => h.HakedisDetaylari)
                    .HasForeignKey(e => e.HakedisId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.MaliyetKalemi)
                    .WithMany(m => m.HakedisDetaylari)
                    .HasForeignKey(e => e.MaliyetKalemiId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}