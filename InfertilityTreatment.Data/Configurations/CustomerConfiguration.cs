using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InfertilityTreatment.Entity.Entities;

namespace InfertilityTreatment.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.UserId)
                   .IsRequired();

            builder.Property(x => x.Address)
                   .HasMaxLength(500);

            builder.Property(x => x.EmergencyContactName)
                   .HasMaxLength(200);

            builder.Property(x => x.EmergencyContactPhone)
                   .HasMaxLength(20);

            builder.Property(x => x.MedicalHistory)
                   .HasColumnType("nvarchar(max)");

            builder.Property(x => x.MaritalStatus)
                   .HasMaxLength(50);

            builder.Property(x => x.Occupation)
                   .HasMaxLength(200);

            // Indexes
            builder.HasIndex(x => x.UserId)
                   .IsUnique();

            // Relationships
            builder.HasMany(x => x.TreatmentCycles)
                   .WithOne(x => x.Customer)
                   .HasForeignKey(x => x.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            //builder.HasMany(x => x.Reviews)
            //       .WithOne(x => x.Customer)
            //       .HasForeignKey(x => x.CustomerId)
            //       .OnDelete(DeleteBehavior.Restrict);
        }
    }
}