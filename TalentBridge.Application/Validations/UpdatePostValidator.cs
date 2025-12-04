using FluentValidation;
using TalentBridge.API.DTOs.CommunityDTOs;

namespace TalentBridge.API.Validations
{
    public class UpdatePostValidator : AbstractValidator<UpdatePostDto>
    {
        public UpdatePostValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(300).WithMessage("El título no puede exceder 300 caracteres")
                .MinimumLength(5).WithMessage("El título debe tener al menos 5 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Title));

            RuleFor(x => x.Content)
                .MaximumLength(10000).WithMessage("El contenido no puede exceder 10000 caracteres")
                .MinimumLength(10).WithMessage("El contenido debe tener al menos 10 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Content));

            RuleFor(x => x.ImageUrl)
                .Must(BeAValidUrl).WithMessage("La URL de la imagen no es válida")
                .When(x => x.ImageUrl != null);

            RuleFor(x => x.AttachmentUrl)
                .Must(BeAValidUrl).WithMessage("La URL del archivo adjunto no es válida")
                .When(x => x.AttachmentUrl != null);
        }

        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return true;
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}