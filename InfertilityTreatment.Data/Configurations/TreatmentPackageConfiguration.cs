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
    public class TreatmentPackageConfiguration : IEntityTypeConfiguration<TreatmentPackage>
    {
        public void Configure(EntityTypeBuilder<TreatmentPackage> builder)
        {
            builder.ToTable("TreatmentPackages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PackageName)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.Description)
                   .HasMaxLength(1000);

            builder.Property(x => x.Price)
                   .HasColumnType("decimal(12,2)")
                   .IsRequired();

            builder.Property(x => x.IncludedServices)
                   .HasMaxLength(1000);

            builder.Property(x => x.DurationWeeks);
        }
    }
}
