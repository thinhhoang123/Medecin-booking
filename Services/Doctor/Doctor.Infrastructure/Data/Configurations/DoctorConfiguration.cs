using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doctor.Infrastructure.Data.Configurations
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Domain.Entities.Doctor>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Doctor> builder)
        {
            builder.ToTable("Doctors");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Specialization)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(d => d.Bio)
                .HasMaxLength(500);

            builder.Property(d => d.Qualifications)
                .HasMaxLength(500);

            builder.Property(d => d.LicenseNumber)
                .HasMaxLength(50);

            builder.Property(d => d.Department)
                .HasMaxLength(100);

            builder.Property(d => d.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(d => d.IsAvailableForAppointments)
                .IsRequired();

            // Indexes
            builder.HasIndex(d => new { d.FirstName, d.LastName })
                .HasDatabaseName("IX_Doctors_Name");

            builder.HasIndex(d => d.LicenseNumber)
                .IsUnique()
                .HasDatabaseName("IX_Doctors_LicenseNumber")
                .HasFilter("[LicenseNumber] IS NOT NULL");

            builder.HasIndex(d => d.Specialization)
                .HasDatabaseName("IX_Doctors_Specialization");

            builder.HasIndex(d => d.Department)
                .HasDatabaseName("IX_Doctors_Department");

            // Owned Types
            builder.OwnsOne(d => d.ContactInfo, contact =>
            {
                contact.Property(c => c.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("Email");

                contact.Property(c => c.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("PhoneNumber");

                contact.Property(c => c.MobileNumber)
                    .HasMaxLength(20)
                    .HasColumnName("MobileNumber");

                contact.Property(c => c.Address)
                    .HasMaxLength(500)
                    .HasColumnName("Address");

                contact.HasIndex(c => c.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Doctors_Email");
            });

            // Relationships
            builder.HasMany(d => d.Schedules)
                .WithOne(s => s.Doctor)
                .HasForeignKey(s => s.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}