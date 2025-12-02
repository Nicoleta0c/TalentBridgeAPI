using FluentValidation;
using TalentBridge.Application.DTOs.UserDTOs;

namespace TalentBridge.Application.Validations
{
    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequestDto>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required")
                .MinimumLength(20).WithMessage("Invalid refresh token format");
        }
    }
}