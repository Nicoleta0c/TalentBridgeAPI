using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TalentBridge.Application.DTOs;
using TalentBridge.Application.DTOs.UserDTOs;
using TalentBridge.Application.Interfaces;
using TalentBridge.Application.Settings;
using TalentBridge.Domain.Entities;

namespace TalentBridge.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly JwtSettings _jwtSettings;

        public AuthService(IAuthRepository authRepository, IOptions<JwtSettings> jwtSettings)
        {
            _authRepository = authRepository;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<AuthResultDto> RegisterAsync(string fullName, string email, string password)
        {
            var existingUser = await _authRepository.GetByEmailAsync(email);
            if (existingUser != null)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Email ya existe"
                };
            }

            var user = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _authRepository.AddAsync(user);

            var token = GenerateJwtToken(user);
            var refreshToken = await _authRepository.GenerateAndSaveRefreshTokenAsync(user.Id);

            return new AuthResultDto
            {
                Success = true,
                Message = "Registro exitoso",
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                User = MapToUserDto(user)
            };
        }

        public async Task<AuthResultDto> LoginAsync(string email, string password)
        {
            var user = await _authRepository.GetByEmailAsync(email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Email o contraseña inválidos"
                };
            }

            if (!user.IsActive)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Cuenta inactiva"
                };
            }

            var token = GenerateJwtToken(user);
            var refreshToken = await _authRepository.GenerateAndSaveRefreshTokenAsync(user.Id);

            return new AuthResultDto
            {
                Success = true,
                Message = "Login exitoso",
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                User = MapToUserDto(user)
            };
        }

        public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Refresh token es requerido"
                };
            }

            var storedToken = await _authRepository.GetValidRefreshTokenAsync(refreshToken);

            if (storedToken == null)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Refresh token inválido o expirado"
                };
            }

            var user = storedToken.User;
            if (user == null || !user.IsActive)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Usuario no encontrado o inactivo"
                };
            }

            storedToken.IsRevoked = true;
            //storedToken.RevokedAt = DateTime.UtcNow;
            await _authRepository.UpdateRefreshTokenAsync(storedToken);

            var newJwtToken = GenerateJwtToken(user);
            var newRefreshToken = await _authRepository.GenerateAndSaveRefreshTokenAsync(user.Id);

            return new AuthResultDto
            {
                Success = true,
                Message = "Token refrescado exitosamente",
                Token = newJwtToken,
                RefreshToken = newRefreshToken,
                TokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                User = MapToUserDto(user)
            };
        }

        public async Task RevokeTokenAsync(string refreshToken)
        {
            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _authRepository.RevokeRefreshTokenAsync(refreshToken);
            }
        }

        public async Task RevokeAllTokensAsync(int userId)
        {
            await _authRepository.RevokeAllRefreshTokensForUserAsync(userId);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }
    }
}