using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Entities.Notification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static BackgroundService.Domain.Common.BaseEntity;

namespace BackgroundService.Infrastructure.Data.Notification.Configurations
{
    public class NotificationEventLogConfiguration : IEntityTypeConfiguration<NotificationEventLog>
    {
        public void Configure(EntityTypeBuilder<NotificationEventLog> builder)
        {
            var isActiveConverter = new ValueConverter<Status, bool>
            (
                 v => v == Status.Active,
                 v => v ? Status.Active : Status.Inactive
             );

            var isDeletedConverter = new ValueConverter<IsDelete, bool>
            (
             v => v == IsDelete.Deleted,
             v => v ? IsDelete.Deleted : IsDelete.NotDeleted
            );

            builder.ToTable("NotificationEventLog", "AppNotification");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .HasColumnName("Id")
                .HasColumnType("int")
                .IsRequired();
            
            builder.Property(t => t.UnitId)
                .HasColumnName("UnitId")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(t => t.NotificationLevelRuleId)
            .HasColumnName("NotificationLevelRuleId")
            .HasColumnType("int")
            .IsRequired();
            builder.HasOne(ac => ac.NotificationEventRules)
            .WithMany(am => am.NotificationEventLog)
            .HasForeignKey(ac => ac.NotificationLevelRuleId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Property(t => t.ChannelId)
            .HasColumnName("ChannelId")
            .HasColumnType("int")
            .IsRequired();               
            builder.HasOne(ac => ac.Channel)
            .WithMany(am => am.Channel)
            .HasForeignKey(ac => ac.ChannelId)
            .OnDelete(DeleteBehavior.NoAction);

            builder.Property(t => t.ActionStatus)
            .HasColumnName("ActionStatus")
            .HasColumnType("Varchar(250)")
            .IsRequired();

            builder.Property(t => t.NotificationStatusId)
            .HasColumnName("NotificationStatusId")
            .HasColumnType("int")
            .IsRequired();
              builder.HasOne(ac => ac.NotificationStatus)
            .WithMany(am => am.NotificationStatus)
            .HasForeignKey(ac => ac.NotificationStatusId)
            .OnDelete(DeleteBehavior.NoAction);

             builder.Property(t => t.SendTo)
            .HasColumnName("SendTo")
            .HasColumnType("varchar(1000)")
            .IsRequired();

            builder.Property(t => t.MessageText)
            .HasColumnName("MessageText")
            .HasColumnType("Varchar(Max)")
            .IsRequired();

            builder.Property(t => t.ReadStatusId)
            .HasColumnName("ReadStatusId")
            .HasColumnType("int")
            .IsRequired();
              builder.HasOne(ac => ac.ReadStatus)
            .WithMany(am => am.ReadStatus)
            .HasForeignKey(ac => ac.ReadStatusId)
            .OnDelete(DeleteBehavior.NoAction);


            builder.Property(t => t.Timestamp)
            .HasColumnName("Timestamp")
            .HasColumnType("datetimeoffset")
            .IsRequired();

            builder.Property(cf => cf.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .HasConversion(isActiveConverter)
            .IsRequired();

            builder.Property(cf => cf.IsDeleted)
                 .HasColumnName("IsDeleted")
                 .HasColumnType("bit")
                 .HasConversion(isDeletedConverter)
                 .IsRequired();

            builder.Property(cf => cf.CreatedByName)
                .IsRequired()
                .HasColumnType("varchar(50)");

            builder.Property(cf => cf.CreatedIP)
                .IsRequired()
                .HasColumnType("varchar(255)");

            builder.Property(cf => cf.ModifiedByName)
                 .HasColumnType("varchar(50)");

            builder.Property(cf => cf.ModifiedIP)
                .HasColumnType("varchar(255)");       
        }
    }
}