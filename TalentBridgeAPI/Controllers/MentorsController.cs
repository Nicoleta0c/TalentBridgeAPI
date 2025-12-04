using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentBridge.API.DTOs.MentorshipDTOs;
using TalentBridge.API.Interfaces.IMentorship;

namespace TalentBridge.API.Controllers.MentorshipControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MentorsController : ControllerBase
    {
        private readonly IMentorshipService _mentorshipService;
        private readonly ILogger<MentorsController> _logger;

        public MentorsController(
            IMentorshipService mentorshipService,
            ILogger<MentorsController> logger)
        {
            _mentorshipService = mentorshipService;
            _logger = logger;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        /// <summary>
        /// Obtiene el perfil de mentor del usuario autenticado
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MentorProfileDto>> GetMyProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var profile = await _mentorshipService.GetMentorProfileAsync(userId.Value);

                if (profile == null)
                {
                    return NotFound(new { message = "Perfil de mentor no encontrado" });
                }

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener perfil de mentor");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene el perfil de mentor de un usuario específico
        /// </summary>
        [HttpGet("{userId}/profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MentorProfileDto>> GetMentorProfile(int userId)
        {
            try
            {
                var profile = await _mentorshipService.GetMentorProfileAsync(userId);

                if (profile == null)
                {
                    return NotFound(new { message = $"Perfil de mentor para usuario {userId} no encontrado" });
                }

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener perfil de mentor para usuario {UserId}", userId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza el perfil de mentor del usuario autenticado
        /// </summary>
        [HttpPut("profile")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MentorProfileDto>> UpdateProfile([FromBody] UpdateMentorProfileDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var profile = await _mentorshipService.UpdateMentorProfileAsync(userId.Value, dto);
                return Ok(profile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar perfil de mentor");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Busca mentores disponibles
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MentorProfileDto>>> SearchMentors([FromQuery] SearchMentorsDto dto)
        {
            try
            {
                var mentors = await _mentorshipService.SearchMentorsAsync(dto);
                return Ok(mentors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar mentores");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene mentores recomendados para el usuario autenticado
        /// </summary>
        [HttpGet("recommended")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<MentorProfileDto>>> GetRecommendedMentors()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var mentors = await _mentorshipService.GetRecommendedMentorsAsync(userId.Value);
                return Ok(mentors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener mentores recomendados");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene todos los mentores disponibles
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MentorProfileDto>>> GetAllMentors()
        {
            try
            {
                var searchDto = new SearchMentorsDto
                {
                    Page = 1,
                    PageSize = 50
                };

                var mentors = await _mentorshipService.SearchMentorsAsync(searchDto);
                return Ok(mentors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los mentores");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}