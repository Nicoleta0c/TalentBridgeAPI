using FluentValidation;
using TalentBridge.Application.Interfaces;
using TalentBridge.Application.Interfaces.IUser;
using TalentBridge.Domain.Entities;

namespace TalentBridge.Application.Validations
{
    public class UserEmailValidator : AbstractValidator<User>
    {
        private readonly IUserRepository _userRepository;

        public UserEmailValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters")
                .MustAsync(BeUniqueEmail).WithMessage("Email already exists")
                .When(x => !string.IsNullOrEmpty(x.Email));
        }

        private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByEmailAsync(email);
            return existingUser == null;
        }
    }
}