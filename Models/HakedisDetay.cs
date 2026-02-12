using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HakedisYonetimSistemi.Models
{
    public class HakedisDetay
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Hakediş")]
        public int HakedisId { get; set; }
        
        [Required]
        [Display(Name = "Maliyet Kalemi")]
        public int MaliyetKalemiId { get; set; }
        
        [Required(ErrorMessage = "Miktar zorunludur")]
        [Display(Name = "Miktar")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Miktar 0'dan büyük olmalıdır")]
        [Column(TypeName = "decimal(18,3)")]
        public decimal Miktar { get; set; }
        
        [Required(ErrorMessage = "Birim fiyat zorunludur")]
        [Display(Name = "Birim Fiyat")]
        [DataType(DataType.Currency)]
        [Range(0.01, double.MaxValue, ErrorMessage = "Birim fiyat 0'dan büyük olmalıdır")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BirimFiyat { get; set; }
        
        [Display(Name = "Toplam Tutar")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ToplamTutar => Miktar * BirimFiyat;
        
        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; }
        
        // Navigation Properties
        [ForeignKey("HakedisId")]
        public virtual Hakedis Hakedis { get; set; } = null!;
        
        [ForeignKey("MaliyetKalemiId")]
        public virtual MaliyetKalemi MaliyetKalemi { get; set; } = null!;
    }
}