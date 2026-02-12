namespace HakedisYonetimSistemi.DTOs
{
    public class ProjeDto
    {
        public int Id { get; set; }
        public string ProjeAdi { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public decimal Butce { get; set; }
        public string? MusteriAdi { get; set; }
        public string? Adres { get; set; }
        public int Durum { get; set; }
        public DateTime OlusturulmaTarihi { get; set; }

        // Hesaplanan alanlar
        public decimal ToplamMaliyet { get; set; }
        public decimal KalanButce { get; set; }
        public decimal YuzdeHarcanan { get; set; }
    }

    public class CreateProjeDto
    {
        public string ProjeAdi { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public decimal Butce { get; set; }
        public string? MusteriAdi { get; set; }
        public string? Adres { get; set; }
    }

    public class UpdateProjeDto
    {
        public int Id { get; set; }
        public string ProjeAdi { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public decimal Butce { get; set; }
        public string? MusteriAdi { get; set; }
        public string? Adres { get; set; }
    }
}
