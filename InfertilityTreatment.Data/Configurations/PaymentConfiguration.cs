using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InfertilityTreatment.Entity.Entities;

namespace InfertilityTreatment.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.PaymentId)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Amount)
                .IsRequired()
                .HasPrecision(12, 2);

            builder.Property(p => p.PaymentMethod)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(p => p.TransactionId)
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .HasMaxLength(500);

            // Relationships
            builder.HasOne(p => p.Customer)
                .WithMany(c => c.Payments)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.TreatmentPackage)
                .WithMany() // TreatmentPackage doesn't have a Payments navigation property
                .HasForeignKey(p => p.TreatmentPackageId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(p => p.PaymentId)
                .IsUnique();

            builder.HasIndex(p => p.CustomerId);

            builder.HasIndex(p => p.TreatmentPackageId);

            builder.HasIndex(p => p.Status);

            builder.HasIndex(p => p.TransactionId);

            builder.HasIndex(p => p.CreatedAt);
        }
    }
}
