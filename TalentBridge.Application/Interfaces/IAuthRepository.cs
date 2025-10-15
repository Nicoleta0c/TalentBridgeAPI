using TalentBridge.Domain.Entities;

namespace TalentBridge.Application.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
    }
}
