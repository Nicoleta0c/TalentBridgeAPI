using FluentValidation;
using TalentBridge.API.DTOs.CommunityDTOs;
using TalentBridge.Application.DTOs.CommentsDTOs;

namespace TalentBridge.API.Validations
{
    public class CreateCommentValidator : AbstractValidator<CreateCommentDto>
    {
        public CreateCommentValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("El contenido del comentario es requerido")
                .MaximumLength(5000).WithMessage("El contenido no puede exceder 5000 caracteres")
                .MinimumLength(1).WithMessage("El contenido debe tener al menos 1 carácter");

            RuleFor(x => x.PostId)
                .GreaterThan(0).WithMessage("El ID del post debe ser mayor a 0");

            RuleFor(x => x.ParentCommentId)
                .GreaterThan(0).WithMessage("El ID del comentario padre debe ser mayor a 0")
                .When(x => x.ParentCommentId.HasValue);
        }
    }
}