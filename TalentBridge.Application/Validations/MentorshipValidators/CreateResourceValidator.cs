using FluentValidation;
using TalentBridge.API.DTOs.MentorshipDTOs;

namespace TalentBridge.API.Validations.MentorshipValidators
{
    public class CreateResourceValidator : AbstractValidator<CreateResourceDto>
    {
        public CreateResourceValidator()
        {
            RuleFor(x => x.MentorshipId)
                .GreaterThan(0).WithMessage("El ID de la mentoría debe ser mayor a 0");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("El título es requerido")
                .MaximumLength(200).WithMessage("El título no puede exceder 200 caracteres")
                .MinimumLength(5).WithMessage("El título debe tener al menos 5 caracteres");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("La descripción no puede exceder 1000 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Tipo de recurso no válido");

            RuleFor(x => x.Url)
                .NotEmpty().WithMessage("La URL es requerida")
                .MaximumLength(500).WithMessage("La URL no puede exceder 500 caracteres")
                .Must(BeAValidUrl).WithMessage("La URL debe ser válida");

            RuleFor(x => x.ThumbnailUrl)
                .MaximumLength(500).WithMessage("La URL del thumbnail no puede exceder 500 caracteres")
                .Must(BeAValidUrl).WithMessage("La URL del thumbnail debe ser válida")
                .When(x => !string.IsNullOrEmpty(x.ThumbnailUrl));
        }

        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return true;
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}