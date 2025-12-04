using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentBridge.API.DTOs.MentorshipDTOs;
using TalentBridge.API.Interfaces.IMentorship;

namespace TalentBridge.API.Controllers.MentorshipControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MentorshipsController : ControllerBase
    {
        private readonly IMentorshipService _mentorshipService;
        private readonly ILogger<MentorshipsController> _logger;

        public MentorshipsController(
            IMentorshipService mentorshipService,
            ILogger<MentorshipsController> logger)
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
        /// Obtiene todas las mentorías del usuario autenticado
        /// </summary>
        [HttpGet("my-mentorships")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<MentorshipDto>>> GetMyMentorships()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var mentorships = await _mentorshipService.GetUserMentorshipsAsync(userId.Value);
                return Ok(mentorships);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener mentorías del usuario");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene una mentoría por ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MentorshipDto>> GetMentorship(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var mentorship = await _mentorshipService.GetMentorshipByIdAsync(id, userId.Value);

                if (mentorship == null)
                {
                    return NotFound(new { message = $"Mentoría con ID {id} no encontrada" });
                }

                return Ok(mentorship);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener mentoría con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene detalles completos de una mentoría
        /// </summary>
        [HttpGet("{id}/details")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MentorshipDetailDto>> GetMentorshipDetails(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var mentorship = await _mentorshipService.GetMentorshipDetailByIdAsync(id, userId.Value);

                if (mentorship == null)
                {
                    return NotFound(new { message = $"Mentoría con ID {id} no encontrada" });
                }

                return Ok(mentorship);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalles de mentoría con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crea una nueva mentoría
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MentorshipDto>> CreateMentorship([FromBody] CreateMentorshipDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var mentorship = await _mentorshipService.CreateMentorshipAsync(dto, userId.Value);
                return CreatedAtAction(nameof(GetMentorship), new { id = mentorship.Id }, mentorship);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear mentoría");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza una mentoría existente
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<MentorshipDto>> UpdateMentorship(int id, [FromBody] UpdateMentorshipDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var mentorship = await _mentorshipService.UpdateMentorshipAsync(id, dto, userId.Value);
                return Ok(mentorship);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar mentoría con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Completa una mentoría
        /// </summary>
        [HttpPost("{id}/complete")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CompleteMentorship(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                await _mentorshipService.CompleteMentorshipAsync(id, userId.Value);
                return Ok(new { message = "Mentoría completada exitosamente" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al completar mentoría con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Cancela una mentoría
        /// </summary>
        [HttpPost("{id}/cancel")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CancelMentorship(int id, [FromBody] CancelMentorshipRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                await _mentorshipService.CancelMentorshipAsync(id, userId.Value, request.Reason);
                return Ok(new { message = "Mentoría cancelada exitosamente" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar mentoría con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }

    public class CancelMentorshipRequest
    {
        public string Reason { get; set; } = string.Empty;
    }
}