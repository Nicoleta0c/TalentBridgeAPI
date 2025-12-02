using TalentBridge.Domain.Entities;

namespace TalentBridge.Application.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task<User?> GetByIdAsync(int id);
        Task<bool> UserExistsAsync(string email);
        Task<string> GenerateAndSaveRefreshTokenAsync(int userId);
        Task<bool> ValidateRefreshTokenAsync(int userId, string token);
        Task RevokeRefreshTokenAsync(string token);
    }
}