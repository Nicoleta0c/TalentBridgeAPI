using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentBridge.Application.DTOs.UserDTOs;
using TalentBridge.Application.Interfaces;
using TalentBridge.Application.Services;
using RegisterRequest = TalentBridge.Application.DTOs.UserDTOs.RegisterRequestDto;

namespace TalentBridge.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<LoginRequestDto> _loginValidator;
        private readonly IValidator<RefreshTokenRequestDto> _refreshTokenValidator;
        private readonly IValidator<RevokeTokenRequest> _revokeTokenValidator;
        private readonly IValidator<CreateAdminDto> _createAdminValidator;

        public AuthController(
            IAuthService authService,
            IUserService userService, 
            IValidator<RegisterRequest> registerValidator,
            IValidator<LoginRequestDto> loginValidator,
            IValidator<RefreshTokenRequestDto> refreshTokenValidator,
            IValidator<RevokeTokenRequest> revokeTokenValidator,
            IValidator<CreateAdminDto> createAdminValidator) 
        {
            _authService = authService;
            _userService = userService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
            _refreshTokenValidator = refreshTokenValidator;
            _revokeTokenValidator = revokeTokenValidator;
            _createAdminValidator = createAdminValidator;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var validationResult = await _registerValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => new
                    {
                        property = e.PropertyName,
                        error = e.ErrorMessage
                    })
                });
            }

            var result = await _authService.RegisterAsync(request.FullName, request.Email, request.Password);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var validationResult = await _loginValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => new
                    {
                        property = e.PropertyName,
                        error = e.ErrorMessage
                    })
                });
            }

            var result = await _authService.LoginAsync(request.Email, request.Password);
            return result.Success ? Ok(result) : Unauthorized(result);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var validationResult = await _refreshTokenValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => new
                    {
                        property = e.PropertyName,
                        error = e.ErrorMessage
                    })
                });
            }

            var result = await _authService.RefreshTokenAsync(request.RefreshToken);
            return result.Success ? Ok(result) : Unauthorized(result);
        }

        [HttpPost("revoke-token")]
        [Authorize]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
        {
            var validationResult = await _revokeTokenValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => new
                    {
                        property = e.PropertyName,
                        error = e.ErrorMessage
                    })
                });
            }

            await _authService.RevokeTokenAsync(request.RefreshToken);
            return Ok(new { message = "Token revoked successfully" });
        }

        [HttpPost("create-admin")]
        //[Authorize(Roles = "Admin")] // Solo admins pueden crear otros admins
        [AllowAnonymous] // temporalmente
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminDto request)
        {
            var validationResult = await _createAdminValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => new
                    {
                        property = e.PropertyName,
                        error = e.ErrorMessage
                    })
                });
            }

            try
            {
                var admin = await _userService.CreateAdminAsync(request);
                return Ok(new
                {
                    success = true,
                    message = "Admin created successfully",
                    data = admin
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}