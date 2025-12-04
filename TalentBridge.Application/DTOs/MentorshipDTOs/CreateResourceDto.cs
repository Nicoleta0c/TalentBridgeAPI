using TalentBridge.Domain.Enums;

namespace TalentBridge.API.DTOs.MentorshipDTOs
{
    public class CreateResourceDto
    {
        public int MentorshipId { get; set; }
        public int? SessionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ResourceType Type { get; set; } = ResourceType.Document;
        public string Url { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public bool IsPublic { get; set; } = false;
        public bool IsDownloadable { get; set; } = true;
    }
}

// UpdateResourceDto.cs
namespace TalentBridge.API.DTOs.MentorshipDTOs
{
    public class UpdateResourceDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public ResourceType? Type { get; set; }
        public string? Url { get; set; }
        public string? ThumbnailUrl { get; set; }
        public bool? IsPublic { get; set; }
        public bool? IsDownloadable { get; set; }
    }
}

// MentorshipResourceDto.cs
namespace TalentBridge.API.DTOs.MentorshipDTOs
{
    public class MentorshipResourceDto
    {
        public int Id { get; set; }
        public int MentorshipId { get; set; }
        public int? SessionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public bool IsPublic { get; set; }
        public bool IsDownloadable { get; set; }
        public int CreatedByUserId { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}