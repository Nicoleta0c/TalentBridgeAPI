using FluentValidation;
using TalentBridge.Application.DTOs.UserDTOs;

namespace TalentBridge.Application.Validations
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .MinimumLength(2).WithMessage("Full name must be at least 2 characters")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Full name can only contain letters and spaces");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches(@"[A-Z]+").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[a-z]+").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"[0-9]+").WithMessage("Password must contain at least one number")
                .Matches(@"[\!\?\*\.]+").WithMessage("Password must contain at least one special character (!?*.)")
                .MaximumLength(50).WithMessage("Password cannot exceed 50 characters");
        }
    }
}