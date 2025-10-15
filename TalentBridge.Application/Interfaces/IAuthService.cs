using TalentBridge.Application.DTOs;

namespace TalentBridge.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(string fullName, string email, string password);
        Task<AuthResultDto> LoginAsync(string email, string password);
    }
}
