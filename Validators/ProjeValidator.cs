using FluentValidation;
using HakedisYonetimSistemi.DTOs;

namespace HakedisYonetimSistemi.Validators
{
    public class CreateProjeValidator : AbstractValidator<CreateProjeDto>
    {
        public CreateProjeValidator()
        {
            RuleFor(x => x.ProjeAdi)
                .NotEmpty().WithMessage("Proje adı zorunludur")
                .MinimumLength(3).WithMessage("Proje adı en az 3 karakter olmalıdır")
                .MaximumLength(200).WithMessage("Proje adı en fazla 200 karakter olmalıdır");

            RuleFor(x => x.BaslangicTarihi)
                .NotEmpty().WithMessage("Başlangıç tarihi zorunludur")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Başlangıç tarihi bugünden ileride olamaz");

            RuleFor(x => x.BitisTarihi)
                .GreaterThanOrEqualTo(x => x.BaslangicTarihi)
                .When(x => x.BitisTarihi.HasValue)
                .WithMessage("Bitiş tarihi başlangıç tarihinden sonra olmalıdır");

            RuleFor(x => x.Butce)
                .NotEmpty().WithMessage("Bütçe zorunludur")
                .GreaterThan(0).WithMessage("Bütçe 0'dan büyük olmalıdır");

            RuleFor(x => x.MusteriAdi)
                .MaximumLength(200).WithMessage("Müşteri adı en fazla 200 karakter olmalıdır")
                .When(x => !string.IsNullOrEmpty(x.MusteriAdi));

            RuleFor(x => x.Aciklama)
                .MaximumLength(1000).WithMessage("Açıklama en fazla 1000 karakter olmalıdır")
                .When(x => !string.IsNullOrEmpty(x.Aciklama));
        }
    }

    public class UpdateProjeValidator : AbstractValidator<UpdateProjeDto>
    {
        public UpdateProjeValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Geçerli bir proje seçmelisiniz");

            RuleFor(x => x.ProjeAdi)
                .NotEmpty().WithMessage("Proje adı zorunludur")
                .MinimumLength(3).WithMessage("Proje adı en az 3 karakter olmalıdır")
                .MaximumLength(200).WithMessage("Proje adı en fazla 200 karakter olmalıdır");

            RuleFor(x => x.BaslangicTarihi)
                .NotEmpty().WithMessage("Başlangıç tarihi zorunludur");

            RuleFor(x => x.BitisTarihi)
                .GreaterThanOrEqualTo(x => x.BaslangicTarihi)
                .When(x => x.BitisTarihi.HasValue)
                .WithMessage("Bitiş tarihi başlangıç tarihinden sonra olmalıdır");

            RuleFor(x => x.Butce)
                .NotEmpty().WithMessage("Bütçe zorunludur")
                .GreaterThan(0).WithMessage("Bütçe 0'dan büyük olmalıdır");
        }
    }
}
