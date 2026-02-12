using FluentValidation;
using HakedisYonetimSistemi.DTOs;

namespace HakedisYonetimSistemi.Validators
{
    public class CreateMaliyetKalemiValidator : AbstractValidator<CreateMaliyetKalemiDto>
    {
        public CreateMaliyetKalemiValidator()
        {
            RuleFor(x => x.ProjeId)
                .GreaterThan(0).WithMessage("Geçerli bir proje seçmelisiniz");

            RuleFor(x => x.KalemAdi)
                .NotEmpty().WithMessage("Kalem adı zorunludur")
                .MinimumLength(2).WithMessage("Kalem adı en az 2 karakter olmalıdır")
                .MaximumLength(200).WithMessage("Kalem adı en fazla 200 karakter olmalıdır");

            RuleFor(x => x.Birim)
                .NotEmpty().WithMessage("Birim zorunludur")
                .MaximumLength(50).WithMessage("Birim en fazla 50 karakter olmalıdır");

            RuleFor(x => x.BirimFiyat)
                .NotEmpty().WithMessage("Birim fiyat zorunludur")
                .GreaterThan(0).WithMessage("Birim fiyat 0'dan büyük olmalıdır");

            RuleFor(x => x.ToplamMiktar)
                .NotEmpty().WithMessage("Toplam miktar zorunludur")
                .GreaterThan(0).WithMessage("Toplam miktar 0'dan büyük olmalıdır");

            RuleFor(x => x.Aciklama)
                .MaximumLength(1000).WithMessage("Açıklama en fazla 1000 karakter olmalıdır")
                .When(x => !string.IsNullOrEmpty(x.Aciklama));
        }
    }

    public class UpdateMaliyetKalemiValidator : AbstractValidator<UpdateMaliyetKalemiDto>
    {
        public UpdateMaliyetKalemiValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Geçerli bir kalem seçmelisiniz");

            RuleFor(x => x.KalemAdi)
                .NotEmpty().WithMessage("Kalem adı zorunludur")
                .MinimumLength(2).WithMessage("Kalem adı en az 2 karakter olmalıdır")
                .MaximumLength(200).WithMessage("Kalem adı en fazla 200 karakter olmalıdır");

            RuleFor(x => x.Birim)
                .NotEmpty().WithMessage("Birim zorunludur")
                .MaximumLength(50).WithMessage("Birim en fazla 50 karakter olmalıdır");

            RuleFor(x => x.BirimFiyat)
                .NotEmpty().WithMessage("Birim fiyat zorunludur")
                .GreaterThan(0).WithMessage("Birim fiyat 0'dan büyük olmalıdır");

            RuleFor(x => x.ToplamMiktar)
                .NotEmpty().WithMessage("Toplam miktar zorunludur")
                .GreaterThan(0).WithMessage("Toplam miktar 0'dan büyük olmalıdır");
        }
    }
}
