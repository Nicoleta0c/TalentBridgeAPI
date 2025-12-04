using FluentValidation;
using TalentBridge.API.DTOs.MentorshipDTOs;

namespace TalentBridge.API.Validations.MentorshipValidators
{
    public class CreateMentorshipValidator : AbstractValidator<CreateMentorshipDto>
    {
        public CreateMentorshipValidator()
        {
            RuleFor(x => x.MentorId)
                .GreaterThan(0).WithMessage("El ID del mentor debe ser mayor a 0");

            RuleFor(x => x.MenteeId)
                .GreaterThan(0).WithMessage("El ID del mentee debe ser mayor a 0")
                .NotEqual(x => x.MentorId).WithMessage("El mentor y el mentee no pueden ser la misma persona");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("El título es requerido")
                .MaximumLength(200).WithMessage("El título no puede exceder 200 caracteres")
                .MinimumLength(5).WithMessage("El título debe tener al menos 5 caracteres");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es requerida")
                .MaximumLength(2000).WithMessage("La descripción no puede exceder 2000 caracteres")
                .MinimumLength(20).WithMessage("La descripción debe tener al menos 20 caracteres");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("La categoría es requerida")
                .MaximumLength(100).WithMessage("La categoría no puede exceder 100 caracteres");

            RuleFor(x => x.Tags)
                .Must(tags => tags == null || tags.Count <= 10).WithMessage("No se pueden agregar más de 10 tags")
                .ForEach(tag => tag.MaximumLength(50).WithMessage("Cada tag no puede exceder 50 caracteres"));

            RuleFor(x => x.DurationWeeks)
                .InclusiveBetween(1, 52).WithMessage("La duración debe estar entre 1 y 52 semanas");

            RuleFor(x => x.SessionsPerWeek)
                .InclusiveBetween(1, 7).WithMessage("Las sesiones por semana deben estar entre 1 y 7");

            RuleFor(x => x.SessionDurationMinutes)
                .InclusiveBetween(15, 240).WithMessage("La duración de la sesión debe estar entre 15 y 240 minutos");

            RuleFor(x => x.MeetingMethod)
                .IsInEnum().WithMessage("Método de reunión no válido");

            RuleFor(x => x.MeetingLink)
                .Must(BeAValidUrl).WithMessage("El enlace de reunión debe ser una URL válida")
                .When(x => !string.IsNullOrEmpty(x.MeetingLink));
        }

        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return true;
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}