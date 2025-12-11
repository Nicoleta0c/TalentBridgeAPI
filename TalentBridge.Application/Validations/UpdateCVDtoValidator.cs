using FluentValidation;
using TalentBridge.Application.DTOs.CVDTOs;

namespace TalentBridge.Application.Validations
{
    public class UpdateCVDtoValidator : AbstractValidator<UpdateCVDto>
    {
        public UpdateCVDtoValidator()
        {
            RuleFor(x => x.FileName)
                .MaximumLength(255).WithMessage("File name cannot exceed 255 characters")
                .When(x => !string.IsNullOrEmpty(x.FileName));

            RuleFor(x => x.AnalysisResult)
                .MaximumLength(4000).WithMessage("Analysis result cannot exceed 4000 characters")
                .When(x => !string.IsNullOrEmpty(x.AnalysisResult));

            RuleFor(x => x.Score)
                .InclusiveBetween(0, 100).WithMessage("Score must be between 0 and 100");

            RuleFor(x => x.Skills)
                .MaximumLength(1000).WithMessage("Skills cannot exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Skills));

            RuleFor(x => x.Experience)
                .MaximumLength(2000).WithMessage("Experience cannot exceed 2000 characters")
                .When(x => !string.IsNullOrEmpty(x.Experience));

            RuleFor(x => x.Education)
                .MaximumLength(1000).WithMessage("Education cannot exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Education));

            RuleFor(x => x.Certifications)
                .MaximumLength(1000).WithMessage("Certifications cannot exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Certifications));
        }
    }
}