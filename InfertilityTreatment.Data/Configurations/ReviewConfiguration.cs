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
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Rating)
                   .IsRequired()
                   .HasConversion<int>();

            builder.Property(r => r.Comment)
                   .HasColumnType("nvarchar(max)");

            builder.Property(r => r.ReviewType)
                   .HasMaxLength(50);

            builder.Property(r => r.CreatedAt).IsRequired();
            builder.Property(r => r.UpdatedAt).IsRequired(false);
            builder.Property(r => r.IsActive).IsRequired();
            builder.Property(r => r.IsApproved).IsRequired();

            builder.HasOne(r => r.TreatmentCycle)
                   .WithMany(tc => tc.Reviews)
                   .HasForeignKey(r => r.CycleId)
                   .OnDelete(DeleteBehavior.SetNull); 

            builder.HasOne(r => r.Customer)
                   .WithMany(c => c.Reviews)
                   .HasForeignKey(r => r.CustomerId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(r => r.Doctor)
                   .WithMany(d => d.Reviews)
                   .HasForeignKey(r => r.DoctorId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }

}
