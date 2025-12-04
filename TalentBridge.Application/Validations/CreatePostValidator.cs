using FluentValidation;
using TalentBridge.API.DTOs.CommunityDTOs;

namespace TalentBridge.API.Validations
{
    public class CreatePostValidator : AbstractValidator<CreatePostDto>
    {
        public CreatePostValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("El título es requerido")
                .MaximumLength(300).WithMessage("El título no puede exceder 300 caracteres")
                .MinimumLength(5).WithMessage("El título debe tener al menos 5 caracteres");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("El contenido es requerido")
                .MaximumLength(10000).WithMessage("El contenido no puede exceder 10000 caracteres")
                .MinimumLength(10).WithMessage("El contenido debe tener al menos 10 caracteres");

            RuleFor(x => x.CommunityId)
                .GreaterThan(0).WithMessage("El ID de la comunidad debe ser mayor a 0");

            RuleFor(x => x.ImageUrl)
                .Must(BeAValidUrl).WithMessage("La URL de la imagen no es válida")
                .When(x => !string.IsNullOrEmpty(x.ImageUrl));

            RuleFor(x => x.AttachmentUrl)
                .Must(BeAValidUrl).WithMessage("La URL del archivo adjunto no es válida")
                .When(x => !string.IsNullOrEmpty(x.AttachmentUrl));

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("El tipo de post es requerido")
                .Must(BeAValidPostType).WithMessage("Tipo de post no válido. Valores permitidos: Discussion, Question, Announcement, JobOffer, Event, Resource");
        }

        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return true;
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private bool BeAValidPostType(string type)
        {
            var validTypes = new[] { "Discussion", "Question", "Announcement", "JobOffer", "Event", "Resource" };
            return validTypes.Contains(type, StringComparer.OrdinalIgnoreCase);
        }
    }
}