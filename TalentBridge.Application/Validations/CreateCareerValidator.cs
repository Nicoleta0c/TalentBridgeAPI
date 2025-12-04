using FluentValidation;

namespace TalentBridge.API.Validations
{
    public class CreateCareerValidator : AbstractValidator<CreateCareerDto>
    {
        public CreateCareerValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la carrera es requerido")
                .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres")
                .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código de la carrera es requerido")
                .MaximumLength(50).WithMessage("El código no puede exceder 50 caracteres")
                .Matches("^[A-Z0-9-]+$").WithMessage("El código debe contener solo letras mayúsculas, números y guiones");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("La descripción no puede exceder 1000 caracteres");

            RuleFor(x => x.AreaOfStudy)
                .NotEmpty().WithMessage("El área de estudio es requerida")
                .MaximumLength(100).WithMessage("El área de estudio no puede exceder 100 caracteres");

            RuleFor(x => x.DurationYears)
                .GreaterThan(0).WithMessage("La duración debe ser mayor a 0")
                .LessThanOrEqualTo(10).WithMessage("La duración no puede exceder 10 años");

            RuleFor(x => x.DegreeType)
                .NotEmpty().WithMessage("El tipo de grado es requerido")
                .Must(BeAValidDegreeType).WithMessage("Tipo de grado no válido. Valores permitidos: Técnico, Tecnólogo, Licenciatura, Ingeniería, Maestría, Doctorado");

            RuleFor(x => x.UniversityId)
                .GreaterThan(0).WithMessage("El ID de la universidad debe ser mayor a 0");
        }

        private bool BeAValidDegreeType(string degreeType)
        {
            var validTypes = new[] { "Técnico", "Tecnólogo", "Licenciatura", "Ingeniería", "Maestría", "Doctorado" };
            return validTypes.Contains(degreeType, StringComparer.OrdinalIgnoreCase);
        }
    }
}