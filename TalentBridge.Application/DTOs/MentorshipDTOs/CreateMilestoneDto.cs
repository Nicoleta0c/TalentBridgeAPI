using TalentBridge.Domain.Enums;

namespace TalentBridge.API.DTOs.MentorshipDTOs
{
    public class CreateMilestoneDto
    {
        public int MentorshipId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
    }
}

// UpdateMilestoneDto.cs
namespace TalentBridge.API.DTOs.MentorshipDTOs
{
    public class UpdateMilestoneDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public MilestoneStatus? Status { get; set; }
        public int? ProgressPercentage { get; set; }
        public string? Notes { get; set; }
        public string? Evidence { get; set; }
    }
}

// MentorshipMilestoneDto.cs
namespace TalentBridge.API.DTOs.MentorshipDTOs
{
    public class MentorshipMilestoneDto
    {
        public int Id { get; set; }
        public int MentorshipId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public MilestoneStatus Status { get; set; }
        public int ProgressPercentage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}