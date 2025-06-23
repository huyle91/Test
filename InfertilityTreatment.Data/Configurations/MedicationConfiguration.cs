using InfertilityTreatment.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfertilityTreatment.Data.Configurations
{
    public class MedicationConfiguration : IEntityTypeConfiguration<Medication>
    {
        public void Configure(EntityTypeBuilder<Medication> builder)
        {
            builder.HasKey(m => m.Id);

            builder.HasIndex(m => m.Name);
            builder.HasIndex(m => m.ActiveIngredient);

            builder.Property(m => m.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(m => m.ActiveIngredient)
                   .HasMaxLength(200);

            builder.Property(m => m.Manufacturer)
                   .HasMaxLength(200);

            builder.Property(m => m.Description)
                   .HasMaxLength(500);

            builder.Property(m => m.StorageInstructions)
                   .HasMaxLength(500);

            builder.Property(m => m.SideEffects)
                   .HasMaxLength(500);

            builder.HasMany(m => m.Prescriptions)
                   .WithOne(p => p.Medication)
                   .HasForeignKey(p => p.MedicationId);
        }
    }
}
