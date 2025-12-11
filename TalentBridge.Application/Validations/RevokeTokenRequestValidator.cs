using FluentValidation;
using TalentBridge.Application.DTOs.UserDTOs;

namespace TalentBridge.Application.Validations
{
    public class RevokeTokenRequestValidator : AbstractValidator<RevokeTokenRequest>
    {
        public RevokeTokenRequestValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required")
                .MinimumLength(20).WithMessage("Invalid refresh token format");
        }
    }
}