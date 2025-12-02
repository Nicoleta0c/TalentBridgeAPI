using TalentBridge.Domain.Entities;
using TalentBridge.Application.Interfaces;

namespace TalentBridge.Infrastructure.Data
{
    public class DatabaseSeeder
    {
        private readonly IAuthRepository _authRepository;

        public DatabaseSeeder(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task SeedAdminAsync()
        {
            var existingAdmin = await _authRepository.GetByEmailAsync("admin@talentbridge.com");

            if (existingAdmin == null)
            {
                var admin = new User
                {
                    FullName = "Administrator",
                    Email = "admin@talentbridge.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123*"), 
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _authRepository.AddAsync(admin);
            }
        }
    }
}