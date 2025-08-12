using Core.Application.Common.Interfaces;
using Core.Application.Item.ItemDetail.Queries.GetAllItems;
using FluentValidation;

namespace InventoryManagement.API.Validation.Item.ItemDetail
{
    public sealed class ItemPurchaseDtoValidator : AbstractValidator<ItemPurchaseDto>
    {
        public ItemPurchaseDtoValidator(IMaxLengthProvider maxLen)
        {
            RuleFor(x => x.PurchaseUomId).GreaterThan(0).When(x => x.PurchaseUomId.HasValue);
            RuleFor(x => x.PurchaseRate).GreaterThanOrEqualTo(0).When(x => x.PurchaseRate.HasValue);
            RuleFor(x => x.LeadTimeDays).InclusiveBetween(0, 365).When(x => x.LeadTimeDays.HasValue);
            RuleFor(x => x.SafetyStock).GreaterThanOrEqualTo(0).When(x => x.SafetyStock.HasValue);
            RuleFor(x => x.GrProcessingTimeDays).InclusiveBetween(0, 60).When(x => x.GrProcessingTimeDays.HasValue);
            RuleFor(x => x.OriginCountryId).GreaterThan(0).When(x => x.OriginCountryId.HasValue);

            var tariffMax = maxLen.GetMaxLength<Core.Domain.Entities.Item.ItemDetail.ItemPurchase>(nameof(Core.Domain.Entities.Item.ItemDetail.ItemPurchase.TariffNumber)) ?? 50;
            RuleFor(x => x.TariffNumber).MaximumLength(tariffMax);
        }
    }
}