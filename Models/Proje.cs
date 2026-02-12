using System.ComponentModel.DataAnnotations;

namespace HakedisYonetimSistemi.Models
{
    public class Proje
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Proje adı zorunludur")]
        [Display(Name = "Proje Adı")]
        public string ProjeAdi { get; set; } = string.Empty;
        
        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; }
        
        [Required(ErrorMessage = "Başlangıç tarihi zorunludur")]
        [Display(Name = "Başlangıç Tarihi")]
        [DataType(DataType.Date)]
        public DateTime BaslangicTarihi { get; set; }
        
        [Display(Name = "Bitiş Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? BitisTarihi { get; set; }
        
        [Required(ErrorMessage = "Toplam bütçe zorunludur")]
        [Display(Name = "Toplam Bütçe")]
        [DataType(DataType.Currency)]
        public decimal Butce { get; set; }
        
        [Display(Name = "Müşteri Adı")]
        public string? MusteriAdi { get; set; }
        
        [Display(Name = "Adres")]
        public string? Adres { get; set; }
        
        [Display(Name = "Durum")]
        public ProjeDurum Durum { get; set; } = ProjeDurum.Aktif;
        
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;
        
        // Navigation Properties
        public virtual ICollection<Hakedis> Hakedisler { get; set; } = new List<Hakedis>();
        public virtual ICollection<MaliyetKalemi> MaliyetKalemleri { get; set; } = new List<MaliyetKalemi>();
    }
    
    public enum ProjeDurum
    {
        Aktif,
        Beklemede,
        Tamamlandi,
        Durduruldu,
        Iptal
    }
}