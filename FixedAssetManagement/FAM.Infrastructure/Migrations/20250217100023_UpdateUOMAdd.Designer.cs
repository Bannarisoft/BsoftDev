﻿// <auto-generated />
using System;
using FAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FAM.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250217100023_UpdateUOMAdd")]
    partial class UpdateUOMAdd
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Core.Domain.Entities.AssetCategories", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AssetGroupId")
                        .HasColumnType("int")
                        .HasColumnName("AssetGroupId");

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasColumnName("CategoryName");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("varchar(10)")
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
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Description")
                        .HasColumnType("varchar(250)")
                        .HasColumnName("Description");

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

                    b.Property<int>("SortOrder")
                        .HasColumnType("int")
                        .HasColumnName("SortOrder");

                    b.HasKey("Id");

                    b.HasIndex("AssetGroupId");

                    b.ToTable("AssetCategories", "FixedAsset");
                });

            modelBuilder.Entity("Core.Domain.Entities.AssetGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("varchar(10)")
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
                        .HasColumnType("varchar(255)");

                    b.Property<string>("GroupName")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasColumnName("GroupName");

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

                    b.Property<int>("SortOrder")
                        .HasColumnType("int")
                        .HasColumnName("SortOrder");

                    b.HasKey("Id");

                    b.ToTable("AssetGroup", "FixedAsset");
                });

            modelBuilder.Entity("Core.Domain.Entities.AssetSubCategories", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AssetCategoriesId")
                        .HasColumnType("int")
                        .HasColumnName("AssetCategoriesId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("varchar(10)")
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
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Description")
                        .HasColumnType("varchar(250)")
                        .HasColumnName("Description");

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

                    b.Property<int>("SortOrder")
                        .HasColumnType("int")
                        .HasColumnName("SortOrder");

                    b.Property<string>("SubCategoryName")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasColumnName("SubCategoryName");

                    b.HasKey("Id");

                    b.HasIndex("AssetCategoriesId");

                    b.ToTable("AssetSubCategories", "FixedAsset");
                });

            modelBuilder.Entity("Core.Domain.Entities.DepreciationGroups", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AssetGroupId")
                        .HasColumnType("int");

                    b.Property<string>("BookType")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("varchar(10)");

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

                    b.Property<string>("DepreciationGroupName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<string>("DepreciationMethod")
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

                    b.Property<int>("ResidualValue")
                        .HasColumnType("int");

                    b.Property<int>("SortOrder")
                        .HasColumnType("int")
                        .HasColumnName("SortOrder");

                    b.Property<int>("UsefulLife")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AssetGroupId");

                    b.ToTable("DepreciationGroups", "FixedAsset");
                });

            modelBuilder.Entity("Core.Domain.Entities.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("varchar(10)")
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
                        .HasColumnType("varchar(255)");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int");

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

                    b.Property<string>("LocationName")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasColumnName("LocationName");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("ModifiedByName")
                        .HasColumnType("varchar(50)");

                    b.Property<DateTimeOffset?>("ModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("ModifiedIP")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("SortOrder")
                        .HasColumnType("int")
                        .HasColumnName("SortOrder");

                    b.Property<int>("UnitId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Location", "FixedAsset");
                });

            modelBuilder.Entity("Core.Domain.Entities.Manufactures", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AddressLine1")
                        .IsRequired()
                        .HasColumnType("varchar(250)");

                    b.Property<string>("AddressLine2")
                        .IsRequired()
                        .HasColumnType("varchar(250)");

                    b.Property<int>("CityId")
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("varchar(10)");

                    b.Property<int>("CountryId")
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
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("ManufactureName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<string>("ManufactureType")
                        .IsRequired()
                        .HasColumnType("varchar(10)");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("ModifiedByName")
                        .HasColumnType("varchar(50)");

                    b.Property<DateTimeOffset?>("ModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("ModifiedIP")
                        .HasColumnType("varchar(50)");

                    b.Property<string>("PersonName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("PinCode")
                        .IsRequired()
                        .HasColumnType("varchar(10)");

                    b.Property<int>("StateId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Manufacture", "FixedAsset");
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

                    b.Property<int>("MiscTypeMasterId")
                        .HasColumnType("int")
                        .HasColumnName("MiscTypeMasterId");

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

                    b.HasIndex("MiscTypeMasterId");

                    b.ToTable("MiscMaster", "FixedAsset");
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
                        .HasColumnType("varchar(50)")
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

                    b.ToTable("MiscTypeMaster", "FixedAsset");
                });

            modelBuilder.Entity("Core.Domain.Entities.SubLocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("varchar(10)")
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
                        .HasColumnType("varchar(255)");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int");

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

                    b.Property<int>("LocationId")
                        .HasColumnType("int")
                        .HasColumnName("LocationId");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("ModifiedByName")
                        .HasColumnType("varchar(50)");

                    b.Property<DateTimeOffset?>("ModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("ModifiedIP")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("SubLocationName")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasColumnName("SubLocationName");

                    b.Property<int>("UnitId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("SubLocation", "FixedAsset");
                });

            modelBuilder.Entity("Core.Domain.Entities.UOM", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("varchar(10)")
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
                        .HasColumnType("varchar(255)");

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

                    b.Property<int>("SortOrder")
                        .HasColumnType("int")
                        .HasColumnName("SortOrder");

                    b.Property<string>("UOMName")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasColumnName("UOMName");

                    b.Property<int>("UOMTypeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UOMTypeId");

                    b.ToTable("UOM", "FixedAsset");
                });

            modelBuilder.Entity("Core.Domain.Entities.AssetCategories", b =>
                {
                    b.HasOne("Core.Domain.Entities.AssetGroup", "AssetGroup")
                        .WithMany("AssetCategories")
                        .HasForeignKey("AssetGroupId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("AssetGroup");
                });

            modelBuilder.Entity("Core.Domain.Entities.AssetSubCategories", b =>
                {
                    b.HasOne("Core.Domain.Entities.AssetCategories", "AssetCategories")
                        .WithMany("AssetSubCategories")
                        .HasForeignKey("AssetCategoriesId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("AssetCategories");
                });

            modelBuilder.Entity("Core.Domain.Entities.DepreciationGroups", b =>
                {
                    b.HasOne("Core.Domain.Entities.AssetGroup", "AssetGroup")
                        .WithMany("DepreciationGroups")
                        .HasForeignKey("AssetGroupId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("AssetGroup");
                });

            modelBuilder.Entity("Core.Domain.Entities.MiscMaster", b =>
                {
                    b.HasOne("Core.Domain.Entities.MiscTypeMaster", "MiscTypeMaster")
                        .WithMany("MiscMaster")
                        .HasForeignKey("MiscTypeMasterId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("MiscTypeMaster");
                });

            modelBuilder.Entity("Core.Domain.Entities.SubLocation", b =>
                {
                    b.HasOne("Core.Domain.Entities.Location", "Location")
                        .WithMany("SubLocations")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Location");
                });

            modelBuilder.Entity("Core.Domain.Entities.UOM", b =>
                {
                    b.HasOne("Core.Domain.Entities.MiscMaster", "UOMType")
                        .WithMany("UOMs")
                        .HasForeignKey("UOMTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("UOMType");
                });

            modelBuilder.Entity("Core.Domain.Entities.AssetCategories", b =>
                {
                    b.Navigation("AssetSubCategories");
                });

            modelBuilder.Entity("Core.Domain.Entities.AssetGroup", b =>
                {
                    b.Navigation("AssetCategories");

                    b.Navigation("DepreciationGroups");
                });

            modelBuilder.Entity("Core.Domain.Entities.Location", b =>
                {
                    b.Navigation("SubLocations");
                });

            modelBuilder.Entity("Core.Domain.Entities.MiscMaster", b =>
                {
                    b.Navigation("UOMs");
                });

            modelBuilder.Entity("Core.Domain.Entities.MiscTypeMaster", b =>
                {
                    b.Navigation("MiscMaster");
                });
#pragma warning restore 612, 618
        }
    }
}
