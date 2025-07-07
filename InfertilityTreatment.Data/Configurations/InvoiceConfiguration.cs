using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InfertilityTreatment.Entity.Entities;

namespace InfertilityTreatment.Data.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.ToTable("Invoices");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(i => i.SubTotal)
                .IsRequired()
                .HasPrecision(12, 2);

            builder.Property(i => i.TaxAmount)
                .HasPrecision(12, 2);

            builder.Property(i => i.DiscountAmount)
                .HasPrecision(12, 2);

            builder.Property(i => i.TotalAmount)
                .IsRequired()
                .HasPrecision(12, 2);

            builder.Property(i => i.Status)
                .IsRequired()
                .HasConversion<int>();

            // Relationships
            builder.HasOne(i => i.Customer)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.Appointment)
                .WithMany(a => a.Invoices)
                .HasForeignKey(i => i.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(i => i.TreatmentCycle)
                .WithMany(tc => tc.Invoices)
                .HasForeignKey(i => i.TreatmentCycleId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(i => i.Payment)
                .WithMany(p => p.Invoices)
                .HasForeignKey(i => i.PaymentId)
                .HasPrincipalKey(p => p.PaymentId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(i => i.InvoiceNumber)
                .IsUnique();

            builder.HasIndex(i => i.CustomerId);

            builder.HasIndex(i => i.Status);

            builder.HasIndex(i => i.IssueDate);

            builder.HasIndex(i => i.DueDate);
        }
    }
}
