using FluentValidation;
using TalentBridge.Application.DTOs.CVDTOs;

namespace TalentBridge.Application.Validations
{
    public class UploadCVDtoValidator : AbstractValidator<UploadCVDto>
    {
        public UploadCVDtoValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Valid user ID is required");

            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("File name is required")
                .MaximumLength(255).WithMessage("File name cannot exceed 255 characters")
                .Must(HaveValidExtension).WithMessage("Only PDF, DOC, DOCX, and TXT files are allowed");

            RuleFor(x => x.FileType)
                .NotEmpty().WithMessage("File type is required")
                .Must(BeValidFileType).WithMessage("Invalid file type");

            RuleFor(x => x.FileSize)
                .GreaterThan(0).WithMessage("File size must be greater than 0")
                .LessThanOrEqualTo(10 * 1024 * 1024).WithMessage("File size cannot exceed 10MB");

            RuleFor(x => x.Summary)
                .MaximumLength(1000).WithMessage("Summary cannot exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Summary));
        }

        private bool HaveValidExtension(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".txt" };
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }

        private bool BeValidFileType(string fileType)
        {
            var validTypes = new[] {
                "application/pdf",
                "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "text/plain"
            };
            return validTypes.Contains(fileType);
        }
    }
}