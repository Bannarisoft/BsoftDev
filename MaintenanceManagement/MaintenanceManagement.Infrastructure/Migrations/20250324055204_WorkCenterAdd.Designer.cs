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
    [Migration("20250324055204_WorkCenterAdd")]
    partial class WorkCenterAdd
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

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
#pragma warning restore 612, 618
        }
    }
}
