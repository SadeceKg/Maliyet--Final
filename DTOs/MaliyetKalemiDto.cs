namespace HakedisYonetimSistemi.DTOs
{
    public class MaliyetKalemiDto
    {
        public int Id { get; set; }
        public int ProjeId { get; set; }
        public string KalemAdi { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public string Birim { get; set; } = string.Empty;
        public decimal BirimFiyat { get; set; }
        public decimal ToplamMiktar { get; set; }
        public decimal ToplamTutar { get; set; }
        public int Kategori { get; set; }
        public bool Aktif { get; set; }
        public DateTime OlusturulmaTarihi { get; set; }
    }

    public class CreateMaliyetKalemiDto
    {
        public int ProjeId { get; set; }
        public string KalemAdi { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public string Birim { get; set; } = string.Empty;
        public decimal BirimFiyat { get; set; }
        public decimal ToplamMiktar { get; set; }
        public int Kategori { get; set; }
    }

    public class UpdateMaliyetKalemiDto
    {
        public int Id { get; set; }
        public string KalemAdi { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public string Birim { get; set; } = string.Empty;
        public decimal BirimFiyat { get; set; }
        public decimal ToplamMiktar { get; set; }
        public int Kategori { get; set; }
        public bool Aktif { get; set; }
    }
}
