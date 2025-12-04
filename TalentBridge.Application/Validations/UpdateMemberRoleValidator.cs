using FluentValidation;
using TalentBridge.API.DTOs.CommunityDTOs;

namespace TalentBridge.API.Validations
{
    public class UpdateMemberRoleValidator : AbstractValidator<UpdateMemberRoleDto>
    {
        public UpdateMemberRoleValidator()
        {
            RuleFor(x => x.MemberId)
                .GreaterThan(0).WithMessage("El ID del miembro debe ser mayor a 0");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("El rol es requerido")
                .Must(BeAValidRole).WithMessage("Rol no válido. Valores permitidos: Member, Moderator, Admin");
        }

        private bool BeAValidRole(string role)
        {
            var validRoles = new[] { "Member", "Moderator", "Admin" };
            return validRoles.Contains(role, StringComparer.OrdinalIgnoreCase);
        }
    }
}