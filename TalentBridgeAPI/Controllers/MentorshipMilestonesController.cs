using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentBridge.API.DTOs.MentorshipDTOs;
using TalentBridge.API.Interfaces.IMentorship;

namespace TalentBridge.API.Controllers.MentorshipControllers
{
    [ApiController]
    [Route("api/mentorships/{mentorshipId}/milestones")]
    public class MentorshipMilestonesController : ControllerBase
    {
        private readonly IMentorshipService _mentorshipService;
        private readonly ILogger<MentorshipMilestonesController> _logger;

        public MentorshipMilestonesController(
            IMentorshipService mentorshipService,
            ILogger<MentorshipMilestonesController> logger)
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
        /// Obtiene todos los hitos de una mentoría
        /// </summary>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<MentorshipMilestoneDto>>> GetMilestones(int mentorshipId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var milestones = await _mentorshipService.GetMentorshipMilestonesAsync(mentorshipId, userId.Value);
                return Ok(milestones);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener hitos de mentoría {MentorshipId}", mentorshipId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene un hito por ID
        /// </summary>
        [HttpGet("{milestoneId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MentorshipMilestoneDto>> GetMilestone(int mentorshipId, int milestoneId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var milestone = await _mentorshipService.GetMilestoneByIdAsync(milestoneId, userId.Value);

                if (milestone == null)
                {
                    return NotFound(new { message = $"Hito con ID {milestoneId} no encontrado" });
                }

                return Ok(milestone);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener hito {MilestoneId} de mentoría {MentorshipId}", milestoneId, mentorshipId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crea un nuevo hito
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MentorshipMilestoneDto>> CreateMilestone(int mentorshipId, [FromBody] CreateMilestoneDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                dto.MentorshipId = mentorshipId;
                var milestone = await _mentorshipService.CreateMilestoneAsync(dto, userId.Value);
                return CreatedAtAction(nameof(GetMilestone), new { mentorshipId, milestoneId = milestone.Id }, milestone);
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
                _logger.LogError(ex, "Error al crear hito en mentoría {MentorshipId}", mentorshipId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza un hito existente
        /// </summary>
        [HttpPut("{milestoneId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<MentorshipMilestoneDto>> UpdateMilestone(int mentorshipId, int milestoneId, [FromBody] UpdateMilestoneDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var milestone = await _mentorshipService.UpdateMilestoneAsync(milestoneId, dto, userId.Value);
                return Ok(milestone);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar hito {MilestoneId}", milestoneId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Elimina un hito
        /// </summary>
        [HttpDelete("{milestoneId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteMilestone(int mentorshipId, int milestoneId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                await _mentorshipService.DeleteMilestoneAsync(milestoneId, userId.Value);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar hito {MilestoneId}", milestoneId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Marca un hito como completado
        /// </summary>
        [HttpPost("{milestoneId}/complete")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CompleteMilestone(int mentorshipId, int milestoneId, [FromBody] CompleteMilestoneRequest? request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                await _mentorshipService.MarkMilestoneCompleteAsync(milestoneId, userId.Value, request?.Evidence);
                return Ok(new { message = "Hito marcado como completado exitosamente" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al completar hito {MilestoneId}", milestoneId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }

    public class CompleteMilestoneRequest
    {
        public string? Evidence { get; set; }
    }
}