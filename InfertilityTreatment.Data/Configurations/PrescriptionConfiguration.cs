using InfertilityTreatment.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Data.Configurations
{
    public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
    {
        public void Configure(EntityTypeBuilder<Prescription> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Dosage).HasMaxLength(100);
            builder.Property(p => p.Frequency).HasMaxLength(100);
            builder.Property(p => p.Instructions).HasMaxLength(500);

            builder.HasOne(p => p.Medication)
                   .WithMany(m => m.Prescriptions)
                   .HasForeignKey(p => p.MedicationId);
        }
    }
}
