using Core.Application.AssetLocation.Commands.CreateAssetLocation;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.CreateAssetComposite;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.CreateAssetMasterGeneral;
using Core.Application.AssetMaster.AssetPurchase.Commands.CreateAssetPurchaseDetails;
using FluentValidation;

namespace FAM.API.Validation.AssetMaster.AssetComposite
{
    public class CreateAssetCompositeCommandValidator : AbstractValidator<CreateAssetCompositeCommand>
    {
        public CreateAssetCompositeCommandValidator(
            IValidator<CreateAssetMasterGeneralCommand> masterValidator,
            IValidator<CreateAssetPurchaseDetailCommand> purchaseValidator,
            IValidator<CreateAssetLocationCommand> locationValidator)
        {
            // Validate that the composite object is not null.
            RuleFor(x => x.AssetComposite).NotNull().WithMessage("Asset composite data is required.");

            // Validate the AssetMaster portion using the existing validator.
            RuleFor(x => x.AssetComposite.AssetMaster)
                .NotNull().WithMessage("Asset master data is required.")
                .SetValidator(masterValidator);

            // If there are purchase details provided, validate each item.
            When(x => x.AssetComposite.AssetPurchaseDetails != null, () =>
            {
                RuleForEach(x => x.AssetComposite.AssetPurchaseDetails)
                    .SetValidator(purchaseValidator);
            });

            // If there are location details provided, validate each item.
            When(x => x.AssetComposite.AssetLocation != null, () =>
            {
                RuleForEach(x => x.AssetComposite.AssetLocation)
                    .SetValidator(locationValidator);
            }); 
        }
    }
}