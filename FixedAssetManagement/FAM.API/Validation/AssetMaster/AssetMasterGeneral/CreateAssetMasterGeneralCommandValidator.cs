

using Core.Application.AssetLocation.Commands.CreateAssetLocation;
using Core.Application.AssetMaster.AssetAdditionalCost.Commands.CreateAssetAdditionalCost;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.CreateAssetMasterGeneral;
using Core.Application.AssetMaster.AssetPurchase.Commands.CreateAssetPurchaseDetails;
using Core.Domain.Entities;
using Core.Domain.Entities.AssetPurchase;
using FAM.API.Validation.Common;
using FluentValidation;

namespace FAM.API.Validation.AssetMaster.AssetMasterGeneral
{
    public class CreateAssetMasterGeneralCommandValidator : AbstractValidator<CreateAssetMasterGeneralCommand>
    {
        private readonly List<ValidationRule> _validationRules;

        public CreateAssetMasterGeneralCommandValidator(MaxLengthProvider maxLengthProvider)
        {
            // Get max lengths dynamically using MaxLengthProvider
            var assetMasterGeneralCodeMaxLength = maxLengthProvider.GetMaxLength<AssetMasterGenerals>("AssetCode")??50;
            var assetMasterGeneralNameMaxLength = maxLengthProvider.GetMaxLength<AssetMasterGenerals>("AssetName")??100;                        
            var assetMasterGeneralMachineCodeMaxLength = maxLengthProvider.GetMaxLength<AssetMasterGenerals>("MachineCode")??50;  

            var BudgetType = maxLengthProvider.GetMaxLength<AssetPurchaseDetails>("BudgetType") ?? 50;
            var OldUnitId = maxLengthProvider.GetMaxLength<AssetPurchaseDetails>("OldUnitId") ?? 2;
            var VendorCode = maxLengthProvider.GetMaxLength<AssetPurchaseDetails>("VendorCode") ?? 50;
            var VendorName = maxLengthProvider.GetMaxLength<AssetPurchaseDetails>("VendorName") ?? 400;
            var PoNo = maxLengthProvider.GetMaxLength<AssetPurchaseDetails>("PoNo") ?? 10;
            var PoSno = maxLengthProvider.GetMaxLength<AssetPurchaseDetails>("PoSno") ?? 4;
            var ItemCode = maxLengthProvider.GetMaxLength<AssetPurchaseDetails>("ItemCode") ?? 100;
            var ItemName = maxLengthProvider.GetMaxLength<AssetPurchaseDetails>("ItemName") ?? 500;
            var GrnNo = maxLengthProvider.GetMaxLength<AssetPurchaseDetails>("GrnNo") ?? 10;
            var GrnSno = maxLengthProvider.GetMaxLength<AssetPurchaseDetails>("GrnSno") ?? 4;
            var BillNo = maxLengthProvider.GetMaxLength<AssetPurchaseDetails>("BillNo") ?? 100;
            var Uom = maxLengthProvider.GetMaxLength<AssetPurchaseDetails>("Uom") ?? 20;
            var BinLocation = maxLengthProvider.GetMaxLength<AssetPurchaseDetails>("BinLocation") ?? 100;
            var PjYear = maxLengthProvider.GetMaxLength<AssetPurchaseDetails>("BinLocation") ?? 8;
            var PjDocId = maxLengthProvider.GetMaxLength<AssetPurchaseDetails>("PjYear") ?? 40;
            var PjDocSr = maxLengthProvider.GetMaxLength<AssetPurchaseDetails>("PjDocSr") ?? 40;
            var PjDocNo = maxLengthProvider.GetMaxLength<AssetPurchaseDetails>("PjDocNo") ?? 10;
            var AssetId = maxLengthProvider.GetMaxLength<AssetPurchaseDetails>("AssetId") ?? 10;
            var AssetSourceId = maxLengthProvider.GetMaxLength<AssetPurchaseDetails>("AssetSourceId") ?? 10;
            
            var JournalNo = maxLengthProvider.GetMaxLength<Core.Domain.Entities.AssetPurchase.AssetAdditionalCost>("JournalNo") ?? 100;
            var CostType = maxLengthProvider.GetMaxLength<Core.Domain.Entities.AssetPurchase.AssetAdditionalCost>("CostType") ?? 10;

            // Load validation rules from JSON or another source
            _validationRules = ValidationRuleLoader.LoadValidationRules();
            if (_validationRules is null || !_validationRules.Any())
            {
                throw new InvalidOperationException("Validation rules could not be loaded.");
            }

            // Loop through the rules and apply them
            foreach (var rule in _validationRules)
            {
                switch (rule.Rule)
                {
                    case "NotEmpty":                        
                        RuleFor(x => x.AssetMaster.AssetName ?? string.Empty)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetMaster.AssetName)} {rule.Error}");                       
                        RuleFor(x => x.AssetMaster.AssetGroupId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetMaster.AssetGroupId)} {rule.Error}");
                        RuleFor(x => x.AssetMaster.CompanyId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetMaster.CompanyId)} {rule.Error}");
                        RuleFor(x => x.AssetMaster.AssetGroupId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetMaster.AssetGroupId)} {rule.Error}");
                        RuleFor(x => x.AssetMaster.AssetCategoryId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetMaster.AssetCategoryId)} {rule.Error}");
                        RuleFor(x => x.AssetMaster.AssetSubCategoryId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetMaster.AssetSubCategoryId)} {rule.Error}");   
                        RuleFor(x => x.AssetMaster.AssetType)
                            .NotEmpty()                            
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetMaster.AssetType)} {rule.Error}");   
                        RuleFor(x => x.AssetMaster.Quantity)
                            .NotEmpty()                            
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetMaster.Quantity)} {rule.Error}");    
                        RuleFor(x => x.AssetMaster.UOMId)
                            .NotEmpty()                            
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetMaster.UOMId)} {rule.Error}");    
                        RuleFor(x => x.AssetMaster.AssetDescription)
                            .NotEmpty()                            
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetMaster.AssetDescription)} {rule.Error}");    
                        RuleFor(x => x.AssetMaster.WorkingStatus)
                            .NotEmpty()                            
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetMaster.WorkingStatus)} {rule.Error}");   
                        //Location
                        RuleFor(x => x.AssetMaster.AssetLocation.UnitId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetLocationCommand.UnitId)} {rule.Error}")                            
                            .WithMessage($"{nameof(CreateAssetLocationCommand.UnitId)} must be a valid number.");
                         RuleFor(x => x.AssetMaster.AssetLocation.DepartmentId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetLocationCommand.DepartmentId)} {rule.Error}")                            
                            .WithMessage($"{nameof(CreateAssetLocationCommand.DepartmentId)} must be a valid number.");    
                        RuleFor(x => x.AssetMaster.AssetLocation.LocationId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetLocationCommand.LocationId)} {rule.Error}")                            
                            .WithMessage($"{nameof(CreateAssetLocationCommand.LocationId)} must be a valid number.");
                            RuleFor(x => x.AssetMaster.AssetLocation.SubLocationId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetLocationCommand.SubLocationId)} {rule.Error}")                            
                            .WithMessage($"{nameof(CreateAssetLocationCommand.SubLocationId)} must be a valid number.");
                        //Additional Cost
                         RuleForEach(x => x.AssetMaster.AssetAdditionalCost)
                            .ChildRules(additionalCost =>
                            {
                                additionalCost.RuleFor(x => x.JournalNo)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(CreateAssetAdditionalCostCommand.JournalNo)} {rule.Error}");
                                additionalCost.RuleFor(x => x.CostType)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(CreateAssetAdditionalCostCommand.CostType)} {rule.Error}");
                                additionalCost.RuleFor(x => x.Amount)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(CreateAssetAdditionalCostCommand.Amount)} {rule.Error}");
                            });
                        //Purchase
                        RuleForEach(x => x.AssetMaster.AssetPurchaseDetails)
                            .ChildRules(purchase =>
                            {
                                purchase.RuleFor(x => x.OldUnitId)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.OldUnitId)} {rule.Error}");
                                purchase.RuleFor(x => x.VendorCode)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.VendorCode)} {rule.Error}");
                                  purchase.RuleFor(x => x.VendorName)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.VendorName)} {rule.Error}");
                                  purchase.RuleFor(x => x.PoNo)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.PoNo)} {rule.Error}");
                                  purchase.RuleFor(x => x.PoSno)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.PoSno)} {rule.Error}");
                                  purchase.RuleFor(x => x.ItemCode)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.ItemCode)} {rule.Error}");
                                      purchase.RuleFor(x => x.ItemName)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.ItemName)} {rule.Error}");
                                  purchase.RuleFor(x => x.GrnNo)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.GrnNo)} {rule.Error}");
                                  purchase.RuleFor(x => x.GrnSno)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.GrnSno)} {rule.Error}");
                                  purchase.RuleFor(x => x.AcceptedQty)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.AcceptedQty)} {rule.Error}");
                                purchase.RuleFor(x => x.PurchaseValue)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.PurchaseValue)} {rule.Error}");
                                purchase.RuleFor(x => x.GrnValue)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.GrnValue)} {rule.Error}");
                                purchase.RuleFor(x => x.BillNo)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.BillNo)} {rule.Error}");
                                purchase.RuleFor(x => x.PjYear)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.PjYear)} {rule.Error}");
                                purchase.RuleFor(x => x.PjDocId)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.PjDocId)} {rule.Error}");
                                purchase.RuleFor(x => x.PjDocNo)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.PjDocNo)} {rule.Error}");                            
                                purchase.RuleFor(x => x.AssetSourceId)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.AssetSourceId)} {rule.Error}");
                            });
                            break;
                    case "MaxLength":                                              
                        RuleFor(x => x.AssetMaster.AssetName)
                            .MaximumLength(assetMasterGeneralNameMaxLength) 
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetMaster.AssetName)} {rule.Error} {assetMasterGeneralNameMaxLength}");                                                     
                        RuleFor(x => x.AssetMaster.AssetDescription)
                            .MaximumLength(assetMasterGeneralMachineCodeMaxLength) 
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetMaster.MachineCode)} {rule.Error} {assetMasterGeneralMachineCodeMaxLength}");
                        //Additional Cost
                        RuleForEach(x => x.AssetMaster.AssetAdditionalCost)
                            .ChildRules(additionalCost =>
                            {
                                additionalCost.RuleFor(x => x.JournalNo)
                                    .MaximumLength(JournalNo)
                                .WithMessage($"{nameof(CreateAssetAdditionalCostCommand.JournalNo)} {rule.Error}{JournalNo}");
                                additionalCost.RuleFor(x => x.CostType.ToString())
                                    .MaximumLength(CostType)
                                .WithMessage($"{nameof(CreateAssetAdditionalCostCommand.CostType)} {rule.Error}{CostType}");
                            });                       
                        //Purchase 
                        RuleForEach(x => x.AssetMaster.AssetPurchaseDetails)
                            .ChildRules(purchase =>
                            {
                                purchase.RuleFor(x => x.VendorCode)
                                     .MaximumLength(VendorCode)
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.VendorCode)} {rule.Error}{VendorCode}");
                                purchase.RuleFor(x => x.VendorName)
                                     .MaximumLength(VendorName)
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.VendorName)} {rule.Error}{VendorName}");
                                purchase.RuleFor(x => x.PoNo.ToString())
                                     .MaximumLength(PoNo)
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.PoNo)} {rule.Error}{PoNo}");
                                purchase.RuleFor(x => x.PoSno.ToString())
                                     .MaximumLength(PoSno)
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.PoSno)} {rule.Error}{PoSno}");
                                purchase.RuleFor(x => x.ItemCode)
                                     .MaximumLength(ItemCode)
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.ItemCode)} {rule.Error}{ItemCode}");
                                purchase.RuleFor(x => x.ItemName)
                                     .MaximumLength(ItemName)
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.ItemName)} {rule.Error}{ItemName}");
                                purchase.RuleFor(x => x.GrnNo.ToString())
                                     .MaximumLength(GrnNo)
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.GrnNo)} {rule.Error}{GrnNo}");
                                purchase.RuleFor(x => x.GrnSno.ToString())
                                     .MaximumLength(GrnSno)
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.GrnSno)} {rule.Error}{GrnSno}");
                                purchase.RuleFor(x => x.BillNo.ToString())
                                     .MaximumLength(BillNo)
                                    .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.BillNo)} {rule.Error}{BillNo}");
                     
                                purchase.RuleFor(x => x.PjYear.ToString())
                                                .MaximumLength(PjYear)
                                                .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.PjYear)} {rule.Error}{PjYear}");
                                purchase.RuleFor(x => x.PjDocId.ToString())
                                                .MaximumLength(PjDocId)
                                                .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.PjDocId)} {rule.Error}{PjDocId}");
                                purchase.RuleFor(x => x.PjDocNo.ToString())
                                                .MaximumLength(PjDocNo)
                                                .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.PjDocNo)} {rule.Error}{PjDocNo}");
                                purchase.RuleFor(x => x.AssetSourceId.ToString())
                                                .MaximumLength(AssetSourceId)
                                                .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.AssetSourceId)} {rule.Error}{AssetSourceId}");
                        });                                          
                        break;          
                    case "NumericOnly":       
                        RuleFor(x => x.AssetMaster.Quantity)
                            .InclusiveBetween(1, int.MaxValue)
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetMaster.Quantity)} {rule.Error}");                                                
                        break;
                    case "NumericWithDecimal":
                        RuleForEach(x => x.AssetMaster.AssetAdditionalCost)
                            .ChildRules(additionalCost =>
                            {                               
                                additionalCost.RuleFor(x => x.Amount.ToString())
                                .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                                .WithMessage($"{nameof(CreateAssetAdditionalCostCommand.Amount)} {rule.Error}");
                            });                        
                        break;
                    case "YesNoStatus":
                        RuleForEach(x => x.AssetMaster.AssetPurchaseDetails)
                            .ChildRules(purchase =>
                            {
                                 purchase.RuleFor(x => x.QcCompleted)
                                      .NotEmpty()
                                .Must(value => value.HasValue && System.Text.RegularExpressions.Regex.IsMatch(value.Value.ToString(), rule.Pattern))
                                .WithMessage($"{nameof(CreateAssetPurchaseDetailCommand.QcCompleted)} {rule.Error}");
                            });                              
                         break;   
                    default:                        
                    break;
                }
            }
        }
    }
}