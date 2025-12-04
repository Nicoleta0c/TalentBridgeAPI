using FluentValidation;
using TalentBridge.API.DTOs.MentorshipDTOs;

namespace TalentBridge.API.Validations.MentorshipValidators
{
    public class UpdateMentorProfileValidator : AbstractValidator<UpdateMentorProfileDto>
    {
        public UpdateMentorProfileValidator()
        {
            RuleFor(x => x.MentorTitle)
                .MaximumLength(100).WithMessage("El título no puede exceder 100 caracteres")
                .When(x => !string.IsNullOrEmpty(x.MentorTitle));

            RuleFor(x => x.MentorBio)
                .MaximumLength(2000).WithMessage("La biografía no puede exceder 2000 caracteres")
                .When(x => !string.IsNullOrEmpty(x.MentorBio));

            RuleFor(x => x.YearsOfExperience)
                .InclusiveBetween(0, 50).WithMessage("Los años de experiencia deben estar entre 0 y 50")
                .When(x => x.YearsOfExperience.HasValue);

            RuleFor(x => x.MentorExpertise)
                .Must(expertise => expertise == null || expertise.Count <= 15)
                .WithMessage("No se pueden agregar más de 15 áreas de experiencia")
                .ForEach(expertise => expertise.MaximumLength(50).WithMessage("Cada área no puede exceder 50 caracteres"))
                .When(x => x.MentorExpertise != null);

            RuleFor(x => x.MentorIndustries)
                .Must(industries => industries == null || industries.Count <= 10)
                .WithMessage("No se pueden agregar más de 10 industrias")
                .ForEach(industry => industry.MaximumLength(50).WithMessage("Cada industria no puede exceder 50 caracteres"))
                .When(x => x.MentorIndustries != null);

            RuleFor(x => x.HourlyRate)
                .GreaterThanOrEqualTo(0).WithMessage("La tarifa por hora no puede ser negativa")
                .LessThanOrEqualTo(1000).WithMessage("La tarifa por hora no puede exceder $1000")
                .When(x => x.HourlyRate.HasValue);

            RuleFor(x => x.MaxMentees)
                .InclusiveBetween(1, 20).WithMessage("El máximo de mentees debe estar entre 1 y 20")
                .When(x => x.MaxMentees.HasValue);

            RuleFor(x => x.PreferredMentorshipCategories)
                .Must(categories => categories == null || categories.Count <= 10)
                .WithMessage("No se pueden agregar más de 10 categorías preferidas")
                .ForEach(category => category.MaximumLength(50).WithMessage("Cada categoría no puede exceder 50 caracteres"))
                .When(x => x.PreferredMentorshipCategories != null);

            RuleFor(x => x.PreferredMeetingMethod)
                .IsInEnum().WithMessage("Método de reunión preferido no válido")
                .When(x => x.PreferredMeetingMethod.HasValue);

            RuleFor(x => x.Timezone)
                .MaximumLength(50).WithMessage("La zona horaria no puede exceder 50 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Timezone));
        }
    }
}