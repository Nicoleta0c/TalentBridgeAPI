using Microsoft.EntityFrameworkCore;
using TalentBridge.Domain.Entities;

namespace TalentBridge.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<CV> CVs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.FullName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
                entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Id).ValueGeneratedOnAdd();
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

                // Relationships
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

                // Relationship
                entity.HasOne(c => c.User)
                      .WithMany()
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(c => c.Id).ValueGeneratedOnAdd();
            });
        }
    }
}