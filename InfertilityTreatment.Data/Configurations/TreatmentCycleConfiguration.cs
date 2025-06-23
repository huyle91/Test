using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InfertilityTreatment.Entity.Entities;
using InfertilityTreatment.Entity.Enums;

namespace InfertilityTreatment.Data.Configurations
{
    public class TreatmentCycleConfiguration : IEntityTypeConfiguration<TreatmentCycle>
    {
        public void Configure(EntityTypeBuilder<TreatmentCycle> builder)
        {
            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.CustomerId)
                   .IsRequired();

            builder.Property(x => x.DoctorId)
                   .IsRequired();

            builder.Property(x => x.PackageId)
                   .IsRequired();

            builder.Property(x => x.Status)
                   .HasConversion<byte>()
                   .IsRequired();

            builder.Property(x => x.TotalCost)
                   .HasColumnType("decimal(12,2)");

            builder.Property(x => x.Notes)
                   .HasColumnType("nvarchar(max)");

            // Indexes
            builder.HasIndex(x => x.CustomerId);
            builder.HasIndex(x => x.DoctorId);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.StartDate);
            builder.HasIndex(tc => tc.Status);
            builder.HasIndex(tc => tc.StartDate);

            // Relationships
            //builder.HasMany(x => x.TreatmentPhases)
            //       .WithOne(x => x.TreatmentCycle)
            //       .HasForeignKey(x => x.CycleId)
            //       .OnDelete(DeleteBehavior.Cascade);

            //builder.HasMany(x => x.Appointments)
            //       .WithOne(x => x.TreatmentCycle)
            //       .HasForeignKey(x => x.CycleId)
            //       .OnDelete(DeleteBehavior.Restrict);

            //builder.HasMany(x => x.TestResults)
            //       .WithOne(x => x.TreatmentCycle)
            //       .HasForeignKey(x => x.CycleId)
            //       .OnDelete(DeleteBehavior.Cascade);

            //builder.HasMany(x => x.Reviews)
            //       .WithOne(x => x.TreatmentCycle)
            //       .HasForeignKey(x => x.CycleId)
            //       .OnDelete(DeleteBehavior.SetNull);
        }
    }
}