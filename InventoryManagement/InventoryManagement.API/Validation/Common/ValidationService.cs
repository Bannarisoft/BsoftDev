using Core.Application.Item.ItemCategory.Commands.CreateItemCategory;
using Core.Application.Item.ItemCategory.Commands.DeleteItemCategory;
using Core.Application.Item.ItemCategory.Commands.UpdateItemCategory;
using Core.Application.Item.ItemGroup.Commands.CreateItemGroup;
using Core.Application.Item.ItemGroup.Commands.DeleteItemGroup;
using Core.Application.Item.ItemGroup.Commands.UpdateItemGroup;
using FluentValidation;
using InventoryManagement.API.Validation.Item.ItemCategory;
using InventoryManagement.API.Validation.Item.ItemGroup;

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
        }
    }
}