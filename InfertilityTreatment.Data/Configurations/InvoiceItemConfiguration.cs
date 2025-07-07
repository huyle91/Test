using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InfertilityTreatment.Entity.Entities;

namespace InfertilityTreatment.Data.Configurations
{
    public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
    {
        public void Configure(EntityTypeBuilder<InvoiceItem> builder)
        {
            builder.ToTable("InvoiceItems");

            builder.HasKey(ii => ii.Id);

            builder.Property(ii => ii.Description)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(ii => ii.Quantity)
                .IsRequired();

            builder.Property(ii => ii.UnitPrice)
                .IsRequired()
                .HasPrecision(12, 2);

            builder.Property(ii => ii.TotalPrice)
                .IsRequired()
                .HasPrecision(12, 2);

            // Relationships
            builder.HasOne(ii => ii.Invoice)
                .WithMany(i => i.InvoiceItems)
                .HasForeignKey(ii => ii.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ii => ii.TreatmentService)
                .WithMany()
                .HasForeignKey(ii => ii.TreatmentServiceId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(ii => ii.Medication)
                .WithMany()
                .HasForeignKey(ii => ii.MedicationId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(ii => ii.InvoiceId);
        }
    }
}
