using FluentValidation;
using TalentBridge.API.DTOs.MentorshipDTOs;

namespace TalentBridge.API.Validations.MentorshipValidators
{
    public class CreateMentorApplicationValidator : AbstractValidator<CreateMentorApplicationDto>
    {
        public CreateMentorApplicationValidator()
        {
            RuleFor(x => x.RequestId)
                .GreaterThan(0).WithMessage("El ID de la solicitud debe ser mayor a 0");

            RuleFor(x => x.Proposal)
                .NotEmpty().WithMessage("La propuesta es requerida")
                .MaximumLength(2000).WithMessage("La propuesta no puede exceder 2000 caracteres")
                .MinimumLength(50).WithMessage("La propuesta debe tener al menos 50 caracteres");

            RuleFor(x => x.Experience)
                .MaximumLength(2000).WithMessage("La experiencia no puede exceder 2000 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Experience));

            RuleFor(x => x.WhyChooseMe)
                .MaximumLength(2000).WithMessage("La razón para elegirte no puede exceder 2000 caracteres")
                .When(x => !string.IsNullOrEmpty(x.WhyChooseMe));

            RuleFor(x => x.ProposedDurationWeeks)
                .InclusiveBetween(1, 52).WithMessage("La duración propuesta debe estar entre 1 y 52 semanas")
                .When(x => x.ProposedDurationWeeks.HasValue);

            RuleFor(x => x.ProposedSessionsPerWeek)
                .InclusiveBetween(1, 7).WithMessage("Las sesiones por semana deben estar entre 1 y 7")
                .When(x => x.ProposedSessionsPerWeek.HasValue);

            RuleFor(x => x.ProposedMeetingMethod)
                .IsInEnum().WithMessage("Método de reunión propuesto no válido")
                .When(x => x.ProposedMeetingMethod.HasValue);
        }
    }
}