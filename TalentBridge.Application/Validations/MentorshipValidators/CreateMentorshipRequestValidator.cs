using FluentValidation;
using TalentBridge.API.DTOs.MentorshipDTOs;

namespace TalentBridge.API.Validations.MentorshipValidators
{
    public class CreateMentorshipRequestValidator : AbstractValidator<CreateMentorshipRequestDto>
    {
        public CreateMentorshipRequestValidator()
        {
            RuleFor(x => x.MentorId)
                .GreaterThan(0).WithMessage("El ID del mentor debe ser mayor a 0")
                .When(x => x.MentorId.HasValue);

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("El título es requerido")
                .MaximumLength(200).WithMessage("El título no puede exceder 200 caracteres")
                .MinimumLength(5).WithMessage("El título debe tener al menos 5 caracteres");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es requerida")
                .MaximumLength(2000).WithMessage("La descripción no puede exceder 2000 caracteres")
                .MinimumLength(20).WithMessage("La descripción debe tener al menos 20 caracteres");

            RuleFor(x => x.Goals)
                .NotEmpty().WithMessage("Las metas son requeridas")
                .MaximumLength(1000).WithMessage("Las metas no pueden exceder 1000 caracteres")
                .MinimumLength(10).WithMessage("Las metas deben tener al menos 10 caracteres");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("La categoría es requerida")
                .MaximumLength(100).WithMessage("La categoría no puede exceder 100 caracteres");

            RuleFor(x => x.Tags)
                .Must(tags => tags == null || tags.Count <= 10).WithMessage("No se pueden agregar más de 10 tags")
                .ForEach(tag => tag.MaximumLength(50).WithMessage("Cada tag no puede exceder 50 caracteres"));

            RuleFor(x => x.PreferredDurationWeeks)
                .InclusiveBetween(1, 52).WithMessage("La duración preferida debe estar entre 1 y 52 semanas");

            RuleFor(x => x.PreferredSessionsPerWeek)
                .InclusiveBetween(1, 7).WithMessage("Las sesiones por semana deben estar entre 1 y 7");

            RuleFor(x => x.PreferredMeetingMethod)
                .IsInEnum().WithMessage("Método de reunión preferido no válido");
        }
    }
}