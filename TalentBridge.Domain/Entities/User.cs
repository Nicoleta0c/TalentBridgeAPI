using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TalentBridge.Domain.Enums;

namespace TalentBridge.Domain.Entities
{
    public class User
    {
        public int            Id                { get; set; }
        public string         FullName          { get; set; } = string.Empty;
        public string         Email             { get; set; } = string.Empty;
        public string         PasswordHash      { get; set; } = string.Empty;
        public string         Role              { get; set; } = "User"; 
        public bool           IsActive          { get; set; } = true;
        public DateTime       CreatedAt         { get; set; } = DateTime.UtcNow;
        public DateTime?      UpdatedAt         { get; set; }
        public int? UniversityId { get; set; }
        public University? University { get; set; }

        public int? CareerCareerId { get; set; }
        public UniversityCareer? Career { get; set; }

        public string? StudentId { get; set; } // Matrícula del estudiante
        public int? ExpectedGraduationYear { get; set; }
        public string? CurrentSemester { get; set; } // Ej: "5to", "8vo"

        // Relaciones
        public ICollection<CommunityMember> CommunityMemberships { get; set; } = new List<CommunityMember>();
        public ICollection<Community> CreatedCommunities { get; set; } = new List<Community>();
        public ICollection<CommunityPost> CommunityPosts { get; set; } = new List<CommunityPost>();
        public ICollection<CommunityComment> CommunityComments { get; set; } = new List<CommunityComment>();
        public ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();
        public ICollection<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();

        // Información de mentor/mentee
        public bool IsMentor { get; set; } = false;
        public bool IsMentee { get; set; } = true; // Por defecto, todos pueden ser mentees

        // Perfil de mentor
        public string? MentorTitle { get; set; } // Ej: "Senior Software Engineer"
        public string? MentorBio { get; set; }
        public int? YearsOfExperience { get; set; }
        
        [JsonIgnore]
        public List<string> MentorExpertise { get; set; } = new List<string>();
        
        [JsonIgnore]
        public List<string> MentorIndustries { get; set; } = new List<string>();
        
        public decimal? HourlyRate { get; set; }
        public bool AvailableForMentorship { get; set; } = false;
        public int MaxMentees { get; set; } = 3;

        // Calificaciones como mentor
        public decimal? MentorAverageRating { get; set; }
        public int? TotalMentorshipSessions { get; set; } = 0;
        public int? TotalMenteesHelped { get; set; } = 0;

        // Preferencias de mentoría
        [JsonIgnore]
        public List<string> PreferredMentorshipCategories { get; set; } = new List<string>();
        
        public MeetingMethod? PreferredMeetingMethod { get; set; }
        public string? Timezone { get; set; }
        
        [JsonIgnore]
        public string? AvailabilitySchedule { get; set; } // JSON con horarios disponibles

        // Relaciones de mentorías
        public ICollection<Mentorship> MentorMentorships { get; set; } = new List<Mentorship>();
        public ICollection<Mentorship> MenteeMentorships { get; set; } = new List<Mentorship>();
        public ICollection<MentorshipRequest> MentorshipRequests { get; set; } = new List<MentorshipRequest>();
        public ICollection<MentorApplication> MentorApplications { get; set; } = new List<MentorApplication>();
        public ICollection<SessionAttendance> SessionAttendances { get; set; } = new List<SessionAttendance>();
        public ICollection<MentorshipResource> CreatedResources { get; set; } = new List<MentorshipResource>();

    }
}