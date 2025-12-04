using TalentBridge.Domain.Enums;

namespace TalentBridge.API.DTOs.MentorshipDTOs
{
    public class CreateMentorshipRequestDto
    {
        public int? MentorId { get; set; } // Especificar mentor o dejar abierto
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Goals { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
        public int PreferredDurationWeeks { get; set; } = 4;
        public int PreferredSessionsPerWeek { get; set; } = 1;
        public MeetingMethod PreferredMeetingMethod { get; set; } = MeetingMethod.Virtual;
    }
}

// MentorshipRequestDto.cs
namespace TalentBridge.API.DTOs.MentorshipDTOs
{
    public class MentorshipRequestDto
    {
        public int Id { get; set; }
        public int MenteeId { get; set; }
        public string MenteeName { get; set; } = string.Empty;
        public int? MentorId { get; set; }
        public string? MentorName { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
        public RequestStatus Status { get; set; }
        public int TotalApplications { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}

// CreateMentorApplicationDto.cs
namespace TalentBridge.API.DTOs.MentorshipDTOs
{
    public class CreateMentorApplicationDto
    {
        public int RequestId { get; set; }
        public string Proposal { get; set; } = string.Empty;
        public string? Experience { get; set; }
        public string? WhyChooseMe { get; set; }
        public int? ProposedDurationWeeks { get; set; }
        public int? ProposedSessionsPerWeek { get; set; }
        public MeetingMethod? ProposedMeetingMethod { get; set; }
    }
}

// MentorApplicationDto.cs
namespace TalentBridge.API.DTOs.MentorshipDTOs
{
    public class MentorApplicationDto
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public int MentorId { get; set; }
        public string MentorName { get; set; } = string.Empty;
        public string MentorTitle { get; set; } = string.Empty;
        public decimal? MentorRating { get; set; }
        public int? MentorExperienceYears { get; set; }
        public string Proposal { get; set; } = string.Empty;
        public ApplicationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}