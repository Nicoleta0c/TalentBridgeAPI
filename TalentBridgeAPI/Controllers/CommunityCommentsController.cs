using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentBridge.API.DTOs.CommunityDTOs;
using TalentBridge.API.Interfaces.ICommunity;
using TalentBridge.Application.DTOs.CommentsDTOs;

namespace TalentBridge.API.Controllers
{
    [ApiController]
    [Route("api/posts/{postId}/comments")]
    public class CommunityCommentsController : ControllerBase
    {
        private readonly ICommunityCommentService _commentService;
        private readonly ILogger<CommunityCommentsController> _logger;

        public CommunityCommentsController(
            ICommunityCommentService commentService,
            ILogger<CommunityCommentsController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        /// <summary>
        /// Obtiene todos los comentarios de un post
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CommunityCommentDto>>> GetPostComments(int postId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var comments = await _commentService.GetByPostIdAsync(postId, userId);
                return Ok(comments);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener comentarios del post {PostId}", postId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene un comentario por ID
        /// </summary>
        [HttpGet("{commentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CommunityCommentDto>> GetComment(int postId, int commentId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var comment = await _commentService.GetByIdAsync(commentId, userId);

                if (comment == null)
                {
                    return NotFound(new { message = $"Comentario con ID {commentId} no encontrado" });
                }

                return Ok(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener comentario {CommentId}", commentId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crea un nuevo comentario en el post
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CommunityCommentDto>> CreateComment(int postId, [FromBody] CreateCommentDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                // Asegurar que el comentario pertenece al post especificado
                dto.PostId = postId;

                var comment = await _commentService.CreateAsync(dto, userId.Value);
                return CreatedAtAction(nameof(GetComment), new { postId, commentId = comment.Id }, comment);
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
                _logger.LogError(ex, "Error al crear comentario en post {PostId}", postId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza un comentario existente
        /// </summary>
        [HttpPut("{commentId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CommunityCommentDto>> UpdateComment(int postId, int commentId, [FromBody] UpdateCommentDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var comment = await _commentService.UpdateAsync(commentId, dto, userId.Value);
                return Ok(comment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar comentario {CommentId}", commentId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Elimina un comentario
        /// </summary>
        [HttpDelete("{commentId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteComment(int postId, int commentId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                await _commentService.DeleteAsync(commentId, userId.Value);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar comentario {CommentId}", commentId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Da like a un comentario
        /// </summary>
        [HttpPost("{commentId}/like")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LikeComment(int postId, int commentId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var liked = await _commentService.LikeCommentAsync(commentId, userId.Value);
                return Ok(new
                {
                    message = liked ? "Like agregado" : "Ya has dado like a este comentario",
                    liked
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al dar like a comentario {CommentId}", commentId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Remueve like de un comentario
        /// </summary>
        [HttpDelete("{commentId}/like")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UnlikeComment(int postId, int commentId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                var unliked = await _commentService.UnlikeCommentAsync(commentId, userId.Value);
                return Ok(new
                {
                    message = unliked ? "Like removido" : "No habías dado like a este comentario",
                    unliked
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al remover like de comentario {CommentId}", commentId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}