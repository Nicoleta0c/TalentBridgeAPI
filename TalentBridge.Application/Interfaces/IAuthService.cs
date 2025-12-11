using TalentBridge.Application.DTOs;

namespace TalentBridge.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(string fullName, string email, string password);
        Task<AuthResultDto> LoginAsync(string email, string password);
        Task<AuthResultDto> RefreshTokenAsync(string refreshToken);
        Task RevokeTokenAsync(string refreshToken);
        Task RevokeAllTokensAsync(int userId);
    }
}