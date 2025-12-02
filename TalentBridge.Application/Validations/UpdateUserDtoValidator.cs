using FluentValidation;
using TalentBridge.Application.DTOs.UserDTOs;

namespace TalentBridge.Application.Validations
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            // Estas reglas son opcionales (solo aplican si se proporciona el valor)
            RuleFor(x => x.FullName)
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.FullName));

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
                .Matches(@"^[+\d\s\-()]+$").WithMessage("Invalid phone number format")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.Bio)
                .MaximumLength(1000).WithMessage("Bio cannot exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Bio));

            RuleFor(x => x.Skills)
                .MaximumLength(500).WithMessage("Skills cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Skills));
        }
    }
}