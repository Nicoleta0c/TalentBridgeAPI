using TalentBridge.API.DTOs.CommunityDTOs;
using TalentBridge.API.Interfaces.ICommunity;
using TalentBridge.Application.DTOs.CommentsDTOs;
using TalentBridge.Domain.Entities;
using TalentBridge.Domain.Enums;

namespace TalentBridge.API.Services
{
    public class CommunityPostService : ICommunityPostService
    {
        private readonly ICommunityPostRepository _postRepository;
        private readonly ICommunityMemberRepository _memberRepository;
        private readonly ICommunityRepository _communityRepository;
        private readonly IPostLikeRepository _likeRepository;

        public CommunityPostService(
            ICommunityPostRepository postRepository,
            ICommunityMemberRepository memberRepository,
            ICommunityRepository communityRepository,
            IPostLikeRepository likeRepository)
        {
            _postRepository = postRepository;
            _memberRepository = memberRepository;
            _communityRepository = communityRepository;
            _likeRepository = likeRepository;
        }

        public async Task<CommunityPostDto?> GetByIdAsync(int id, int? currentUserId = null)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null) return null;

            // Incrementar vistas
            await _postRepository.IncrementViewsAsync(id);

            bool isLiked = false;
            if (currentUserId.HasValue)
            {
                isLiked = await _likeRepository.HasUserLikedAsync(id, currentUserId.Value);
            }

            return MapToPostDto(post, isLiked);
        }

        public async Task<PostDetailDto?> GetDetailByIdAsync(int id, int? currentUserId = null)
        {
            var post = await _postRepository.GetByIdWithDetailsAsync(id);
            if (post == null) return null;

            // Incrementar vistas
            await _postRepository.IncrementViewsAsync(id);

            bool isLiked = false;
            if (currentUserId.HasValue)
            {
                isLiked = await _likeRepository.HasUserLikedAsync(id, currentUserId.Value);
            }

            var dto = new PostDetailDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                AttachmentUrl = post.AttachmentUrl,
                CommunityId = post.CommunityId,
                CommunityName = post.Community.Name,
                AuthorId = post.AuthorId,
                AuthorName = post.Author.FullName,
                Type = post.Type.ToString(),
                ViewsCount = post.ViewsCount,
                LikesCount = post.LikesCount,
                CommentsCount = post.CommentsCount,
                IsActive = post.IsActive,
                IsPinned = post.IsPinned,
                IsLocked = post.IsLocked,
                IsLikedByCurrentUser = isLiked,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                Comments = await MapCommentsAsync(post.Comments.Where(c => c.ParentCommentId == null).ToList(), currentUserId)
            };

            return dto;
        }

        public async Task<IEnumerable<CommunityPostDto>> GetByCommunityIdAsync(int communityId, int? currentUserId = null)
        {
            if (!await _communityRepository.ExistsAsync(communityId))
            {
                throw new KeyNotFoundException($"Comunidad con ID {communityId} no encontrada");
            }

            var posts = await _postRepository.GetByCommunityIdAsync(communityId);
            var dtos = new List<CommunityPostDto>();

            foreach (var post in posts)
            {
                bool isLiked = false;
                if (currentUserId.HasValue)
                {
                    isLiked = await _likeRepository.HasUserLikedAsync(post.Id, currentUserId.Value);
                }
                dtos.Add(MapToPostDto(post, isLiked));
            }

            return dtos;
        }

        public async Task<IEnumerable<CommunityPostDto>> GetByAuthorIdAsync(int authorId, int? currentUserId = null)
        {
            var posts = await _postRepository.GetByAuthorIdAsync(authorId);
            var dtos = new List<CommunityPostDto>();

            foreach (var post in posts)
            {
                bool isLiked = false;
                if (currentUserId.HasValue)
                {
                    isLiked = await _likeRepository.HasUserLikedAsync(post.Id, currentUserId.Value);
                }
                dtos.Add(MapToPostDto(post, isLiked));
            }

            return dtos;
        }

        public async Task<CommunityPostDto> CreateAsync(CreatePostDto dto, int userId)
        {
            // Verificar que la comunidad existe
            if (!await _communityRepository.ExistsAsync(dto.CommunityId))
            {
                throw new KeyNotFoundException($"Comunidad con ID {dto.CommunityId} no encontrada");
            }

            // Verificar que el usuario es miembro
            if (!await _memberRepository.IsMemberAsync(dto.CommunityId, userId))
            {
                throw new UnauthorizedAccessException("Debes ser miembro de la comunidad para publicar");
            }

            // Parsear el tipo de post
            if (!Enum.TryParse<PostType>(dto.Type, true, out var postType))
            {
                throw new ArgumentException("Tipo de post inválido");
            }

            var post = new CommunityPost
            {
                Title = dto.Title,
                Content = dto.Content,
                ImageUrl = dto.ImageUrl,
                AttachmentUrl = dto.AttachmentUrl,
                CommunityId = dto.CommunityId,
                AuthorId = userId,
                Type = postType,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _postRepository.CreateAsync(post);

            // Actualizar estadísticas de la comunidad
            await _communityRepository.UpdateStatsAsync(dto.CommunityId);

            // Actualizar estadísticas del miembro
            var membership = await _memberRepository.GetMembershipAsync(dto.CommunityId, userId);
            if (membership != null)
            {
                membership.TotalPosts++;
                await _memberRepository.UpdateMemberAsync(membership);
            }

            return MapToPostDto(await _postRepository.GetByIdAsync(created.Id) ?? created, false);
        }

        public async Task<CommunityPostDto> UpdateAsync(int id, UpdatePostDto dto, int userId)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
            {
                throw new KeyNotFoundException($"Post con ID {id} no encontrado");
            }

            // Verificar permisos: solo el autor o moderadores pueden editar
            var isModerator = await _memberRepository.IsModeratorOrAboveAsync(post.CommunityId, userId);
            if (post.AuthorId != userId && !isModerator)
            {
                throw new UnauthorizedAccessException("No tienes permisos para editar este post");
            }

            // Actualizar campos
            if (!string.IsNullOrEmpty(dto.Title))
                post.Title = dto.Title;

            if (!string.IsNullOrEmpty(dto.Content))
                post.Content = dto.Content;

            if (dto.ImageUrl != null)
                post.ImageUrl = dto.ImageUrl;

            if (dto.AttachmentUrl != null)
                post.AttachmentUrl = dto.AttachmentUrl;

            var updated = await _postRepository.UpdateAsync(post);

            bool isLiked = await _likeRepository.HasUserLikedAsync(id, userId);
            return MapToPostDto(updated, isLiked);
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
            {
                throw new KeyNotFoundException($"Post con ID {id} no encontrado");
            }

            // Verificar permisos
            var isModerator = await _memberRepository.IsModeratorOrAboveAsync(post.CommunityId, userId);
            if (post.AuthorId != userId && !isModerator)
            {
                throw new UnauthorizedAccessException("No tienes permisos para eliminar este post");
            }

            var result = await _postRepository.DeleteAsync(id);

            // Actualizar estadísticas
            await _communityRepository.UpdateStatsAsync(post.CommunityId);

            var membership = await _memberRepository.GetMembershipAsync(post.CommunityId, userId);
            if (membership != null && membership.TotalPosts > 0)
            {
                membership.TotalPosts--;
                await _memberRepository.UpdateMemberAsync(membership);
            }

            return result;
        }

        public async Task<bool> TogglePinAsync(int id, int userId)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
            {
                throw new KeyNotFoundException($"Post con ID {id} no encontrado");
            }

            // Solo moderadores o superiores pueden fijar posts
            if (!await _memberRepository.IsModeratorOrAboveAsync(post.CommunityId, userId))
            {
                throw new UnauthorizedAccessException("No tienes permisos para fijar posts");
            }

            post.IsPinned = !post.IsPinned;
            await _postRepository.UpdateAsync(post);

            return post.IsPinned;
        }

        public async Task<bool> ToggleLockAsync(int id, int userId)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
            {
                throw new KeyNotFoundException($"Post con ID {id} no encontrado");
            }

            // Solo moderadores o superiores pueden bloquear posts
            if (!await _memberRepository.IsModeratorOrAboveAsync(post.CommunityId, userId))
            {
                throw new UnauthorizedAccessException("No tienes permisos para bloquear posts");
            }

            post.IsLocked = !post.IsLocked;
            await _postRepository.UpdateAsync(post);

            return post.IsLocked;
        }

        public async Task<bool> LikePostAsync(int postId, int userId)
        {
            if (!await _postRepository.ExistsAsync(postId))
            {
                throw new KeyNotFoundException($"Post con ID {postId} no encontrado");
            }

            // Verificar si ya dio like
            if (await _likeRepository.HasUserLikedAsync(postId, userId))
            {
                return false; // Ya tiene like
            }

            var like = new PostLike
            {
                PostId = postId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _likeRepository.AddLikeAsync(like);
            await _postRepository.UpdateLikesCountAsync(postId);

            return true;
        }

        public async Task<bool> UnlikePostAsync(int postId, int userId)
        {
            if (!await _postRepository.ExistsAsync(postId))
            {
                throw new KeyNotFoundException($"Post con ID {postId} no encontrado");
            }

            var result = await _likeRepository.RemoveLikeAsync(postId, userId);
            if (result)
            {
                await _postRepository.UpdateLikesCountAsync(postId);
            }

            return result;
        }

        private CommunityPostDto MapToPostDto(CommunityPost post, bool isLiked)
        {
            return new CommunityPostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                AttachmentUrl = post.AttachmentUrl,
                CommunityId = post.CommunityId,
                CommunityName = post.Community.Name,
                AuthorId = post.AuthorId,
                AuthorName = post.Author.FullName,
                Type = post.Type.ToString(),
                ViewsCount = post.ViewsCount,
                LikesCount = post.LikesCount,
                CommentsCount = post.CommentsCount,
                IsActive = post.IsActive,
                IsPinned = post.IsPinned,
                IsLocked = post.IsLocked,
                IsLikedByCurrentUser = isLiked,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt
            };
        }

        private async Task<List<CommunityCommentDto>> MapCommentsAsync(List<CommunityComment> comments, int? currentUserId)
        {
            var dtos = new List<CommunityCommentDto>();

            foreach (var comment in comments)
            {
                bool isLiked = false;
                if (currentUserId.HasValue)
                {
                    var commentLikeRepo = _likeRepository as ICommentLikeRepository;
                    // Nota: Necesitarás inyectar ICommentLikeRepository también
                }

                var dto = new CommunityCommentDto
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
                    Replies = await MapCommentsAsync(comment.Replies.ToList(), currentUserId)
                };

                dtos.Add(dto);
            }

            return dtos;
        }
    }
}