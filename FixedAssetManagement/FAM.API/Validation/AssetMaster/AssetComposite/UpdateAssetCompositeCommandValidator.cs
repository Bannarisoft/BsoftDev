using Core.Application.AssetMaster.AssetLocation.Commands.UpdateAssetLocation;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.UpdateAssetComposite;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.UpdateAssetMasterGeneral;
using Core.Application.AssetMaster.AssetPurchase.Commands.UpdateAssetPurchaseDetails;
using FluentValidation;

namespace FAM.API.Validation.AssetMaster.AssetComposite
{
    public class UpdateAssetCompositeCommandValidator : AbstractValidator<UpdateAssetCompositeCommand>
    {
        public UpdateAssetCompositeCommandValidator(
            IValidator<UpdateAssetMasterGeneralCommand> masterValidator,
            IValidator<UpdateAssetPurchaseDetailCommand> purchaseValidator,
            IValidator<UpdateAssetLocationCommand> locationValidator)
        {
            RuleFor(x => x.AssetComposite)
                .NotNull().WithMessage("Asset composite data is required.");

            RuleFor(x => x.AssetComposite.UpdateAssetMaster)
                .NotNull().WithMessage("Asset master data is required.")
                .SetValidator(masterValidator);

            When(x => x.AssetComposite.UpdateAssetPurchaseDetail != null, () =>
            {
                RuleForEach(x => x.AssetComposite.UpdateAssetPurchaseDetail)
                    .SetValidator(purchaseValidator);
            });

            When(x => x.AssetComposite.UpdateAssetLocation != null, () =>
            {
                RuleForEach(x => x.AssetComposite.UpdateAssetLocation)
                    .SetValidator(locationValidator);
            });
        }
    }
}
