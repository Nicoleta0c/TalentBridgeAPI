using FluentValidation;
using TalentBridge.API.DTOs.CommunityDTOs;
using TalentBridge.Application.DTOs.CommentsDTOs;

namespace TalentBridge.API.Validations
{
    public class UpdateCommentValidator : AbstractValidator<UpdateCommentDto>
    {
        public UpdateCommentValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("El contenido del comentario es requerido")
                .MaximumLength(5000).WithMessage("El contenido no puede exceder 5000 caracteres")
                .MinimumLength(1).WithMessage("El contenido debe tener al menos 1 carácter");
        }
    }
}