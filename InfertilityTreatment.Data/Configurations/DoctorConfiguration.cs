using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InfertilityTreatment.Entity.Entities;

namespace InfertilityTreatment.Data.Configurations
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.UserId)
                   .IsRequired();

            builder.Property(x => x.LicenseNumber)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.Specialization)
                   .HasMaxLength(200);

            builder.Property(x => x.Education)
                   .HasMaxLength(500);

            builder.Property(x => x.Biography)
                   .HasColumnType("nvarchar(max)");

            builder.Property(x => x.ConsultationFee)
                   .HasColumnType("decimal(10,2)");

            builder.Property(x => x.SuccessRate)
                   .HasColumnType("decimal(5,2)");

            // Indexes
            builder.HasIndex(x => x.UserId)
                   .IsUnique();

            builder.HasIndex(x => x.LicenseNumber)
                   .IsUnique();

            builder.HasIndex(x => x.Specialization);

            builder.HasIndex(x => x.IsAvailable);

            // Relationships
            builder.HasMany(x => x.TreatmentCycles)
                   .WithOne(x => x.Doctor)
                   .HasForeignKey(x => x.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            //builder.HasMany(x => x.Appointments)
            //       .WithOne(x => x.Doctor)
            //       .HasForeignKey(x => x.DoctorId)
            //       .OnDelete(DeleteBehavior.Restrict);

            //builder.HasMany(x => x.Reviews)
            //       .WithOne(x => x.Doctor)
            //       .HasForeignKey(x => x.DoctorId)
            //       .OnDelete(DeleteBehavior.Restrict);
        }
    }
}