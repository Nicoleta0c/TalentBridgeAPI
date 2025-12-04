using FluentValidation;
using TalentBridge.API.DTOs.MentorshipDTOs;

namespace TalentBridge.API.Validations.MentorshipValidators
{
    public class CreateSessionValidator : AbstractValidator<CreateSessionDto>
    {
        public CreateSessionValidator()
        {
            RuleFor(x => x.MentorshipId)
                .GreaterThan(0).WithMessage("El ID de la mentoría debe ser mayor a 0");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("El título es requerido")
                .MaximumLength(200).WithMessage("El título no puede exceder 200 caracteres")
                .MinimumLength(5).WithMessage("El título debe tener al menos 5 caracteres");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es requerida")
                .MaximumLength(1000).WithMessage("La descripción no puede exceder 1000 caracteres")
                .MinimumLength(10).WithMessage("La descripción debe tener al menos 10 caracteres");

            RuleFor(x => x.ScheduledDate)
                .NotEmpty().WithMessage("La fecha programada es requerida")
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("La fecha programada no puede ser en el pasado");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("La hora de inicio es requerida")
                .GreaterThan(DateTime.UtcNow).WithMessage("La hora de inicio debe ser en el futuro")
                .Must((dto, startTime) => startTime.Date >= dto.ScheduledDate.Date)
                .WithMessage("La hora de inicio debe ser igual o posterior a la fecha programada");

            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("La hora de fin es requerida")
                .GreaterThan(x => x.StartTime).WithMessage("La hora de fin debe ser posterior a la hora de inicio")
                .Must((dto, endTime) => (endTime - dto.StartTime).TotalHours <= 4)
                .WithMessage("La sesión no puede durar más de 4 horas");

            RuleFor(x => x.MeetingMethod)
                .IsInEnum().WithMessage("Método de reunión no válido");

            RuleFor(x => x.MeetingLink)
                .Must(BeAValidUrl).WithMessage("El enlace de reunión debe ser una URL válida")
                .When(x => !string.IsNullOrEmpty(x.MeetingLink));

            RuleFor(x => x.Agenda)
                .MaximumLength(2000).WithMessage("La agenda no puede exceder 2000 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Agenda));

            RuleFor(x => x.ReminderHoursBefore)
                .InclusiveBetween(1, 168).WithMessage("El recordatorio debe estar entre 1 y 168 horas (1 semana) antes")
                .When(x => x.SendReminder);
        }

        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return true;
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}