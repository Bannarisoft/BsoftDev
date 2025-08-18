using Core.Application.Common.Interfaces;
using Core.Application.Item.ItemDetail.Queries.GetAllItems;
using FluentValidation;

namespace InventoryManagement.API.Validation.Item.ItemDetail
{
    public sealed class ItemSupplierDtoValidator : AbstractValidator<ItemSupplierDto>
    {
        public ItemSupplierDtoValidator(IMaxLengthProvider maxLen)
        {
            RuleFor(x => x.SupplierId).GreaterThan(0);
            RuleFor(x => x.UnitId).GreaterThan(0);

            var partMax = maxLen.GetMaxLength<Core.Domain.Entities.Item.ItemDetail.ItemSupplier>(nameof(Core.Domain.Entities.Item.ItemDetail.ItemSupplier.SupplierPartNo)) ?? 100;
            RuleFor(x => x.SupplierPartNo).MaximumLength(partMax);
        }
    }
}