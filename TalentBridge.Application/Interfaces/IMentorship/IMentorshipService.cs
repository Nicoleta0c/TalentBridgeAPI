using TalentBridge.API.DTOs.MentorshipDTOs;

namespace TalentBridge.API.Interfaces.IMentorship
{
    public interface IMentorshipService
    {
        // Mentorship CRUD
        Task<MentorshipDto?> GetMentorshipByIdAsync(int id, int currentUserId);
        Task<MentorshipDetailDto?> GetMentorshipDetailByIdAsync(int id, int currentUserId);
        Task<IEnumerable<MentorshipDto>> GetUserMentorshipsAsync(int userId);
        Task<IEnumerable<MentorshipDto>> GetActiveMentorshipsAsync();
        Task<MentorshipDto> CreateMentorshipAsync(CreateMentorshipDto dto, int currentUserId);
        Task<MentorshipDto> UpdateMentorshipAsync(int id, UpdateMentorshipDto dto, int currentUserId);
        Task<bool> DeleteMentorshipAsync(int id, int currentUserId);
        Task<bool> CompleteMentorshipAsync(int id, int currentUserId);
        Task<bool> CancelMentorshipAsync(int id, int currentUserId, string reason);

        // Sesiones
        Task<IEnumerable<MentorshipSessionDto>> GetMentorshipSessionsAsync(int mentorshipId, int currentUserId);
        Task<MentorshipSessionDto?> GetSessionByIdAsync(int id, int currentUserId);
        Task<MentorshipSessionDto> CreateSessionAsync(CreateSessionDto dto, int currentUserId);
        Task<MentorshipSessionDto> UpdateSessionAsync(int id, UpdateSessionDto dto, int currentUserId);
        Task<bool> DeleteSessionAsync(int id, int currentUserId);
        Task<bool> StartSessionAsync(int id, int currentUserId);
        Task<bool> EndSessionAsync(int id, int currentUserId);

        // Asistencia
        Task<IEnumerable<AttendanceDto>> GetSessionAttendancesAsync(int sessionId, int currentUserId);
        Task<AttendanceDto?> GetAttendanceAsync(int id, int currentUserId);
        Task<AttendanceDto> UpdateAttendanceAsync(int id, UpdateAttendanceDto dto, int currentUserId);
        Task<bool> MarkAttendanceAsync(int sessionId, int userId, int currentUserId);

        // Hitos
        Task<IEnumerable<MentorshipMilestoneDto>> GetMentorshipMilestonesAsync(int mentorshipId, int currentUserId);
        Task<MentorshipMilestoneDto?> GetMilestoneByIdAsync(int id, int currentUserId);
        Task<MentorshipMilestoneDto> CreateMilestoneAsync(CreateMilestoneDto dto, int currentUserId);
        Task<MentorshipMilestoneDto> UpdateMilestoneAsync(int id, UpdateMilestoneDto dto, int currentUserId);
        Task<bool> DeleteMilestoneAsync(int id, int currentUserId);
        Task<bool> MarkMilestoneCompleteAsync(int id, int currentUserId, string? evidence);

        // Recursos
        Task<IEnumerable<MentorshipResourceDto>> GetMentorshipResourcesAsync(int mentorshipId, int currentUserId);
        Task<MentorshipResourceDto?> GetResourceByIdAsync(int id, int currentUserId);
        Task<MentorshipResourceDto> CreateResourceAsync(CreateResourceDto dto, int currentUserId);
        Task<MentorshipResourceDto> UpdateResourceAsync(int id, UpdateResourceDto dto, int currentUserId);
        Task<bool> DeleteResourceAsync(int id, int currentUserId);

        // Solicitudes y aplicaciones
        Task<IEnumerable<MentorshipRequestDto>> GetMentorshipRequestsAsync(int? userId = null);
        Task<MentorshipRequestDto?> GetRequestByIdAsync(int id, int currentUserId);
        Task<MentorshipRequestDto> CreateRequestAsync(CreateMentorshipRequestDto dto, int currentUserId);
        Task<bool> DeleteRequestAsync(int id, int currentUserId);
        Task<IEnumerable<MentorApplicationDto>> GetRequestApplicationsAsync(int requestId, int currentUserId);
        Task<MentorApplicationDto> ApplyToRequestAsync(CreateMentorApplicationDto dto, int currentUserId);
        Task<bool> WithdrawApplicationAsync(int applicationId, int currentUserId);
        Task<bool> AcceptApplicationAsync(int applicationId, int currentUserId);

        // Perfil de mentor
        Task<MentorProfileDto?> GetMentorProfileAsync(int userId);
        Task<MentorProfileDto> UpdateMentorProfileAsync(int userId, UpdateMentorProfileDto dto);
        Task<IEnumerable<MentorProfileDto>> SearchMentorsAsync(SearchMentorsDto dto);
        Task<IEnumerable<MentorProfileDto>> GetRecommendedMentorsAsync(int userId);
    }
}