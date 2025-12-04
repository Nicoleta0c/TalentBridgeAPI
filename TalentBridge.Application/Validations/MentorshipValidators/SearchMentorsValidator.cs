using FluentValidation;
using TalentBridge.API.DTOs.MentorshipDTOs;

namespace TalentBridge.API.Validations.MentorshipValidators
{
    public class SearchMentorsValidator : AbstractValidator<SearchMentorsDto>
    {
        public SearchMentorsValidator()
        {
            RuleFor(x => x.Category)
                .MaximumLength(100).WithMessage("La categoría no puede exceder 100 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Category));

            RuleFor(x => x.Expertise)
                .Must(expertise => expertise == null || expertise.Count <= 10)
                .WithMessage("No se pueden buscar más de 10 áreas de experiencia")
                .ForEach(expertise => expertise.MaximumLength(50).WithMessage("Cada área no puede exceder 50 caracteres"))
                .When(x => x.Expertise != null);

            RuleFor(x => x.MaxHourlyRate)
                .GreaterThanOrEqualTo(0).WithMessage("La tarifa máxima no puede ser negativa")
                .LessThanOrEqualTo(1000).WithMessage("La tarifa máxima no puede exceder $1000")
                .When(x => x.MaxHourlyRate.HasValue);

            RuleFor(x => x.MinYearsOfExperience)
                .InclusiveBetween(0, 50).WithMessage("Los años mínimos de experiencia deben estar entre 0 y 50")
                .When(x => x.MinYearsOfExperience.HasValue);

            RuleFor(x => x.MinRating)
                .InclusiveBetween(1, 5).WithMessage("La calificación mínima debe estar entre 1 y 5")
                .When(x => x.MinRating.HasValue);

            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("La página debe ser mayor a 0");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("El tamaño de página debe estar entre 1 y 100");
        }
    }
}