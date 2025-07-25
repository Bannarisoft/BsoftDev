﻿// <auto-generated />
using System;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MaintenanceManagement.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250328072805_removegroupid")]
    partial class removegroupid
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Core.Domain.Entities.ActivityMaster", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ActivityName")
                        .IsRequired()
                        .HasColumnType("varchar(250)")
                        .HasColumnName("ActivityName");

                    b.Property<int>("ActivityType")
                        .HasColumnType("int")
                        .HasColumnName("ActivityType");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<string>("CreatedByName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset?>("CreatedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("CreatedIP")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int")
                        .HasColumnName("DepartmentId");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("varchar(500)")
                        .HasColumnName("Description");

                    b.Property<int>("EstimatedDuration")
                        .HasColumnType("int")
                        .HasColumnName("EstimatedDuration");

                    b.Property<int>("IsActive")
                        .HasColumnType("int");

                    b.Property<int>("IsDeleted")
                        .HasColumnType("int");

                    b.Property<int>("MachineGroupId")
                        .HasColumnType("int");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("ModifiedByName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset?>("ModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("ModifiedIP")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ActivityMaster", "Maintenance");
                });

            modelBuilder.Entity("Core.Domain.Entities.CostCenter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal?>("BudgetAllocated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(18,2)")
                        .HasDefaultValue(0.00m);

                    b.Property<string>("CostCenterCode")
                        .IsRequired()
                        .HasColumnType("varchar(10)")
                        .HasColumnName("CostCenterCode");

                    b.Property<string>("CostCenterName")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasColumnName("CostCenterName");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<string>("CreatedByName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<DateTimeOffset?>("CreatedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("CreatedIP")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int")
                        .HasColumnName("DepartmentId");

                    b.Property<DateTimeOffset>("EffectiveDate")
                        .HasColumnType("datetimeoffset")
                        .HasColumnName("EffectiveDate");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("ModifiedByName")
                        .HasColumnType("varchar(50)");

                    b.Property<DateTimeOffset?>("ModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("ModifiedIP")
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Remarks")
                        .HasColumnType("varchar(250)")
                        .HasColumnName("Remarks");

                    b.Property<string>("ResponsiblePerson")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("ResponsiblePerson");

                    b.Property<int>("UnitId")
                        .HasColumnType("int")
                        .HasColumnName("UnitId");

                    b.HasKey("Id");

                    b.ToTable("CostCenter", "Maintenance");
                });

            modelBuilder.Entity("Core.Domain.Entities.MachineGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<string>("CreatedByName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<DateTimeOffset?>("CreatedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("CreatedIP")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<string>("GroupName")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasColumnName("GroupName");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("Manufacturer")
                        .HasColumnType("int")
                        .HasColumnName("Manufacturer");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("ModifiedByName")
                        .HasColumnType("varchar(50)");

                    b.Property<DateTimeOffset?>("ModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("ModifiedIP")
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.ToTable("MachineGroup", "Maintenance");
                });

            modelBuilder.Entity("Core.Domain.Entities.MaintenanceCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasColumnName("CategoryName");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<string>("CreatedByName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<DateTimeOffset?>("CreatedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("CreatedIP")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Description")
                        .HasColumnType("varchar(250)")
                        .HasColumnName("Description");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("ModifiedByName")
                        .HasColumnType("varchar(50)");

                    b.Property<DateTimeOffset?>("ModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("ModifiedIP")
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.ToTable("MaintenanceCategory", "Maintenance");
                });

            modelBuilder.Entity("Core.Domain.Entities.MaintenanceType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<string>("CreatedByName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<DateTimeOffset?>("CreatedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("CreatedIP")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("ModifiedByName")
                        .HasColumnType("varchar(50)");

                    b.Property<DateTimeOffset?>("ModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("ModifiedIP")
                        .HasColumnType("varchar(50)");

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasColumnName("TypeName");

                    b.HasKey("Id");

                    b.ToTable("MaintenanceType", "Maintenance");
                });

            modelBuilder.Entity("Core.Domain.Entities.MiscMaster", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("Code");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<string>("CreatedByName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<DateTimeOffset?>("CreatedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("CreatedIP")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("varchar(250)")
                        .HasColumnName("description");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("IsActive");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit")
                        .HasColumnName("IsDeleted");

                    b.Property<int>("MiscTypeId")
                        .HasColumnType("int")
                        .HasColumnName("MiscTypeId");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("ModifiedByName")
                        .HasColumnType("varchar(50)");

                    b.Property<DateTimeOffset?>("ModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("ModifiedIP")
                        .HasColumnType("varchar(20)");

                    b.Property<int>("SortOrder")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0)
                        .HasColumnName("sortOrder");

                    b.HasKey("Id");

                    b.HasIndex("MiscTypeId");

                    b.ToTable("MiscMaster", "Maintenance");
                });

            modelBuilder.Entity("Core.Domain.Entities.MiscTypeMaster", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<string>("CreatedByName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<DateTimeOffset?>("CreatedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("CreatedIP")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("varchar(250)")
                        .HasColumnName("Description");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("IsActive");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit")
                        .HasColumnName("IsDeleted");

                    b.Property<string>("MiscTypeCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("MiscTypeCode");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("ModifiedByName")
                        .HasColumnType("varchar(50)");

                    b.Property<DateTimeOffset?>("ModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("ModifiedIP")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("MiscTypeMaster", "Maintenance");
                });

            modelBuilder.Entity("Core.Domain.Entities.ShiftMaster", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<string>("CreatedByName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<DateTimeOffset?>("CreatedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("CreatedIP")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<DateOnly>("EffectiveDate")
                        .HasColumnType("date");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("IsActive");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit")
                        .HasColumnName("IsDeleted");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("ModifiedByName")
                        .HasColumnType("varchar(50)");

                    b.Property<DateTimeOffset?>("ModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("ModifiedIP")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ShiftCode")
                        .IsRequired()
                        .HasColumnType("varchar(10)");

                    b.Property<string>("ShiftName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.ToTable("ShiftMaster", "Maintenance");
                });

            modelBuilder.Entity("Core.Domain.Entities.ShiftMasterDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BreakDurationInMinutes")
                        .HasColumnType("int");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<string>("CreatedByName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<DateTimeOffset?>("CreatedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("CreatedIP")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<decimal>("DurationInHours")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateOnly>("EffectiveDate")
                        .HasColumnType("date");

                    b.Property<TimeSpan>("EndTime")
                        .HasColumnType("time");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("IsActive");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit")
                        .HasColumnName("IsDeleted");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("ModifiedByName")
                        .HasColumnType("varchar(50)");

                    b.Property<DateTimeOffset?>("ModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("ModifiedIP")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("ShiftMasterId")
                        .HasColumnType("int");

                    b.Property<int>("ShiftSupervisorId")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("StartTime")
                        .HasColumnType("time");

                    b.Property<int>("UnitId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ShiftMasterId");

                    b.ToTable("ShiftMasterDetails", "Maintenance");
                });

            modelBuilder.Entity("Core.Domain.Entities.WorkCenter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<string>("CreatedByName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<DateTimeOffset?>("CreatedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("CreatedIP")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int")
                        .HasColumnName("DepartmentId");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("ModifiedByName")
                        .HasColumnType("varchar(50)");

                    b.Property<DateTimeOffset?>("ModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("ModifiedIP")
                        .HasColumnType("varchar(50)");

                    b.Property<int>("UnitId")
                        .HasColumnType("int")
                        .HasColumnName("UnitId");

                    b.Property<string>("WorkCenterCode")
                        .IsRequired()
                        .HasColumnType("varchar(10)")
                        .HasColumnName("WorkCenterCode");

                    b.Property<string>("WorkCenterName")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasColumnName("WorkCenterName");

                    b.HasKey("Id");

                    b.ToTable("WorkCenter", "Maintenance");
                });

            modelBuilder.Entity("Core.Domain.Entities.MiscMaster", b =>
                {
                    b.HasOne("Core.Domain.Entities.MiscTypeMaster", "MiscTypeMaster")
                        .WithMany("MiscMaster")
                        .HasForeignKey("MiscTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("MiscTypeMaster");
                });

            modelBuilder.Entity("Core.Domain.Entities.ShiftMasterDetail", b =>
                {
                    b.HasOne("Core.Domain.Entities.ShiftMaster", "ShiftMaster")
                        .WithMany("ShiftMasterDetails")
                        .HasForeignKey("ShiftMasterId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ShiftMaster");
                });

            modelBuilder.Entity("Core.Domain.Entities.MiscTypeMaster", b =>
                {
                    b.Navigation("MiscMaster");
                });

            modelBuilder.Entity("Core.Domain.Entities.ShiftMaster", b =>
                {
                    b.Navigation("ShiftMasterDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
