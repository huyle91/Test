using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InfertilityTreatment.Entity.Entities;
using InfertilityTreatment.Entity.Enums;

namespace InfertilityTreatment.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Email)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(x => x.PasswordHash)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(x => x.FullName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.PhoneNumber)
                   .HasMaxLength(20);

            builder.Property(x => x.Role)
                   .HasConversion<byte>()
                   .IsRequired();

            builder.Property(x => x.Gender)
                   .HasConversion<byte>();

            // Indexes
            builder.HasIndex(x => x.Email)
                   .IsUnique();

            builder.HasIndex(x => x.Role);

            // One-to-One relationships
            builder.HasOne(x => x.Customer)
                   .WithOne(x => x.User)
                   .HasForeignKey<Customer>(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Doctor)
                   .WithOne(x => x.User)
                   .HasForeignKey<Doctor>(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many relationships
            builder.HasMany(x => x.RefreshTokens)
                   .WithOne(x => x.User)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            //builder.HasMany(x => x.BlogPosts)
            //       .WithOne(x => x.Author)
            //       .HasForeignKey(x => x.AuthorId)
            //       .OnDelete(DeleteBehavior.Restrict);

            //builder.HasMany(x => x.Notifications)
            //       .WithOne(x => x.User)
            //       .HasForeignKey(x => x.UserId)
            //       .OnDelete(DeleteBehavior.Cascade);
        }
    }
}