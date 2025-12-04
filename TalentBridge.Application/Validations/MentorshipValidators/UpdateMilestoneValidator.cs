using FluentValidation;
using TalentBridge.API.DTOs.MentorshipDTOs;

namespace TalentBridge.API.Validations.MentorshipValidators
{
    public class UpdateMilestoneValidator : AbstractValidator<UpdateMilestoneDto>
    {
        public UpdateMilestoneValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(200).WithMessage("El título no puede exceder 200 caracteres")
                .MinimumLength(5).WithMessage("El título debe tener al menos 5 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Title));

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("La descripción no puede exceder 1000 caracteres")
                .MinimumLength(10).WithMessage("La descripción debe tener al menos 10 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.DueDate)
                .Must(dueDate => dueDate >= DateTime.UtcNow.Date)
                .WithMessage("La fecha límite no puede ser en el pasado")
                .When(x => x.DueDate.HasValue);

            RuleFor(x => x.ProgressPercentage)
                .InclusiveBetween(0, 100).WithMessage("El progreso debe estar entre 0 y 100%")
                .When(x => x.ProgressPercentage.HasValue);

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Estado no válido")
                .When(x => x.Status.HasValue);

            // Validaciones adicionales opcionales:
            RuleFor(x => x.CompletedDate)
                .GreaterThanOrEqualTo(x => x.DueDate)
                .WithMessage("La fecha de completado no puede ser anterior a la fecha límite")
                .When(x => x.CompletedDate.HasValue && x.DueDate.HasValue);

            RuleFor(x => x.CompletedDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("La fecha de completado no puede ser en el futuro")
                .When(x => x.CompletedDate.HasValue);

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Las notas no pueden exceder 1000 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Notes));

            RuleFor(x => x.Evidence)
                .MaximumLength(500).WithMessage("La evidencia no puede exceder 500 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Evidence));
        }
    }
}