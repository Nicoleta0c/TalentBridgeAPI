using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;
using TalentBridge.Domain.Entities;

namespace TalentBridge.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<CV> CVs { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<University> Universities { get; set; }
        public DbSet<UniversityCareer> UniversityCareers { get; set; }
        public DbSet<Community> Communities { get; set; }
        public DbSet<CommunityMember> CommunityMembers { get; set; }
        public DbSet<CommunityPost> CommunityPosts { get; set; }
        public DbSet<CommunityComment> CommunityComments { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<CommentLike> CommentLikes { get; set; }
        public DbSet<Mentorship> Mentorships { get; set; }
        public DbSet<MentorshipSession> MentorshipSessions { get; set; }
        public DbSet<SessionAttendance> SessionAttendances { get; set; }
        public DbSet<MentorshipMilestone> MentorshipMilestones { get; set; }
        public DbSet<MentorshipResource> MentorshipResources { get; set; }
        public DbSet<MentorshipRequest> MentorshipRequests { get; set; }
        public DbSet<MentorApplication> MentorApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Función para crear ValueComparer para List<string>
            ValueComparer<List<string>> CreateListComparer()
            {
                return new ValueComparer<List<string>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v != null ? v.GetHashCode() : 0)),
                    c => c.ToList()
                );
            }

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.FullName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
                entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);
                entity.Property(u => u.Role).IsRequired().HasMaxLength(50).HasDefaultValue("User");
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Id).ValueGeneratedOnAdd();

                // Mentorship properties
                entity.Property(u => u.MentorTitle).HasMaxLength(100);
                entity.Property(u => u.MentorBio).HasMaxLength(2000);
                entity.Property(u => u.HourlyRate).HasColumnType("decimal(10,2)");
                entity.Property(u => u.MentorAverageRating).HasColumnType("decimal(3,2)");
                entity.Property(u => u.Timezone).HasMaxLength(50);

                // Lists converted to JSON con ValueComparer
                entity.Property(u => u.MentorExpertise)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
                    )
                    .Metadata.SetValueComparer(CreateListComparer());

                entity.Property(u => u.MentorIndustries)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
                    )
                    .Metadata.SetValueComparer(CreateListComparer());

                entity.Property(u => u.PreferredMentorshipCategories)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
                    )
                    .Metadata.SetValueComparer(CreateListComparer());
            });

            // RefreshToken configuration
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);
                entity.Property(rt => rt.Token).IsRequired().HasMaxLength(500);
                entity.HasIndex(rt => rt.Token).IsUnique();

                entity.HasOne(rt => rt.User)
                    .WithMany()
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(rt => rt.Id).ValueGeneratedOnAdd();
            });

            // Job configuration
            modelBuilder.Entity<Job>(entity =>
            {
                entity.HasKey(j => j.Id);
                entity.Property(j => j.Title).IsRequired().HasMaxLength(200);
                entity.Property(j => j.Company).IsRequired().HasMaxLength(100);
                entity.Property(j => j.Description).IsRequired();
                entity.Property(j => j.Requirements).IsRequired();
                entity.Property(j => j.Location).IsRequired().HasMaxLength(100);
                entity.Property(j => j.SalaryRange).HasMaxLength(100);
                entity.Property(j => j.JobType).IsRequired().HasMaxLength(50);
                entity.Property(j => j.Id).ValueGeneratedOnAdd();
            });

            // JobApplication configuration
            modelBuilder.Entity<JobApplication>(entity =>
            {
                entity.HasKey(ja => ja.Id);
                entity.Property(ja => ja.Status).IsRequired().HasMaxLength(50);
                entity.Property(ja => ja.CoverLetter).HasMaxLength(1000);

                entity.HasOne(ja => ja.Job)
                    .WithMany()
                    .HasForeignKey(ja => ja.JobId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ja => ja.User)
                    .WithMany()
                    .HasForeignKey(ja => ja.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(ja => ja.Id).ValueGeneratedOnAdd();
            });

            // CV configuration
            modelBuilder.Entity<CV>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.FileName).IsRequired().HasMaxLength(255);
                entity.Property(c => c.FilePath).IsRequired().HasMaxLength(500);
                entity.Property(c => c.FileType).IsRequired().HasMaxLength(10);
                entity.Property(c => c.AnalysisResult).HasColumnType("TEXT");
                entity.Property(c => c.Score).HasColumnType("DECIMAL(5,2)");

                entity.HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(c => c.Id).ValueGeneratedOnAdd();
            });

            // University configuration
            modelBuilder.Entity<University>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Name).IsRequired().HasMaxLength(200);
                entity.Property(u => u.Acronym).IsRequired().HasMaxLength(20);
                entity.HasIndex(u => u.Acronym).IsUnique();
                entity.Property(u => u.Email).HasMaxLength(100);

                entity.HasMany(u => u.Communities)
                    .WithOne(c => c.University)
                    .HasForeignKey(c => c.UniversityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.Students)
                    .WithOne(s => s.University)
                    .HasForeignKey(s => s.UniversityId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // UniversityCareer configuration
            modelBuilder.Entity<UniversityCareer>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
                entity.Property(c => c.Code).IsRequired().HasMaxLength(50);

                entity.HasOne(c => c.University)
                    .WithMany(u => u.Careers)
                    .HasForeignKey(c => c.UniversityId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Community configuration
            modelBuilder.Entity<Community>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
                entity.Property(c => c.Description).IsRequired().HasMaxLength(1000);
                entity.Property(c => c.ImageUrl).HasMaxLength(500);
                entity.Property(c => c.Purpose).HasMaxLength(500);
                entity.Property(c => c.Rules).HasMaxLength(2000);

                entity.HasOne(c => c.University)
                    .WithMany(u => u.Communities)
                    .HasForeignKey(c => c.UniversityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.CreatedBy)
                    .WithMany(u => u.CreatedCommunities)
                    .HasForeignKey(c => c.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(c => c.Members)
                    .WithOne(m => m.Community)
                    .HasForeignKey(m => m.CommunityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.Posts)
                    .WithOne(p => p.Community)
                    .HasForeignKey(p => p.CommunityId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // CommunityMember configuration
            modelBuilder.Entity<CommunityMember>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Role).HasConversion<string>();

                entity.HasIndex(m => new { m.CommunityId, m.UserId })
                    .IsUnique()
                    .HasFilter("[IsActive] = 1");

                entity.HasOne(m => m.Community)
                    .WithMany(c => c.Members)
                    .HasForeignKey(m => m.CommunityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(m => m.User)
                    .WithMany(u => u.CommunityMemberships)
                    .HasForeignKey(m => m.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // CommunityPost configuration
            modelBuilder.Entity<CommunityPost>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Title).IsRequired().HasMaxLength(300);
                entity.Property(p => p.Content).IsRequired().HasMaxLength(10000);
                entity.Property(p => p.ImageUrl).HasMaxLength(500);
                entity.Property(p => p.AttachmentUrl).HasMaxLength(500);
                entity.Property(p => p.Type).HasConversion<string>();

                entity.HasOne(p => p.Community)
                    .WithMany(c => c.Posts)
                    .HasForeignKey(p => p.CommunityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Author)
                    .WithMany(u => u.CommunityPosts)
                    .HasForeignKey(p => p.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // CommunityComment configuration
            modelBuilder.Entity<CommunityComment>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Content).IsRequired().HasMaxLength(5000);

                entity.HasOne(c => c.Post)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(c => c.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Author)
                    .WithMany(u => u.CommunityComments)
                    .HasForeignKey(c => c.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.ParentComment)
                    .WithMany(p => p.Replies)
                    .HasForeignKey(c => c.ParentCommentId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // PostLike configuration
            modelBuilder.Entity<PostLike>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.HasIndex(l => new { l.PostId, l.UserId }).IsUnique();

                entity.HasOne(l => l.Post)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(l => l.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(l => l.User)
                    .WithMany(u => u.PostLikes)
                    .HasForeignKey(l => l.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // CommentLike configuration
            modelBuilder.Entity<CommentLike>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.HasIndex(l => new { l.CommentId, l.UserId }).IsUnique();

                entity.HasOne(l => l.Comment)
                    .WithMany(c => c.Likes)
                    .HasForeignKey(l => l.CommentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(l => l.User)
                    .WithMany(u => u.CommentLikes)
                    .HasForeignKey(l => l.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Mentorship configuration
            modelBuilder.Entity<Mentorship>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Title).IsRequired().HasMaxLength(200);
                entity.Property(m => m.Description).IsRequired().HasMaxLength(2000);
                entity.Property(m => m.Goals).HasMaxLength(1000);
                entity.Property(m => m.Expectations).HasMaxLength(1000);
                entity.Property(m => m.Category).IsRequired().HasMaxLength(100);
                entity.Property(m => m.Status).HasConversion<string>();
                entity.Property(m => m.MeetingMethod).HasConversion<string>();
                entity.Property(m => m.MentorFeedback).HasMaxLength(2000);
                entity.Property(m => m.MenteeFeedback).HasMaxLength(2000);

                // Tags converted to JSON con ValueComparer
                entity.Property(m => m.Tags)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
                    )
                    .Metadata.SetValueComparer(CreateListComparer());

                entity.HasOne(m => m.Mentor)
                    .WithMany(u => u.MentorMentorships)
                    .HasForeignKey(m => m.MentorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Mentee)
                    .WithMany(u => u.MenteeMentorships)
                    .HasForeignKey(m => m.MenteeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(m => m.Sessions)
                    .WithOne(s => s.Mentorship)
                    .HasForeignKey(s => s.MentorshipId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(m => m.Milestones)
                    .WithOne(mil => mil.Mentorship)
                    .HasForeignKey(mil => mil.MentorshipId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(m => m.Resources)
                    .WithOne(r => r.Mentorship)
                    .HasForeignKey(r => r.MentorshipId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // MentorshipSession configuration
            modelBuilder.Entity<MentorshipSession>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Title).IsRequired().HasMaxLength(200);
                entity.Property(s => s.Description).IsRequired().HasMaxLength(1000);
                entity.Property(s => s.Agenda).HasMaxLength(2000);
                entity.Property(s => s.Notes).HasMaxLength(4000);
                entity.Property(s => s.Homework).HasMaxLength(2000);
                entity.Property(s => s.MeetingLink).HasMaxLength(500);
                entity.Property(s => s.MeetingId).HasMaxLength(100);
                entity.Property(s => s.Status).HasConversion<string>();
                entity.Property(s => s.MeetingMethod).HasConversion<string>();

                entity.HasOne(s => s.Mentorship)
                    .WithMany(m => m.Sessions)
                    .HasForeignKey(s => s.MentorshipId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(s => s.Attendances)
                    .WithOne(a => a.Session)
                    .HasForeignKey(a => a.SessionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // SessionAttendance configuration
            modelBuilder.Entity<SessionAttendance>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Feedback).HasMaxLength(1000);
                entity.Property(a => a.Status).HasConversion<string>();

                entity.HasIndex(a => new { a.SessionId, a.UserId }).IsUnique();

                entity.HasOne(a => a.Session)
                    .WithMany(s => s.Attendances)
                    .HasForeignKey(a => a.SessionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.User)
                    .WithMany(u => u.SessionAttendances)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // MentorshipMilestone configuration
            modelBuilder.Entity<MentorshipMilestone>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Title).IsRequired().HasMaxLength(200);
                entity.Property(m => m.Description).IsRequired().HasMaxLength(1000);
                entity.Property(m => m.Notes).HasMaxLength(2000);
                entity.Property(m => m.Evidence).HasMaxLength(500);
                entity.Property(m => m.Status).HasConversion<string>();

                entity.HasOne(m => m.Mentorship)
                    .WithMany(ms => ms.Milestones)
                    .HasForeignKey(m => m.MentorshipId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // MentorshipResource configuration
            modelBuilder.Entity<MentorshipResource>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Title).IsRequired().HasMaxLength(200);
                entity.Property(r => r.Description).HasMaxLength(1000);
                entity.Property(r => r.Url).IsRequired().HasMaxLength(500);
                entity.Property(r => r.ThumbnailUrl).HasMaxLength(500);
                entity.Property(r => r.Type).HasConversion<string>();
                entity.Property(r => r.IsPublic).HasDefaultValue(false);
                entity.Property(r => r.IsDownloadable).HasDefaultValue(true);
                entity.Property(r => r.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP"); // Cambiado para SQLite

                entity.HasOne(r => r.Mentorship)
                    .WithMany(m => m.Resources)
                    .HasForeignKey(r => r.MentorshipId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Session)
                    .WithMany()
                    .HasForeignKey(r => r.SessionId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(r => r.CreatedBy)
                    .WithMany(u => u.CreatedResources)
                    .HasForeignKey(r => r.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // MentorshipRequest configuration
            modelBuilder.Entity<MentorshipRequest>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Title).IsRequired().HasMaxLength(200);
                entity.Property(r => r.Description).IsRequired().HasMaxLength(2000);
                entity.Property(r => r.Goals).IsRequired().HasMaxLength(1000);
                entity.Property(r => r.Category).IsRequired().HasMaxLength(100);
                entity.Property(r => r.RejectionReason).HasMaxLength(500);
                entity.Property(r => r.Status).HasConversion<string>();
                entity.Property(r => r.PreferredMeetingMethod).HasConversion<string>();

                entity.Property(r => r.Tags)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
                    )
                    .Metadata.SetValueComparer(CreateListComparer());

                entity.HasOne(r => r.Mentee)
                    .WithMany(u => u.MentorshipRequests)
                    .HasForeignKey(r => r.MenteeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.ProposedMentorship)
                    .WithMany()
                    .HasForeignKey(r => r.ProposedMentorshipId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(r => r.MentorApplications)
                    .WithOne(a => a.Request)
                    .HasForeignKey(a => a.RequestId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // MentorApplication configuration
            modelBuilder.Entity<MentorApplication>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Proposal).IsRequired().HasMaxLength(2000);
                entity.Property(a => a.Experience).HasMaxLength(2000);
                entity.Property(a => a.WhyChooseMe).HasMaxLength(2000);
                entity.Property(a => a.RejectionReason).HasMaxLength(500);
                entity.Property(a => a.Status).HasConversion<string>();
                entity.Property(a => a.ProposedMeetingMethod).HasConversion<string>();

                entity.HasIndex(a => new { a.RequestId, a.MentorId }).IsUnique();

                entity.HasOne(a => a.Request)
                    .WithMany(r => r.MentorApplications)
                    .HasForeignKey(a => a.RequestId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Mentor)
                    .WithMany(u => u.MentorApplications)
                    .HasForeignKey(a => a.MentorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}