using TalentBridge.API.DTOs.CommunityDTOs;
using TalentBridge.API.Interfaces.ICommunity;
using TalentBridge.Application.DTOs.CommentsDTOs;
using TalentBridge.Domain.Entities;

namespace TalentBridge.API.Services
{
    public class CommunityCommentService : ICommunityCommentService
    {
        private readonly ICommunityCommentRepository _commentRepository;
        private readonly ICommunityPostRepository _postRepository;
        private readonly ICommunityMemberRepository _memberRepository;
        private readonly ICommentLikeRepository _likeRepository;

        public CommunityCommentService(
            ICommunityCommentRepository commentRepository,
            ICommunityPostRepository postRepository,
            ICommunityMemberRepository memberRepository,
            ICommentLikeRepository likeRepository)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _memberRepository = memberRepository;
            _likeRepository = likeRepository;
        }

        public async Task<CommunityCommentDto?> GetByIdAsync(int id, int? currentUserId = null)
        {
            var comment = await _commentRepository.GetByIdWithDetailsAsync(id);
            if (comment == null) return null;

            bool isLiked = false;
            if (currentUserId.HasValue)
            {
                isLiked = await _likeRepository.HasUserLikedAsync(id, currentUserId.Value);
            }

            return await MapToCommentDto(comment, currentUserId, isLiked);
        }

        public async Task<IEnumerable<CommunityCommentDto>> GetByPostIdAsync(int postId, int? currentUserId = null)
        {
            if (!await _postRepository.ExistsAsync(postId))
            {
                throw new KeyNotFoundException($"Post con ID {postId} no encontrado");
            }

            var comments = await _commentRepository.GetByPostIdAsync(postId);
            var dtos = new List<CommunityCommentDto>();

            foreach (var comment in comments)
            {
                bool isLiked = false;
                if (currentUserId.HasValue)
                {
                    isLiked = await _likeRepository.HasUserLikedAsync(comment.Id, currentUserId.Value);
                }
                dtos.Add(await MapToCommentDto(comment, currentUserId, isLiked));
            }

            return dtos;
        }

        public async Task<CommunityCommentDto> CreateAsync(CreateCommentDto dto, int userId)
        {
            var post = await _postRepository.GetByIdAsync(dto.PostId);
            if (post == null)
            {
                throw new KeyNotFoundException($"Post con ID {dto.PostId} no encontrado");
            }

            // Verificar que el post no esté bloqueado
            if (post.IsLocked)
            {
                throw new InvalidOperationException("El post está bloqueado y no permite comentarios");
            }

            // Verificar que el usuario es miembro de la comunidad
            if (!await _memberRepository.IsMemberAsync(post.CommunityId, userId))
            {
                throw new UnauthorizedAccessException("Debes ser miembro de la comunidad para comentar");
            }

            // Si es respuesta, verificar que el comentario padre existe
            if (dto.ParentCommentId.HasValue)
            {
                if (!await _commentRepository.ExistsAsync(dto.ParentCommentId.Value))
                {
                    throw new KeyNotFoundException($"Comentario padre con ID {dto.ParentCommentId.Value} no encontrado");
                }
            }

            var comment = new CommunityComment
            {
                Content = dto.Content,
                PostId = dto.PostId,
                AuthorId = userId,
                ParentCommentId = dto.ParentCommentId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _commentRepository.CreateAsync(comment);

            // Actualizar contador de comentarios del post
            await _postRepository.UpdateCommentsCountAsync(dto.PostId);

            // Actualizar estadísticas del miembro
            var membership = await _memberRepository.GetMembershipAsync(post.CommunityId, userId);
            if (membership != null)
            {
                membership.TotalComments++;
                await _memberRepository.UpdateMemberAsync(membership);
            }

            return await MapToCommentDto(await _commentRepository.GetByIdAsync(created.Id) ?? created, userId, false);
        }

        public async Task<CommunityCommentDto> UpdateAsync(int id, UpdateCommentDto dto, int userId)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                throw new KeyNotFoundException($"Comentario con ID {id} no encontrado");
            }

            // Solo el autor puede editar su comentario
            if (comment.AuthorId != userId)
            {
                throw new UnauthorizedAccessException("No tienes permisos para editar este comentario");
            }

            comment.Content = dto.Content;
            var updated = await _commentRepository.UpdateAsync(comment);

            bool isLiked = await _likeRepository.HasUserLikedAsync(id, userId);
            return await MapToCommentDto(updated, userId, isLiked);
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                throw new KeyNotFoundException($"Comentario con ID {id} no encontrado");
            }

            var post = await _postRepository.GetByIdAsync(comment.PostId);
            if (post == null)
            {
                throw new KeyNotFoundException("Post no encontrado");
            }

            // Verificar permisos: autor o moderador
            var isModerator = await _memberRepository.IsModeratorOrAboveAsync(post.CommunityId, userId);
            if (comment.AuthorId != userId && !isModerator)
            {
                throw new UnauthorizedAccessException("No tienes permisos para eliminar este comentario");
            }

            var result = await _commentRepository.DeleteAsync(id);

            // Actualizar contador de comentarios del post
            await _postRepository.UpdateCommentsCountAsync(comment.PostId);

            // Actualizar estadísticas del miembro
            var membership = await _memberRepository.GetMembershipAsync(post.CommunityId, userId);
            if (membership != null && membership.TotalComments > 0)
            {
                membership.TotalComments--;
                await _memberRepository.UpdateMemberAsync(membership);
            }

            return result;
        }

        public async Task<bool> LikeCommentAsync(int commentId, int userId)
        {
            if (!await _commentRepository.ExistsAsync(commentId))
            {
                throw new KeyNotFoundException($"Comentario con ID {commentId} no encontrado");
            }

            // Verificar si ya dio like
            if (await _likeRepository.HasUserLikedAsync(commentId, userId))
            {
                return false; // Ya tiene like
            }

            var like = new CommentLike
            {
                CommentId = commentId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _likeRepository.AddLikeAsync(like);
            await _commentRepository.UpdateLikesCountAsync(commentId);

            return true;
        }

        public async Task<bool> UnlikeCommentAsync(int commentId, int userId)
        {
            if (!await _commentRepository.ExistsAsync(commentId))
            {
                throw new KeyNotFoundException($"Comentario con ID {commentId} no encontrado");
            }

            var result = await _likeRepository.RemoveLikeAsync(commentId, userId);
            if (result)
            {
                await _commentRepository.UpdateLikesCountAsync(commentId);
            }

            return result;
        }

        private async Task<CommunityCommentDto> MapToCommentDto(CommunityComment comment, int? currentUserId, bool isLiked)
        {
            var replies = new List<CommunityCommentDto>();

            foreach (var reply in comment.Replies.Where(r => r.IsActive))
            {
                bool replyIsLiked = false;
                if (currentUserId.HasValue)
                {
                    replyIsLiked = await _likeRepository.HasUserLikedAsync(reply.Id, currentUserId.Value);
                }
                replies.Add(await MapToCommentDto(reply, currentUserId, replyIsLiked));
            }

            return new CommunityCommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                PostId = comment.PostId,
                AuthorId = comment.AuthorId,
                AuthorName = comment.Author.FullName,
                ParentCommentId = comment.ParentCommentId,
                LikesCount = comment.LikesCount,
                IsActive = comment.IsActive,
                IsEdited = comment.IsEdited,
                IsLikedByCurrentUser = isLiked,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                Replies = replies
            };
        }
    }
}