using FluentValidation;
using TalentBridge.API.DTOs.MentorshipDTOs;

namespace TalentBridge.API.Validations.MentorshipValidators
{
    public class UpdateMentorshipValidator : AbstractValidator<UpdateMentorshipDto>
    {
        public UpdateMentorshipValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(200).WithMessage("El título no puede exceder 200 caracteres")
                .MinimumLength(5).WithMessage("El título debe tener al menos 5 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Title));

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("La descripción no puede exceder 2000 caracteres")
                .MinimumLength(20).WithMessage("La descripción debe tener al menos 20 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.DurationWeeks)
                .InclusiveBetween(1, 52).WithMessage("La duración debe estar entre 1 y 52 semanas")
                .When(x => x.DurationWeeks.HasValue);

            RuleFor(x => x.SessionsPerWeek)
                .InclusiveBetween(1, 7).WithMessage("Las sesiones por semana deben estar entre 1 y 7")
                .When(x => x.SessionsPerWeek.HasValue);

            RuleFor(x => x.SessionDurationMinutes)
                .InclusiveBetween(15, 240).WithMessage("La duración de la sesión debe estar entre 15 y 240 minutos")
                .When(x => x.SessionDurationMinutes.HasValue);

            RuleFor(x => x.MeetingMethod)
                .IsInEnum().WithMessage("Método de reunión no válido")
                .When(x => x.MeetingMethod.HasValue);

            RuleFor(x => x.MeetingLink)
                .Must(BeAValidUrl).WithMessage("El enlace de reunión debe ser una URL válida")
                .When(x => !string.IsNullOrEmpty(x.MeetingLink));

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Estado no válido")
                .When(x => x.Status.HasValue);
        }

        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return true;
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}