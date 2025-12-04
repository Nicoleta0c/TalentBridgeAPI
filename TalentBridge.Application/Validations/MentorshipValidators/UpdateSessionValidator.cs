using FluentValidation;
using TalentBridge.API.DTOs.MentorshipDTOs;

namespace TalentBridge.API.Validations.MentorshipValidators
{
    public class UpdateSessionValidator : AbstractValidator<UpdateSessionDto>
    {
        public UpdateSessionValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(200).WithMessage("El título no puede exceder 200 caracteres")
                .MinimumLength(5).WithMessage("El título debe tener al menos 5 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Title));

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("La descripción no puede exceder 1000 caracteres")
                .MinimumLength(10).WithMessage("La descripción debe tener al menos 10 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.ScheduledDate)
                .Must(scheduledDate => scheduledDate >= DateTime.UtcNow.Date)
                .WithMessage("La fecha programada no puede ser en el pasado")
                .When(x => x.ScheduledDate.HasValue);

            RuleFor(x => x.StartTime)
                .GreaterThan(DateTime.UtcNow).WithMessage("La hora de inicio debe ser en el futuro")
                .When(x => x.StartTime.HasValue);

            RuleFor(x => x.EndTime)
                .GreaterThan(x => x.StartTime).WithMessage("La hora de fin debe ser posterior a la hora de inicio")
                .When(x => x.StartTime.HasValue && x.EndTime.HasValue)
                .Must((dto, endTime) =>
                {
                    if (!dto.StartTime.HasValue || !endTime.HasValue) return true;
                    var duration = endTime.Value - dto.StartTime.Value;
                    return duration.TotalHours <= 4;
                })
                .WithMessage("La sesión no puede durar más de 4 horas")
                .When(x => x.StartTime.HasValue && x.EndTime.HasValue);

            RuleFor(x => x.MeetingMethod)
                .IsInEnum().WithMessage("Método de reunión no válido")
                .When(x => x.MeetingMethod.HasValue);

            RuleFor(x => x.MeetingLink)
                .Must(BeAValidUrl).WithMessage("El enlace de reunión debe ser una URL válida")
                .When(x => !string.IsNullOrEmpty(x.MeetingLink));

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Estado no válido")
                .When(x => x.Status.HasValue);

            RuleFor(x => x.Agenda)
                .MaximumLength(2000).WithMessage("La agenda no puede exceder 2000 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Agenda));

            RuleFor(x => x.Notes)
                .MaximumLength(2000).WithMessage("Las notas no pueden exceder 2000 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Notes));

            RuleFor(x => x.Homework)
                .MaximumLength(1000).WithMessage("La tarea no puede exceder 1000 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Homework));
        }

        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return true;
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}