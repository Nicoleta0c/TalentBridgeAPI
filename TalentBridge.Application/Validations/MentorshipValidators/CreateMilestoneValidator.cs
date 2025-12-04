using FluentValidation;
using TalentBridge.API.DTOs.MentorshipDTOs;

namespace TalentBridge.API.Validations.MentorshipValidators
{
    public class CreateMilestoneValidator : AbstractValidator<CreateMilestoneDto>
    {
        public CreateMilestoneValidator()
        {
            RuleFor(x => x.MentorshipId)
                .GreaterThan(0).WithMessage("El ID de mentoría es requerido");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("El título es requerido")
                .MaximumLength(200).WithMessage("El título no puede exceder 200 caracteres")
                .MinimumLength(5).WithMessage("El título debe tener al menos 5 caracteres");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es requerida")
                .MaximumLength(1000).WithMessage("La descripción no puede exceder 1000 caracteres")
                .MinimumLength(10).WithMessage("La descripción debe tener al menos 10 caracteres");

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("La fecha límite es requerida")
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("La fecha límite no puede ser en el pasado");
        }
    }
}