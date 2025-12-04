using TalentBridge.Domain.Enums;

namespace TalentBridge.API.DTOs.MentorshipDTOs
{
    public class CreateMentorshipDto
    {
        public int MentorId { get; set; }
        public int MenteeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Goals { get; set; }
        public string? Expectations { get; set; }
        public string Category { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
        public int DurationWeeks { get; set; } = 4;
        public int SessionsPerWeek { get; set; } = 1;
        public int SessionDurationMinutes { get; set; } = 60;
        public MeetingMethod MeetingMethod { get; set; } = MeetingMethod.Virtual;
        public string? MeetingLink { get; set; }
    }
}

// UpdateMentorshipDto.cs
namespace TalentBridge.API.DTOs.MentorshipDTOs
{
    public class UpdateMentorshipDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Goals { get; set; }
        public string? Expectations { get; set; }
        public int? DurationWeeks { get; set; }
        public int? SessionsPerWeek { get; set; }
        public int? SessionDurationMinutes { get; set; }
        public MeetingMethod? MeetingMethod { get; set; }
        public string? MeetingLink { get; set; }
        public MentorshipStatus? Status { get; set; }
    }
}

// MentorshipDto.cs
namespace TalentBridge.API.DTOs.MentorshipDTOs
{
    public class MentorshipDto
    {
        public int Id { get; set; }
        public int MentorId { get; set; }
        public string MentorName { get; set; } = string.Empty;
        public string MentorEmail { get; set; } = string.Empty;
        public int MenteeId { get; set; }
        public string MenteeName { get; set; } = string.Empty;
        public string MenteeEmail { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
        public MentorshipStatus Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int DurationWeeks { get; set; }
        public int TotalSessions { get; set; }
        public int CompletedSessions { get; set; }
        public int TotalMilestones { get; set; }
        public int CompletedMilestones { get; set; }
        public decimal? MentorRating { get; set; }
        public decimal? MenteeRating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

// MentorshipDetailDto.cs
namespace TalentBridge.API.DTOs.MentorshipDTOs
{
    public class MentorshipDetailDto : MentorshipDto
    {
        public string? Goals { get; set; }
        public string? Expectations { get; set; }
        public int SessionsPerWeek { get; set; }
        public int SessionDurationMinutes { get; set; }
        public MeetingMethod MeetingMethod { get; set; }
        public string? MeetingLink { get; set; }
        public string? MentorFeedback { get; set; }
        public string? MenteeFeedback { get; set; }
        public List<MentorshipSessionDto> UpcomingSessions { get; set; } = new();
        public List<MentorshipMilestoneDto> RecentMilestones { get; set; } = new();
        public List<MentorshipResourceDto> RecentResources { get; set; } = new();
    }
}