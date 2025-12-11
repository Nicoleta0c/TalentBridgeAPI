using FluentValidation;
using TalentBridge.Application.DTOs;

namespace TalentBridge.Application.Validations
{
    public class AnalyzeCVRequestValidator : AbstractValidator<AnalyzeCVRequestDto>
    {
        public AnalyzeCVRequestValidator()
        {
            RuleFor(x => x.CVId)
                .GreaterThan(0).WithMessage("Valid CV ID is required");

            RuleFor(x => x.JobId)
                .GreaterThan(0).WithMessage("Valid Job ID is required");

            RuleFor(x => x.JobDescription)
                .MaximumLength(5000).WithMessage("Job description cannot exceed 5000 characters")
                .When(x => !string.IsNullOrEmpty(x.JobDescription));
        }
    }
}