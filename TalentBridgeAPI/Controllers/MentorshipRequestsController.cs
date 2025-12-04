using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentBridge.API.DTOs.MentorshipDTOs;
using TalentBridge.API.Interfaces.IMentorship;

namespace TalentBridge.API.Controllers.MentorshipControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MentorshipRequestsController : ControllerBase
    {
        private readonly IMentorshipService _mentorshipService;
        private readonly ILogger<MentorshipRequestsController> _logger;

        public MentorshipRequestsController(
            IMentorshipService mentorshipService,
            ILogger<MentorshipRequestsController> logger)
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
        /// Obtiene todas las solicitudes de mentoría (públicas)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MentorshipRequestDto>>> GetRequests()
        {
            try
            {
                var requests = await _mentorshipService.GetMentorshipRequestsAsync();
                return Ok(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener solicitudes de mentoría");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene las solicitudes del usuario autenticado
        /// </summary>
        [HttpGet("my-requests")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<MentorshipRequestDto>>> GetMyRequests()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var requests = await _mentorshipService.GetMentorshipRequestsAsync(userId.Value);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener solicitudes del usuario");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene una solicitud por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MentorshipRequestDto>> GetRequest(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var request = await _mentorshipService.GetRequestByIdAsync(id, userId ?? -1);

                if (request == null)
                {
                    return NotFound(new { message = $"Solicitud con ID {id} no encontrada" });
                }

                return Ok(request);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener solicitud con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crea una nueva solicitud de mentoría
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MentorshipRequestDto>> CreateRequest([FromBody] CreateMentorshipRequestDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var request = await _mentorshipService.CreateRequestAsync(dto, userId.Value);
                return CreatedAtAction(nameof(GetRequest), new { id = request.Id }, request);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear solicitud de mentoría");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Elimina una solicitud
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                await _mentorshipService.DeleteRequestAsync(id, userId.Value);
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar solicitud con ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene las aplicaciones a una solicitud
        /// </summary>
        [HttpGet("{id}/applications")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<MentorApplicationDto>>> GetRequestApplications(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var applications = await _mentorshipService.GetRequestApplicationsAsync(id, userId.Value);
                return Ok(applications);
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
                _logger.LogError(ex, "Error al obtener aplicaciones de solicitud {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Aplica a una solicitud de mentoría
        /// </summary>
        [HttpPost("{id}/apply")]
        [Authorize(Roles = "Mentor,Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MentorApplicationDto>> ApplyToRequest(int id, [FromBody] CreateMentorApplicationDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                dto.RequestId = id;
                var application = await _mentorshipService.ApplyToRequestAsync(dto, userId.Value);
                return CreatedAtAction(nameof(GetRequestApplications), new { id }, application);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al aplicar a solicitud {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Acepta una aplicación de mentor
        /// </summary>
        [HttpPost("applications/{applicationId}/accept")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AcceptApplication(int applicationId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                await _mentorshipService.AcceptApplicationAsync(applicationId, userId.Value);
                return Ok(new { message = "Aplicación aceptada exitosamente" });
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
                _logger.LogError(ex, "Error al aceptar aplicación {ApplicationId}", applicationId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Retira una aplicación
        /// </summary>
        [HttpPost("applications/{applicationId}/withdraw")]
        [Authorize(Roles = "Mentor,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> WithdrawApplication(int applicationId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                await _mentorshipService.WithdrawApplicationAsync(applicationId, userId.Value);
                return Ok(new { message = "Aplicación retirada exitosamente" });
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
                _logger.LogError(ex, "Error al retirar aplicación {ApplicationId}", applicationId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}