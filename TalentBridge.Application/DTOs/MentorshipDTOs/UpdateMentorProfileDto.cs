using TalentBridge.Domain.Enums;

namespace TalentBridge.API.DTOs.MentorshipDTOs
{
    public class UpdateMentorProfileDto
    {
        public bool? IsMentor { get; set; }
        public string? MentorTitle { get; set; }
        public string? MentorBio { get; set; }
        public int? YearsOfExperience { get; set; }
        public List<string>? MentorExpertise { get; set; }
        public List<string>? MentorIndustries { get; set; }
        public decimal? HourlyRate { get; set; }
        public bool? AvailableForMentorship { get; set; }
        public int? MaxMentees { get; set; }
        public List<string>? PreferredMentorshipCategories { get; set; }
        public MeetingMethod? PreferredMeetingMethod { get; set; }
        public string? Timezone { get; set; }
        public string? AvailabilitySchedule { get; set; }
    }
}

// MentorProfileDto.cs
namespace TalentBridge.API.DTOs.MentorshipDTOs
{
    public class MentorProfileDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string? MentorTitle { get; set; }
        public string? MentorBio { get; set; }
        public int? YearsOfExperience { get; set; }
        public List<string> MentorExpertise { get; set; } = new();
        public List<string> MentorIndustries { get; set; } = new();
        public decimal? HourlyRate { get; set; }
        public bool AvailableForMentorship { get; set; }
        public int MaxMentees { get; set; }
        public int CurrentMenteesCount { get; set; }
        public decimal? MentorAverageRating { get; set; }
        public int? TotalMentorshipSessions { get; set; }
        public int? TotalMenteesHelped { get; set; }
        public List<string> PreferredMentorshipCategories { get; set; } = new();
        public MeetingMethod? PreferredMeetingMethod { get; set; }
        public string? Timezone { get; set; }
    }
}

// SearchMentorsDto.cs
namespace TalentBridge.API.DTOs.MentorshipDTOs
{
    public class SearchMentorsDto
    {
        public string? Category { get; set; }
        public List<string>? Expertise { get; set; }
        public decimal? MaxHourlyRate { get; set; }
        public int? MinYearsOfExperience { get; set; }
        public decimal? MinRating { get; set; }
        public bool? AvailableNow { get; set; }
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}