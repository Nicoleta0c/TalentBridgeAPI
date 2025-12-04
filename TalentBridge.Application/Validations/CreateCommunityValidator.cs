using FluentValidation;
using TalentBridge.API.DTOs.CommunityDTOs;

namespace TalentBridge.API.Validations
{
    public class CreateCommunityValidator : AbstractValidator<CreateCommunityDto>
    {
        public CreateCommunityValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la comunidad es requerido")
                .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres")
                .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es requerida")
                .MaximumLength(1000).WithMessage("La descripción no puede exceder 1000 caracteres")
                .MinimumLength(10).WithMessage("La descripción debe tener al menos 10 caracteres");

            RuleFor(x => x.ImageUrl)
                .Must(BeAValidUrl).WithMessage("La URL de la imagen no es válida")
                .When(x => !string.IsNullOrEmpty(x.ImageUrl));

            RuleFor(x => x.UniversityId)
                .GreaterThan(0).WithMessage("El ID de la universidad debe ser mayor a 0");

            RuleFor(x => x.Purpose)
                .MaximumLength(500).WithMessage("El propósito no puede exceder 500 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Purpose));

            RuleFor(x => x.Rules)
                .MaximumLength(2000).WithMessage("Las reglas no pueden exceder 2000 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Rules));

            RuleFor(x => x.MaxMembers)
                .GreaterThanOrEqualTo(0).WithMessage("El máximo de miembros debe ser mayor o igual a 0");
        }

        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return true;
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}