using Doctor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Doctor.Infrastructure.Data
{
    public class DoctorDbContext : DbContext
    {
        public DoctorDbContext(DbContextOptions<DoctorDbContext> options)
            : base(options)
        {
        }

        public DbSet<Domain.Entities.Doctor> Doctors { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Doctor Configuration
            modelBuilder.Entity<Domain.Entities.Doctor>(entity =>
            {
                entity.HasIndex(e => new { e.FirstName, e.LastName });
                entity.HasIndex(e => e.LicenseNumber).IsUnique();
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Bio).HasMaxLength(500);
                entity.Property(e => e.Qualifications).HasMaxLength(500);
                entity.Property(e => e.LicenseNumber).HasMaxLength(50);
                entity.Property(e => e.Department).HasMaxLength(100);

                // Owned Types
                entity.OwnsOne(e => e.ContactInfo, contact =>
                {
                    contact.Property(c => c.Email).IsRequired().HasMaxLength(100);
                    contact.Property(c => c.PhoneNumber).IsRequired().HasMaxLength(20);
                    contact.Property(c => c.MobileNumber).HasMaxLength(20);
                    contact.Property(c => c.Address).HasMaxLength(500);
                    contact.HasIndex(c => c.Email).IsUnique();
                });

                // Soft Delete Filter
                entity.HasQueryFilter(e => !e.IsDeleted);

                // Relationships
                entity.HasMany(e => e.Schedules)
                    .WithOne(e => e.Doctor)
                    .HasForeignKey(e => e.DoctorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // DoctorSchedule Configuration
            modelBuilder.Entity<DoctorSchedule>(entity =>
            {
                entity.HasIndex(e => new { e.DoctorId, e.DayOfWeek }).IsUnique();
                entity.Property(e => e.SlotDurationInMinutes).IsRequired();

                entity.OwnsOne(e => e.WorkingHours, hours =>
                {
                    hours.Property(h => h.StartTime).IsRequired();
                    hours.Property(h => h.EndTime).IsRequired();
                });

                entity.HasQueryFilter(e => !e.IsDeleted);
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            HandleAuditing();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void HandleAuditing()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity &&
                           (e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted));

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    ((BaseEntity)entry.Entity).SetCreatedInfo();
                }
                else if (entry.State == EntityState.Modified)
                {
                    ((BaseEntity)entry.Entity).SetUpdatedInfo();
                }
                else if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    var entity = (BaseEntity)entry.Entity;
                    entity.SoftDelete();
                }
            }
        }
    }
}