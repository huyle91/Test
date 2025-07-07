using InfertilityTreatment.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfertilityTreatment.Data.Configurations
{
    public class TestResultConfiguration : IEntityTypeConfiguration<TestResult>
    {
        public void Configure(EntityTypeBuilder<TestResult> builder)
        {
            builder.ToTable("TestResults");

            builder.HasKey(tr => tr.Id);

            builder.Property(tr => tr.CycleId)
                   .IsRequired();

            builder.Property(tr => tr.TestType)
                   .IsRequired()
                   .HasConversion<byte>();

            builder.Property(tr => tr.AppointmentId)
                   .IsRequired();

            builder.Property(tr => tr.TestDate)
                   .IsRequired();

            builder.Property(tr => tr.Results)
                   .HasColumnType("nvarchar(max)");

            builder.Property(tr => tr.ReferenceRange)
                   .HasColumnType("nvarchar(100)");

            builder.Property(tr => tr.Status)
                   .IsRequired()
                   .HasConversion<byte>();

            builder.Property(tr => tr.DoctorNotes)
                   .HasColumnType("nvarchar(max)");

            builder.Property(tr => tr.CreatedAt)
                   .IsRequired();

            builder.Property(tr => tr.UpdatedAt)
                   .IsRequired(false);

            builder.Property(tr => tr.IsActive)
                   .IsRequired();

            // Relationships
            builder.HasOne(tr => tr.TreatmentCycle)
                   .WithMany(tc => tc.TestResults) // Đảm bảo TreatmentCycle có ICollection<TestResult> TestResults
                   .HasForeignKey(tr => tr.CycleId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(tr => tr.Appointment)
                   .WithMany(a => a.TestResults) 
                   .HasForeignKey(tr => tr.AppointmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(tr => tr.CycleId);
            builder.HasIndex(tr => tr.TestDate);
            builder.HasIndex(tr => tr.TestType);
        }
    }
}
