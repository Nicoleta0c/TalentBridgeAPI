using TalentBridge.Domain.Enums;

namespace TalentBridge.Domain.Entities
{
    public class MentorshipMilestone
    {
        public int Id { get; set; }

        public int MentorshipId { get; set; }
        public Mentorship Mentorship { get; set; } = null!;

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        public MilestoneStatus Status { get; set; } = MilestoneStatus.Pending;
        public int ProgressPercentage { get; set; } = 0; // 0-100

        public string? Notes { get; set; }
        public string? Evidence { get; set; } // URL de evidencia

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}