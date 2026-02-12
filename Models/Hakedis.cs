using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HakedisYonetimSistemi.Models
{
    public class Hakedis
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Proje")]
        public int ProjeId { get; set; }
        
        [Required(ErrorMessage = "Hakediş numarası zorunludur")]
        [Display(Name = "Hakediş No")]
        public string HakedisNo { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Hakediş tarihi zorunludur")]
        [Display(Name = "Hakediş Tarihi")]
        [DataType(DataType.Date)]
        public DateTime HakedisTarihi { get; set; }
        
        [Display(Name = "Dönem Başlangıç")]
        [DataType(DataType.Date)]
        public DateTime DonemBaslangic { get; set; }
        
        [Display(Name = "Dönem Bitiş")]
        [DataType(DataType.Date)]
        public DateTime DonemBitis { get; set; }
        
        [Required(ErrorMessage = "Hakediş tutarı zorunludur")]
        [Display(Name = "Hakediş Tutarı")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal HakedisTutari { get; set; }
        
        [Display(Name = "KDV Oranı (%)")]
        [Range(0, 100)]
        public decimal KdvOrani { get; set; } = 20;
        
        [Display(Name = "KDV Tutarı")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal KdvTutari => HakedisTutari * KdvOrani / 100;
        
        [Display(Name = "Toplam Tutar")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ToplamTutar => HakedisTutari + KdvTutari;
        
        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; }
        
        [Display(Name = "Durum")]
        public HakedisDurum Durum { get; set; } = HakedisDurum.Hazirlaniyor;
        
        [Display(Name = "Onay Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? OnayTarihi { get; set; }
        
        [Display(Name = "Ödeme Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? OdemeTarihi { get; set; }
        
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;
        
        // Navigation Properties
        [ForeignKey("ProjeId")]
        public virtual Proje Proje { get; set; } = null!;
        public virtual ICollection<HakedisDetay> HakedisDetaylari { get; set; } = new List<HakedisDetay>();
    }
    
    public enum HakedisDurum
    {
        Hazirlaniyor,
        Onayda,
        Onaylandi,
        Odendi,
        Reddedildi
    }
}