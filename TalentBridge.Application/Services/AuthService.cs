using System.Security.Cryptography;
using System.Text;
using TalentBridge.Application.DTOs;
using TalentBridge.Application.Interfaces;
using TalentBridge.Domain.Entities;

namespace TalentBridge.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repo;

        public AuthService(IAuthRepository repo)
        {
            _repo = repo;
        }

        public async Task<AuthResultDto> RegisterAsync(string fullName, string email, string password)
        {
            var existing = await _repo.GetByEmailAsync(email);
            if (existing != null)
                return new AuthResultDto { Success = false, Message = "El correo ya esta registrado" };

            var user = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = HashPassword(password)
            };

            await _repo.AddAsync(user);

            return new AuthResultDto { Success = true, Message = "Registro exitoso" };
        }

        public async Task<AuthResultDto> LoginAsync(string email, string password)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null || user.PasswordHash != HashPassword(password))
                return new AuthResultDto { Success = false, Message = "Credenciales invalidas." };

            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user.Email}:{DateTime.UtcNow}"));

            return new AuthResultDto { Success = true, Token = token, Message = "Inicio de sesion exitoso." };
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
