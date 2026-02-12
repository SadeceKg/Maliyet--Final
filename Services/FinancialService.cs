namespace HakedisYonetimSistemi.Services
{
    public interface IFinancialService
    {
        /// <summary>
        /// Temel maliyete KDV, işçilik ve kar marjı ekleyerek önerilen satış fiyatını hesaplar
        /// </summary>
        decimal HesaplaÖnerilenenSatisFiyati(decimal baseMaliyet, decimal kdvYuzdesi = 20, 
            decimal iscilikYuzdesi = 10, decimal karMarjıYuzdesi = 30);

        /// <summary>
        /// KDV ve diğer masraflar eklenerek toplam tutarı hesaplar
        /// </summary>
        decimal HesaplaToplam(decimal baseTutar, decimal kdvYuzdesi = 20);

        /// <summary>
        /// Kar marjını ve yüzdesini hesaplar
        /// </summary>
        (decimal KarMiktarı, decimal KarYuzdesi) HesaplaKarMarji(decimal maliyet, decimal satisFiyati);

        /// <summary>
        /// Proje bütçesinin durumunu analiz eder
        /// </summary>
        (decimal ToplamMaliyet, decimal KalanButce, decimal YuzdeHarcanan, string Durum) AnalizProjeButcesi(
            decimal toplamButce, decimal toplamMaliyet);
    }

    public class FinancialService : IFinancialService
    {
        public decimal HesaplaÖnerilenenSatisFiyati(decimal baseMaliyet, decimal kdvYuzdesi = 20, 
            decimal iscilikYuzdesi = 10, decimal karMarjıYuzdesi = 30)
        {
            if (baseMaliyet <= 0)
                throw new ArgumentException("Maliyet 0'dan büyük olmalıdır", nameof(baseMaliyet));

            // Temel maliyet + işçilik
            var maliyetVeIscilik = baseMaliyet + (baseMaliyet * iscilikYuzdesi / 100);

            // Kar marjı ekle
            var satisUcretiMaliyetiyle = maliyetVeIscilik + (maliyetVeIscilik * karMarjıYuzdesi / 100);

            // KDV ekle
            var sonucKDVle = satisUcretiMaliyetiyle + (satisUcretiMaliyetiyle * kdvYuzdesi / 100);

            return Math.Round(sonucKDVle, 2);
        }

        public decimal HesaplaToplam(decimal baseTutar, decimal kdvYuzdesi = 20)
        {
            if (baseTutar < 0)
                throw new ArgumentException("Tutar 0'dan az olamaz", nameof(baseTutar));

            var kdvTutari = baseTutar * kdvYuzdesi / 100;
            return Math.Round(baseTutar + kdvTutari, 2);
        }

        public (decimal KarMiktarı, decimal KarYuzdesi) HesaplaKarMarji(decimal maliyet, decimal satisFiyati)
        {
            if (maliyet < 0 || satisFiyati < 0)
                throw new ArgumentException("Maliyet ve satış fiyatı 0'dan az olamaz");

            if (maliyet == 0)
                return (KarMiktarı: 0, KarYuzdesi: 0);

            var karMiktarı = satisFiyati - maliyet;
            var karYuzdesi = (karMiktarı / maliyet) * 100;

            return (KarMiktarı: Math.Round(karMiktarı, 2), KarYuzdesi: Math.Round(karYuzdesi, 2));
        }

        public (decimal ToplamMaliyet, decimal KalanButce, decimal YuzdeHarcanan, string Durum) AnalizProjeButcesi(
            decimal toplamButce, decimal toplamMaliyet)
        {
            if (toplamButce < 0 || toplamMaliyet < 0)
                throw new ArgumentException("Bütçe ve maliyet 0'dan az olamaz");

            var kalanButce = toplamButce - toplamMaliyet;
            var yuzdeHarcanan = toplamButce > 0 ? (toplamMaliyet / toplamButce) * 100 : 0;

            var durum = "";
            if (yuzdeHarcanan <= 50)
                durum = "✅ İyi - Bütçenin %50'si altında";
            else if (yuzdeHarcanan <= 75)
                durum = "⚠️ Dikkat - Bütçenin %50-75'i kullanıldı";
            else if (yuzdeHarcanan <= 90)
                durum = "🔴 Kritik - Bütçenin %75-90'ı kullanıldı";
            else if (yuzdeHarcanan < 100)
                durum = "🔴 Çok Kritik - Bütçeyi aşmak üzere";
            else
                durum = "❌ UYARI - Bütçe aşıldı!";

            return (
                ToplamMaliyet: Math.Round(toplamMaliyet, 2),
                KalanButce: Math.Round(kalanButce, 2),
                YuzdeHarcanan: Math.Round(yuzdeHarcanan, 2),
                Durum: durum
            );
        }
    }
}
