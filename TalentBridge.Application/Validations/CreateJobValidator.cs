using FluentValidation;
using TalentBridge.Application.DTOs;
using TalentBridge.Application.DTOs.JobDTOs;

namespace TalentBridge.Application.Validations
{
    public class CreateJobValidator : AbstractValidator<CreateJobDto>
    {
        public CreateJobValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MinimumLength(3).WithMessage("Title must be at least 3 characters")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

            RuleFor(x => x.Company)
                .NotEmpty().WithMessage("Company is required")
                .MinimumLength(2).WithMessage("Company must be at least 2 characters")
                .MaximumLength(100).WithMessage("Company cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MinimumLength(10).WithMessage("Description must be at least 10 characters")
                .MaximumLength(5000).WithMessage("Description cannot exceed 5000 characters");

            RuleFor(x => x.Requirements)
                .NotEmpty().WithMessage("Requirements are required")
                .MinimumLength(10).WithMessage("Requirements must be at least 10 characters")
                .MaximumLength(2000).WithMessage("Requirements cannot exceed 2000 characters");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required")
                .MaximumLength(100).WithMessage("Location cannot exceed 100 characters");

            RuleFor(x => x.SalaryRange)
                .NotEmpty().WithMessage("Salary range is required")
                .MaximumLength(50).WithMessage("Salary range cannot exceed 50 characters");

            RuleFor(x => x.JobType)
                .NotEmpty().WithMessage("Job type is required")
                .Must(BeValidJobType).WithMessage("Invalid job type. Valid types: Full-time, Part-time, Contract, Internship, Remote");
        }

        private bool BeValidJobType(string jobType)
        {
            var validTypes = new[] { "Full-time", "Part-time", "Contract", "Internship", "Remote" };
            return validTypes.Contains(jobType);
        }
    }
}