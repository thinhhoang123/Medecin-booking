using Doctor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doctor.Infrastructure.Data.Configurations
{
    public class DoctorScheduleConfiguration : IEntityTypeConfiguration<DoctorSchedule>
    {
        public void Configure(EntityTypeBuilder<DoctorSchedule> builder)
        {
            builder.ToTable("DoctorSchedules");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.DoctorId)
                .IsRequired();

            builder.Property(s => s.DayOfWeek)
                .IsRequired();

            builder.Property(s => s.SlotDurationInMinutes)
                .IsRequired();

            builder.Property(s => s.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(s => s.ValidFrom)
                .IsRequired();

            // Indexes
            builder.HasIndex(s => new { s.DoctorId, s.DayOfWeek })
                .IsUnique()
                .HasDatabaseName("IX_DoctorSchedules_DoctorId_DayOfWeek");

            builder.HasIndex(s => s.Status)
                .HasDatabaseName("IX_DoctorSchedules_Status");

            // Owned Types
            builder.OwnsOne(s => s.WorkingHours, hours =>
            {
                hours.Property(h => h.StartTime)
                    .IsRequired()
                    .HasColumnName("StartTime");

                hours.Property(h => h.EndTime)
                    .IsRequired()
                    .HasColumnName("EndTime");
            });
        }
    }
}