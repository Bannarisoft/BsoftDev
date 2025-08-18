using Core.Application.Item.ItemDetail.Queries.GetAllItems;
using FluentValidation;

namespace InventoryManagement.API.Validation.Item.ItemDetail
{
    public sealed class ItemQualityDtoValidator : AbstractValidator<ItemQualityDto>
    {
        public ItemQualityDtoValidator()
        {
            RuleFor(x => x.InspectionTemplateId).GreaterThan(0).When(x => x.InspectionTemplateId.HasValue);
            RuleFor(x => x.CertificateTypeId).GreaterThan(0).When(x => x.CertificateTypeId.HasValue);
            RuleFor(x => x.InspLotProcessingTime).InclusiveBetween(0, 60).When(x => x.InspLotProcessingTime.HasValue);

            // Example: if inspection required, a template should be present
            RuleFor(x => x.InspectionTemplateId)
                .NotNull()
                .When(x => x.InspectionRequired)
                .WithMessage("Inspection Template is required when Inspection is required.");
        }
    }
}