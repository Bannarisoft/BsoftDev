using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Entities.Workflow;
using Microsoft.EntityFrameworkCore;

namespace BackgroundService.Infrastructure.Data.Workflow.Configurations
{
    public class RuleSkipApproverMappingConfiguration : IEntityTypeConfiguration<RuleSkipApproverMapping>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<RuleSkipApproverMapping> builder)
        {
            builder.ToTable("RuleSkipApproverMapping", "AppData");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .HasColumnName("Id")
                .HasColumnType("int")
                .IsRequired(true);

            builder.Property(t => t.RuleId)
            .HasColumnName("RuleId")
            .HasColumnType("int")
            .IsRequired(true);

            builder.Property(t => t.ApprovalDetailId)
            .HasColumnName("ApprovalDetailId")
            .HasColumnType("int")
            .IsRequired(true);
            
             builder.HasOne(ac => ac.ApprovalStepDetail)
          .WithMany(am => am.RuleSkipApproverMappings)
          .HasForeignKey(ac => ac.ApprovalDetailId)
          .OnDelete(DeleteBehavior.NoAction);
        }
    }
}