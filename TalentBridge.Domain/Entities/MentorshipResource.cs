
using TalentBridge.Domain.Enums;

namespace TalentBridge.Domain.Entities
{
    public class MentorshipResource
    {
        public int Id { get; set; }

        public int MentorshipId { get; set; }
        public Mentorship Mentorship { get; set; } = null!;

        public int? SessionId { get; set; }
        public MentorshipSession? Session { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ResourceType Type { get; set; } = ResourceType.Document;
        public string Url { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }

        public bool IsPublic { get; set; } = false;
        public bool IsDownloadable { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public int CreatedByUserId { get; set; }
        public User CreatedBy { get; set; } = null!;
    }
}