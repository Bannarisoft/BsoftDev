using Core.Application.Item.ItemCategory.Commands.CreateItemCategory;
using Core.Application.Item.ItemCategory.Commands.DeleteItemCategory;
using Core.Application.Item.ItemCategory.Commands.UpdateItemCategory;
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
using FluentValidation;
using InventoryManagement.API.Validation.Item.ItemCategory;
using InventoryManagement.API.Validation.Item.ItemGroup;
using InventoryManagement.API.Validation.MiscMaster;
using InventoryManagement.API.Validation.MiscTypeMaster;

namespace InventoryManagement.API.Validation.Common
{
    public class ValidationService
    {
        public void AddValidationServices(IServiceCollection services)
        {
            services.AddScoped<MaxLengthProvider>();
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
        }
    }
}