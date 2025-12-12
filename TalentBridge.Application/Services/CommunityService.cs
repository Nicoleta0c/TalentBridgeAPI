using TalentBridge.API.DTOs.CommunityDTOs;
using TalentBridge.API.Interfaces.ICommunity;
using TalentBridge.Domain.Entities;
using TalentBridge.Domain.Enums;

namespace TalentBridge.API.Services
{
    public class CommunityService : ICommunityService
    {
        private readonly ICommunityRepository _communityRepository;
        private readonly ICommunityMemberRepository _memberRepository;
        private readonly IUniversityRepository _universityRepository;

        public CommunityService(
            ICommunityRepository communityRepository,
            ICommunityMemberRepository memberRepository,
            IUniversityRepository universityRepository)
        {
            _communityRepository = communityRepository;
            _memberRepository = memberRepository;
            _universityRepository = universityRepository;
        }

        public async Task<CommunityDto?> GetByIdAsync(int id, int? currentUserId = null)
        {
            var community = await _communityRepository.GetByIdAsync(id);
            if (community == null) return null;

            return MapToCommunityDto(community);
        }

        public async Task<CommunityDetailDto?> GetDetailByIdAsync(int id, int? currentUserId = null)
        {
            var community = await _communityRepository.GetByIdWithDetailsAsync(id);
            if (community == null) return null;

            bool isUserMember = false;
            string? userRole = null;

            if (currentUserId.HasValue)
            {
                var membership = await _memberRepository.GetMembershipAsync(id, currentUserId.Value);
                isUserMember = membership?.IsActive ?? false;
                userRole = membership?.Role.ToString();
            }

            var dto = new CommunityDetailDto
            {
                Id = community.Id,
                Name = community.Name,
                Description = community.Description,
                ImageUrl = community.ImageUrl,
                UniversityId = community.UniversityId,
                UniversityName = community.University.Name,
                Purpose = community.Purpose,
                Rules = community.Rules,
                IsActive = community.IsActive,
                IsPrivate = community.IsPrivate,
                TotalMembers = community.TotalMembers,
                TotalPosts = community.TotalPosts,
                MaxMembers = community.MaxMembers,
                CreatedAt = community.CreatedAt,
                CreatedByUserId = community.CreatedByUserId,
                CreatedByName = community.CreatedBy.FullName,
                IsUserMember = isUserMember,
                UserRole = userRole,
                RecentMembers = community.Members
                    .Where(m => m.IsActive)
                    .OrderByDescending(m => m.JoinedAt)
                    .Take(10)
                    .Select(m => new CommunityMemberDto
                    {
                        Id = m.Id,
                        UserId = m.UserId,
                        UserName = m.User.FullName,
                        UserEmail = m.User.Email,
                        Role = m.Role.ToString(),
                        IsActive = m.IsActive,
                        JoinedAt = m.JoinedAt,
                        TotalPosts = m.TotalPosts,
                        TotalComments = m.TotalComments,
                        ReputationPoints = m.ReputationPoints
                    }).ToList(),
                RecentPosts = community.Posts
                    .Where(p => p.IsActive)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(10)
                    .Select(p => new CommunityPostDto
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Content = p.Content,
                        ImageUrl = p.ImageUrl,
                        CommunityId = p.CommunityId,
                        CommunityName = community.Name,
                        AuthorId = p.AuthorId,
                        AuthorName = p.Author.FullName,
                        Type = p.Type.ToString(),
                        ViewsCount = p.ViewsCount,
                        LikesCount = p.LikesCount,
                        CommentsCount = p.CommentsCount,
                        IsActive = p.IsActive,
                        IsPinned = p.IsPinned,
                        IsLocked = p.IsLocked,
                        CreatedAt = p.CreatedAt
                    }).ToList()
            };

            return dto;
        }

        public async Task<IEnumerable<CommunityDto>> GetAllAsync()
        {
            var communities = await _communityRepository.GetAllAsync();
            return communities.Select(MapToCommunityDto);
        }

        public async Task<IEnumerable<CommunityDto>> GetByUniversityIdAsync(int universityId)
        {
            var communities = await _communityRepository.GetByUniversityIdAsync(universityId);
            return communities.Select(MapToCommunityDto);
        }

        public async Task<IEnumerable<CommunityDto>> GetPublicCommunitiesAsync()
        {
            var communities = await _communityRepository.GetPublicCommunitiesAsync();
            return communities.Select(MapToCommunityDto);
        }

        public async Task<IEnumerable<CommunityDto>> GetUserCommunitiesAsync(int userId)
        {
            var communities = await _communityRepository.GetUserCommunitiesAsync(userId);
            return communities.Select(MapToCommunityDto);
        }

        public async Task<CommunityDto> CreateAsync(CreateCommunityDto dto, int userId)
        {
            // Verificar que la universidad existe
            if (!await _universityRepository.ExistsAsync(dto.UniversityId))
            {
                throw new KeyNotFoundException($"Universidad con ID {dto.UniversityId} no encontrada");
            }

            var community = new Community
            {
                Name = dto.Name,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                UniversityId = dto.UniversityId,
                Purpose = dto.Purpose,
                Rules = dto.Rules,
                MaxMembers = dto.MaxMembers,
                IsPrivate = dto.IsPrivate,
                IsActive = true,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _communityRepository.CreateAsync(community);

            // Agregar al creador como Owner
            var ownerMembership = new CommunityMember
            {
                CommunityId = created.Id,
                UserId = userId,
                Role = CommunityRole.Owner,
                IsActive = true,
                JoinedAt = DateTime.UtcNow
            };

            await _memberRepository.AddMemberAsync(ownerMembership);
            await _communityRepository.UpdateStatsAsync(created.Id);

            return MapToCommunityDto(await _communityRepository.GetByIdAsync(created.Id) ?? created);
        }

        public async Task<CommunityDto> UpdateAsync(int id, UpdateCommunityDto dto, int userId)
        {
            // ✅ Usar GetByIdForUpdateAsync para poder actualizar
            var community = await _communityRepository.GetByIdForUpdateAsync(id);
            if (community == null)
            {
                throw new KeyNotFoundException($"Comunidad con ID {id} no encontrada");
            }

            // Verificar permisos
            var isModeratorOrAbove = await _memberRepository.IsModeratorOrAboveAsync(id, userId);
            if (!isModeratorOrAbove && community.CreatedByUserId != userId)
            {
                throw new UnauthorizedAccessException("No tienes permisos para editar esta comunidad");
            }

            // Actualizar campos
            if (!string.IsNullOrEmpty(dto.Name))
                community.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Description))
                community.Description = dto.Description;

            if (!string.IsNullOrEmpty(dto.ImageUrl))
                community.ImageUrl = dto.ImageUrl;

            if (!string.IsNullOrEmpty(dto.Purpose))
                community.Purpose = dto.Purpose;

            if (!string.IsNullOrEmpty(dto.Rules))
                community.Rules = dto.Rules;

            if (dto.MaxMembers.HasValue)
                community.MaxMembers = dto.MaxMembers.Value;

            if (dto.IsPrivate.HasValue)
                community.IsPrivate = dto.IsPrivate.Value;

            var updated = await _communityRepository.UpdateAsync(community);
            return MapToCommunityDto(updated);
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            // Usar GetByIdForUpdateAsync para poder eliminar
            var community = await _communityRepository.GetByIdForUpdateAsync(id);
            if (community == null)
            {
                throw new KeyNotFoundException($"Comunidad con ID {id} no encontrada");
            }

            // Solo el owner puede eliminar
            if (community.CreatedByUserId != userId)
            {
                throw new UnauthorizedAccessException("Solo el creador puede eliminar esta comunidad");
            }

            return await _communityRepository.DeleteAsync(id);
        }

        public async Task<bool> JoinCommunityAsync(int communityId, int userId)
        {
            // ✅ Usar GetByIdForUpdateAsync para obtener la comunidad
            var community = await _communityRepository.GetByIdForUpdateAsync(communityId);
            if (community == null)
            {
                throw new KeyNotFoundException($"Comunidad con ID {communityId} no encontrada");
            }

            if (!community.IsActive)
            {
                throw new InvalidOperationException("La comunidad no está activa");
            }

            // Verificar si ya es miembro
            if (await _memberRepository.IsMemberAsync(communityId, userId))
            {
                throw new InvalidOperationException("Ya eres miembro de esta comunidad");
            }

            // Verificar límite de miembros
            if (community.MaxMembers > 0 && community.TotalMembers >= community.MaxMembers)
            {
                throw new InvalidOperationException("La comunidad ha alcanzado su límite de miembros");
            }

            var member = new CommunityMember
            {
                CommunityId = communityId,
                UserId = userId,
                Role = CommunityRole.Member,
                IsActive = true,
                JoinedAt = DateTime.UtcNow
            };

            await _memberRepository.AddMemberAsync(member);
            await _communityRepository.UpdateStatsAsync(communityId);

            return true;
        }

        public async Task<bool> LeaveCommunityAsync(int communityId, int userId)
        {
            var membership = await _memberRepository.GetMembershipAsync(communityId, userId);
            if (membership == null || !membership.IsActive)
            {
                throw new InvalidOperationException("No eres miembro de esta comunidad");
            }

            // El owner no puede abandonar, debe transferir ownership primero
            if (membership.Role == CommunityRole.Owner)
            {
                throw new InvalidOperationException("El dueño no puede abandonar la comunidad. Debe transferir la propiedad primero");
            }

            await _memberRepository.RemoveMemberAsync(membership.Id);
            await _communityRepository.UpdateStatsAsync(communityId);

            return true;
        }

        public async Task<IEnumerable<CommunityMemberDto>> GetMembersAsync(int communityId)
        {
            if (!await _communityRepository.ExistsAsync(communityId))
            {
                throw new KeyNotFoundException($"Comunidad con ID {communityId} no encontrada");
            }

            var members = await _memberRepository.GetCommunityMembersAsync(communityId);

            return members.Select(m => new CommunityMemberDto
            {
                Id = m.Id,
                UserId = m.UserId,
                UserName = m.User.FullName,
                UserEmail = m.User.Email,
                Role = m.Role.ToString(),
                IsActive = m.IsActive,
                JoinedAt = m.JoinedAt,
                TotalPosts = m.TotalPosts,
                TotalComments = m.TotalComments,
                ReputationPoints = m.ReputationPoints
            });
        }

        public async Task<bool> UpdateMemberRoleAsync(int communityId, int memberId, string role, int currentUserId)
        {
            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member == null || member.CommunityId != communityId)
            {
                throw new KeyNotFoundException("Miembro no encontrado");
            }

            // Verificar permisos del usuario actual
            var currentUserRole = await _memberRepository.GetUserRoleAsync(communityId, currentUserId);
            if (currentUserRole == null || currentUserRole < CommunityRole.Admin)
            {
                throw new UnauthorizedAccessException("No tienes permisos para cambiar roles");
            }

            // Parsear el nuevo rol
            if (!Enum.TryParse<CommunityRole>(role, true, out var newRole))
            {
                throw new ArgumentException("Rol inválido");
            }

            // No se puede cambiar el rol del owner
            if (member.Role == CommunityRole.Owner)
            {
                throw new InvalidOperationException("No se puede cambiar el rol del dueño");
            }

            member.Role = newRole;
            await _memberRepository.UpdateMemberAsync(member);

            return true;
        }

        public async Task<bool> RemoveMemberAsync(int communityId, int memberId, int currentUserId)
        {
            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member == null || member.CommunityId != communityId)
            {
                throw new KeyNotFoundException("Miembro no encontrado");
            }

            // Verificar permisos
            var isModeratorOrAbove = await _memberRepository.IsModeratorOrAboveAsync(communityId, currentUserId);
            if (!isModeratorOrAbove)
            {
                throw new UnauthorizedAccessException("No tienes permisos para remover miembros");
            }

            // No se puede remover al owner
            if (member.Role == CommunityRole.Owner)
            {
                throw new InvalidOperationException("No se puede remover al dueño");
            }

            await _memberRepository.RemoveMemberAsync(memberId);
            await _communityRepository.UpdateStatsAsync(communityId);

            return true;
        }

        private CommunityDto MapToCommunityDto(Community community)
        {
            return new CommunityDto
            {
                Id = community.Id,
                Name = community.Name,
                Description = community.Description,
                ImageUrl = community.ImageUrl,
                UniversityId = community.UniversityId,
                UniversityName = community.University.Name,
                IsActive = community.IsActive,
                IsPrivate = community.IsPrivate,
                TotalMembers = community.TotalMembers,
                TotalPosts = community.TotalPosts,
                MaxMembers = community.MaxMembers,
                CreatedAt = community.CreatedAt,
                CreatedByName = community.CreatedBy.FullName
            };
        }
    }
}