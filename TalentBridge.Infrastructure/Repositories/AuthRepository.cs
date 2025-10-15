using TalentBridge.Application.Interfaces;
using TalentBridge.Domain.Entities;

namespace TalentBridge.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private static readonly List<User> _users = new();

        public Task<User?> GetByEmailAsync(string email)
        {
            var user = _users.FirstOrDefault(u => u.Email == email);
            return Task.FromResult(user);
        }

        public Task AddAsync(User user)
        {
            user.Id = _users.Count + 1;
            _users.Add(user);
            return Task.CompletedTask;
        }
    }
}
