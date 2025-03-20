using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Core.Domain.Common.BaseEntity;
using Core.Domain.Entities;

namespace MaintenanceManagement.Infrastructure.Data.Configurations
{
    public class MachineGroupConfiguration :IEntityTypeConfiguration<MachineGroup>
    {
        
          public void Configure(EntityTypeBuilder<MachineGroup> builder)
        {
            builder.ToTable("FormulaTable", "AppData");
        }
    }
}