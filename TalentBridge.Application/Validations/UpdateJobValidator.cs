using FluentValidation;
using TalentBridge.Application.DTOs.JobDTOs;

namespace TalentBridge.Application.Validations
{
    public class UpdateJobValidator : AbstractValidator<UpdateJobDto>
    {
        public UpdateJobValidator()
        {
            // Estas reglas son opcionales (solo aplican si se proporciona el valor)
            RuleFor(x => x.Title)
                .MinimumLength(3).WithMessage("Title must be at least 3 characters")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.Title));

            RuleFor(x => x.Company)
                .MinimumLength(2).WithMessage("Company must be at least 2 characters")
                .MaximumLength(100).WithMessage("Company cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Company));

            RuleFor(x => x.Description)
                .MinimumLength(10).WithMessage("Description must be at least 10 characters")
                .MaximumLength(5000).WithMessage("Description cannot exceed 5000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Requirements)
                .MinimumLength(10).WithMessage("Requirements must be at least 10 characters")
                .MaximumLength(2000).WithMessage("Requirements cannot exceed 2000 characters")
                .When(x => !string.IsNullOrEmpty(x.Requirements));

            RuleFor(x => x.Location)
                .MaximumLength(100).WithMessage("Location cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Location));

            RuleFor(x => x.SalaryRange)
                .MaximumLength(50).WithMessage("Salary range cannot exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.SalaryRange));

            RuleFor(x => x.JobType)
                .Must(BeValidJobType).WithMessage("Invalid job type. Valid types: Full-time, Part-time, Contract, Internship, Remote")
                .When(x => !string.IsNullOrEmpty(x.JobType));
        }

        private bool BeValidJobType(string jobType)
        {
            var validTypes = new[] { "Full-time", "Part-time", "Contract", "Internship", "Remote" };
            return validTypes.Contains(jobType);
        }
    }
}