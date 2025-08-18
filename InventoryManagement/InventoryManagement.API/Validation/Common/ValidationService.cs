using Core.Application.Budget.Commands.CreateBudget;
using Core.Application.Budget.Commands.UpdateBudget;
using Core.Application.Common.Interfaces;
using Core.Application.HSNMaster.Command.CreateHSNMaster;
using Core.Application.HSNMaster.Command.DeleteHSNMaster;
using Core.Application.HSNMaster.Command.UpdateHSNMaster;
using Core.Application.Item.ItemCategory.Commands.CreateItemCategory;
using Core.Application.Item.ItemCategory.Commands.DeleteItemCategory;
using Core.Application.Item.ItemCategory.Commands.UpdateItemCategory;
using Core.Application.Item.ItemDetail.Commands.CreateItem;
using Core.Application.Item.ItemDetail.Commands.UpdateItem;
using Core.Application.Item.ItemDetail.Queries.GetAllItems;
using Core.Application.Item.ItemGroup.Commands.CreateItemGroup;
using Core.Application.Item.ItemGroup.Commands.DeleteItemGroup;
using Core.Application.Item.ItemGroup.Commands.UpdateItemGroup;
using Core.Application.MiscMaster;
using Core.Application.MiscMaster.Command.CreateMiscMaster;
using Core.Application.MiscMaster.Command.DeleteMiscMaster;
using Core.Application.MiscMaster.Command.UpdateMiscMaster;
using Core.Application.MiscTypeMaster.Command.CreateMiscTypeMaster;
using Core.Application.MiscTypeMaster.Command.DeleteMiscTypeMaster;
using Core.Application.MiscTypeMaster.Command.UpdateMiscTypeMaster;
using Core.Application.UOM.Command.CreateUOM;
using Core.Application.UOM.Command.UpdateUOM;
using Core.Application.UOMConversion.Command.CreateUOMConversion;
using Core.Application.UOMConversion.Command.UpdateUOMConversion;
using FluentValidation;
using FluentValidation.AspNetCore;
using InventoryManagement.API.Validation.Budget;
using InventoryManagement.API.Validation.HSNMaster;
using InventoryManagement.API.Validation.Item.ItemCategory;
using InventoryManagement.API.Validation.Item.ItemDetail;
using InventoryManagement.API.Validation.Item.ItemGroup;
using InventoryManagement.API.Validation.MiscMaster;
using InventoryManagement.API.Validation.MiscTypeMaster;
using InventoryManagement.API.Validation.UOM;
using InventoryManagement.API.Validation.UOMConversion;

namespace InventoryManagement.API.Validation.Common
{
    public class ValidationService
    {
        public void AddValidationServices(IServiceCollection services)
        {
            services.AddScoped<MaxLengthProvider>();
            services.AddScoped<IMaxLengthProvider, MaxLengthProvider>();
            services.AddScoped<IValidator<CreateItemCategoryCommand>, CreateItemCategoryCommandValidator>();
            services.AddScoped<IValidator<DeleteItemCategoryCommand>, DeleteItemCategoryCommandValidator>();
            services.AddScoped<IValidator<UpdateItemCategoryCommand>, UpdateItemCategoryCommandValidator>();
            services.AddScoped<IValidator<CreateItemGroupCommand>, CreateItemGroupCommandValidator>();
            services.AddScoped<IValidator<UpdateItemGroupCommand>, UpdateItemGroupCommandValidator>();
            services.AddScoped<IValidator<DeleteItemGroupCommand>, DeleteItemGroupCommandValidator>();

            services.AddScoped<IValidator<CreateMiscTypeMasterCommand>, CreateMiscTypeMasterCommandValidator>();
            services.AddScoped<IValidator<DeleteMiscTypeMasterCommand>, DeleteMiscTypeMasterCommandValidator>();
            services.AddScoped<IValidator<UpdateMiscTypeMasterCommand>, UpdateMiscTypeMasterCommandValidator>();
            services.AddScoped<IValidator<CreateMiscMasterCommand>, CreateMiscMasterCommandValidator>();
            services.AddScoped<IValidator<DeleteMiscMasterCommand>, DeleteMiscMasterCommandValidator>();
            services.AddScoped<IValidator<UpdateMiscMasterCommand>, UpdateMiscMasterCommandValidator>();
            services.AddScoped<IValidator<CreateBudgetCommand>, CreateBudgetCommandValidator>();
            services.AddScoped<IValidator<UpdateBudgetCommand>, UpdateBudgetCommandValidator>();

            services.AddScoped<IValidator<CreateHSNMasterCommand>, CreateHSNMasterCommandValidator>();
            services.AddScoped<IValidator<UpdateHSNMasterCommand>, UpdateHSNMasterCommandValidator>();
            services.AddScoped<IValidator<DeleteHSNMasterCommand>, DeleteHSNMasterCommandValidator>();
            services.AddScoped<IValidator<CreateUOMCommand>, CreateUOMCommandValidator>();
            services.AddScoped<IValidator<UpdateUOMCommand>, UpdateUOMCommandValidator>();
            services.AddScoped<IValidator<CreateUOMConversionCommand>, CreateUOMConversionCommandValidator>();
            services.AddScoped<IValidator<UpdateUOMConversionCommand>, UpdateUOMConversionCommandValidator>();


            services.AddScoped<IValidator<CreateItemCommand>, CreateItemCommandValidator>();
            services.AddScoped<IValidator<UpdateItemCommand>, UpdateItemCommandValidator>();
            services.AddScoped<IValidator<ItemPurchaseDto>, ItemPurchaseDtoValidator>();
            services.AddScoped<IValidator<ItemInventoryDto>, ItemInventoryDtoValidator>();
            services.AddScoped<IValidator<ItemQualityDto>, ItemQualityDtoValidator>();
            services.AddScoped<IValidator<ItemSupplierDto>, ItemSupplierDtoValidator>();
            services.AddScoped<IValidator<ItemManufactureDto>, ItemManufacturingDtoValidator>();
            services.AddScoped<IValidator<ItemUomDto>, ItemUomDtoValidator>();            
            services.AddValidatorsFromAssembly(typeof(CreateItemCommandValidator).Assembly);           
      
        }
    }
}