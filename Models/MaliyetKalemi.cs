using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HakedisYonetimSistemi.Models
{
    public class MaliyetKalemi
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Proje")]
        public int ProjeId { get; set; }
        
        [Required(ErrorMessage = "Kalem adı zorunludur")]
        [Display(Name = "Kalem Adı")]
        public string KalemAdi { get; set; } = string.Empty;
        
        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; }
        
        [Required(ErrorMessage = "Birim zorunludur")]
        [Display(Name = "Birim")]
        public string Birim { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Birim fiyat zorunludur")]
        [Display(Name = "Birim Fiyat")]
        [DataType(DataType.Currency)]
        [Range(0.01, double.MaxValue, ErrorMessage = "Birim fiyat 0'dan büyük olmalıdır")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BirimFiyat { get; set; }
        
        [Required(ErrorMessage = "Toplam miktar zorunludur")]
        [Display(Name = "Toplam Miktar")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Toplam miktar 0'dan büyük olmalıdır")]
        [Column(TypeName = "decimal(18,3)")]
        public decimal ToplamMiktar { get; set; }
        
        [Display(Name = "Toplam Tutar")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ToplamTutar => ToplamMiktar * BirimFiyat;
        
        [Display(Name = "Kategori")]
        public MaliyetKategori Kategori { get; set; } = MaliyetKategori.Malzeme;
        
        [Display(Name = "Aktif")]
        public bool Aktif { get; set; } = true;
        
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;
        
        // Navigation Properties
        [ForeignKey("ProjeId")]
        public virtual Proje Proje { get; set; } = null!;
        public virtual ICollection<HakedisDetay> HakedisDetaylari { get; set; } = new List<HakedisDetay>();
    }
    
    public enum MaliyetKategori
    {
        Malzeme,
        Iscilik,
        Makine,
        Nakliye,
        Diger
    }
}