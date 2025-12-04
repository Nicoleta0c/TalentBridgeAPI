using FluentValidation;

namespace TalentBridge.API.Validations
{
    public class CreateUniversityValidator : AbstractValidator<CreateUniversityDto>
    {
        public CreateUniversityValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la universidad es requerido")
                .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres")
                .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres");

            RuleFor(x => x.Acronym)
                .NotEmpty().WithMessage("El acrónimo es requerido")
                .MaximumLength(20).WithMessage("El acrónimo no puede exceder 20 caracteres")
                .MinimumLength(2).WithMessage("El acrónimo debe tener al menos 2 caracteres")
                .Matches("^[A-Z]+$").WithMessage("El acrónimo debe contener solo letras mayúsculas");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("La descripción no puede exceder 1000 caracteres");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("El email no es válido")
                .MaximumLength(100).WithMessage("El email no puede exceder 100 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Website)
                .Must(BeAValidUrl).WithMessage("La URL del sitio web no es válida")
                .When(x => !string.IsNullOrEmpty(x.Website));

            RuleFor(x => x.Phone)
                .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("El teléfono no es válido")
                .MinimumLength(10).WithMessage("El teléfono debe tener al menos 10 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("La ciudad es requerida")
                .MaximumLength(100).WithMessage("La ciudad no puede exceder 100 caracteres");

            RuleFor(x => x.Province)
                .NotEmpty().WithMessage("La provincia es requerida")
                .MaximumLength(100).WithMessage("La provincia no puede exceder 100 caracteres");

            RuleFor(x => x.ContactPersonEmail)
                .EmailAddress().WithMessage("El email de contacto no es válido")
                .When(x => !string.IsNullOrEmpty(x.ContactPersonEmail));
        }

        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return true;
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}