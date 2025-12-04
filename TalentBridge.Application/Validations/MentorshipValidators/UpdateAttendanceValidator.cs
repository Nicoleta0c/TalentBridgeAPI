using FluentValidation;
using TalentBridge.API.DTOs.MentorshipDTOs;

namespace TalentBridge.API.Validations.MentorshipValidators
{
    public class UpdateAttendanceValidator : AbstractValidator<UpdateAttendanceDto>
    {
        public UpdateAttendanceValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Estado de asistencia no válido");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("La calificación debe estar entre 1 y 5")
                .When(x => x.Rating.HasValue);

            RuleFor(x => x.Feedback)
                .MaximumLength(1000).WithMessage("El feedback no puede exceder 1000 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Feedback));
        }
    }
}