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
    public class TreatmentServiceConfiguration: IEntityTypeConfiguration<TreatmentService>
    {
        public void Configure(EntityTypeBuilder<TreatmentService> builder)
        {
            builder.ToTable("TreatmentServices");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.Description)
                   .HasMaxLength(1000);

            builder.Property(x => x.BasePrice)
                   .HasColumnType("decimal(12,2)");

            builder.Property(x => x.EstimatedDuration);

            builder.Property(x => x.Requirements)
                   .HasMaxLength(1000);

            builder.HasMany(x => x.TreatmentPackages)
                   .WithOne(x => x.TreatmentService)
                   .HasForeignKey(x => x.ServiceId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
