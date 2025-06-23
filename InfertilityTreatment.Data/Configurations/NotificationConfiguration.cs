using InfertilityTreatment.Entity.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Data.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("notifications");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(n => n.Message)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(n => n.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(n => n.IsRead)
                .IsRequired();

            builder.Property(n => n.RelatedEntityType)
                .HasMaxLength(100);

            builder.Property(n => n.RelatedEntityId);

            builder.Property(n => n.ScheduledAt)
                .HasColumnType("datetime2");

            builder.Property(n => n.SentAt)
                .HasColumnType("datetime2");

            builder.Property(n => n.CreatedAt)
                .HasColumnType("datetime2");

            builder.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(n => n.UserId);
            builder.HasIndex(n => n.IsRead);
        }
    }
}
