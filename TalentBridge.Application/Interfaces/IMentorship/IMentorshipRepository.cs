using TalentBridge.Domain.Entities;
using TalentBridge.Domain.Enums;

namespace TalentBridge.API.Interfaces.IMentorship
{
    public interface IMentorshipRepository
    {
        // Operaciones CRUD básicas
        Task<Mentorship?> GetByIdAsync(int id);
        Task<Mentorship?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Mentorship>> GetAllAsync();
        Task<Mentorship> CreateAsync(Mentorship mentorship);
        Task<Mentorship> UpdateAsync(Mentorship mentorship);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Búsquedas específicas
        Task<IEnumerable<Mentorship>> GetByMentorIdAsync(int mentorId);
        Task<IEnumerable<Mentorship>> GetByMenteeIdAsync(int menteeId);
        Task<IEnumerable<Mentorship>> GetByUserIdAsync(int userId); // Todas las mentorías del usuario
        Task<IEnumerable<Mentorship>> GetActiveMentorshipsAsync();
        Task<IEnumerable<Mentorship>> GetCompletedMentorshipsAsync();
        Task<IEnumerable<Mentorship>> GetPendingMentorshipsAsync();

        // Estadísticas
        Task<int> CountActiveMentorshipsAsync(int mentorId);
        Task<int> CountCompletedMentorshipsAsync(int mentorId);
        Task<int> CountTotalMenteesAsync(int mentorId);
        Task<decimal> CalculateMentorRatingAsync(int mentorId);

        // Operaciones específicas
        Task<bool> ChangeStatusAsync(int id, MentorshipStatus status);
        Task<bool> AddRatingAsync(int id, decimal mentorRating, decimal? menteeRating, string? feedback);
        Task<bool> CompleteMentorshipAsync(int id);
        Task<bool> CancelMentorshipAsync(int id, string reason);
    }
}