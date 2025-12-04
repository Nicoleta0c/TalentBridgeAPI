using FluentValidation;
using TalentBridge.API.DTOs.CommunityDTOs;

namespace TalentBridge.API.Validations
{
    public class UpdateCommunityValidator : AbstractValidator<UpdateCommunityDto>
    {
        public UpdateCommunityValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres")
                .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Name));

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("La descripción no puede exceder 1000 caracteres")
                .MinimumLength(10).WithMessage("La descripción debe tener al menos 10 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.ImageUrl)
                .Must(BeAValidUrl).WithMessage("La URL de la imagen no es válida")
                .When(x => !string.IsNullOrEmpty(x.ImageUrl));

            RuleFor(x => x.Purpose)
                .MaximumLength(500).WithMessage("El propósito no puede exceder 500 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Purpose));

            RuleFor(x => x.Rules)
                .MaximumLength(2000).WithMessage("Las reglas no pueden exceder 2000 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Rules));

            RuleFor(x => x.MaxMembers)
                .GreaterThanOrEqualTo(0).WithMessage("El máximo de miembros debe ser mayor o igual a 0")
                .When(x => x.MaxMembers.HasValue);
        }

        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return true;
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}