using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetAmc.Command.UpdateAssetAmc;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetAmc;
using FAM.API.Validation.Common;
using FluentValidation;
using Serilog;

namespace FAM.API.Validation.AssetMaster.AssetAmc
{
    public class UpdateAssetAmcCommandValidator : AbstractValidator<UpdateAssetAmcCommand>
    {
         private readonly List<ValidationRule> _validationRules;
         private readonly IAssetAmcQueryRepository _assetAmcQueryRepository;
           public UpdateAssetAmcCommandValidator(MaxLengthProvider maxLengthProvider, IAssetAmcQueryRepository assetAmcQueryRepository)
        {
           
            var AssetId = maxLengthProvider.GetMaxLength<Core.Domain.Entities.AssetMaster.AssetAmc>("AssetId") ?? 10;
            var Period = maxLengthProvider.GetMaxLength<Core.Domain.Entities.AssetMaster.AssetAmc>("Period") ?? 10;
            var VendorCode = maxLengthProvider.GetMaxLength<Core.Domain.Entities.AssetMaster.AssetAmc>("VendorCode") ?? 40;
            var VendorName = maxLengthProvider.GetMaxLength<Core.Domain.Entities.AssetMaster.AssetAmc>("VendorName") ?? 400;
            var VendorPhone = maxLengthProvider.GetMaxLength<Core.Domain.Entities.AssetMaster.AssetAmc>("VendorPhone") ?? 80;
            var VendorEmail = maxLengthProvider.GetMaxLength<Core.Domain.Entities.AssetMaster.AssetAmc>("VendorEmail") ?? 200;
            var CoverageType = maxLengthProvider.GetMaxLength<Core.Domain.Entities.AssetMaster.AssetAmc>("CoverageType") ?? 10;
            var FreeServiceCount = maxLengthProvider.GetMaxLength<Core.Domain.Entities.AssetMaster.AssetAmc>("FreeServiceCount") ?? 10;
            var RenewalStatus = maxLengthProvider.GetMaxLength<Core.Domain.Entities.AssetMaster.AssetAmc>("RenewalStatus") ?? 10;


               // Load validation rules from JSON or another source
            _validationRules = ValidationRuleLoader.LoadValidationRules();
            _assetAmcQueryRepository = assetAmcQueryRepository;
            if (_validationRules == null || !_validationRules.Any())
            {
                throw new InvalidOperationException("Validation rules could not be loaded.");
            }

             foreach (var rule in _validationRules)
            {
                 switch (rule.Rule)
                {
                    case "NotEmpty":
                        // Apply NotEmpty validation
                        RuleFor(x => x.StartDate)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateAssetAmcCommand.StartDate)} {rule.Error}");
                        RuleFor(x => x.Period)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateAssetAmcCommand.Period)} {rule.Error}");
                         RuleFor(x => x.VendorCode)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateAssetAmcCommand.VendorCode)} {rule.Error}");
                        RuleFor(x => x.VendorName)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateAssetAmcCommand.VendorName)} {rule.Error}"); 
                        RuleFor(x => x.CoverageType)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateAssetAmcCommand.CoverageType)} {rule.Error}"); 
                         RuleFor(x => x.RenewalStatus)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateAssetAmcCommand.RenewalStatus)} {rule.Error}");
                         RuleFor(x => x.IsActive)
                            .NotNull()
                            .WithMessage($"{nameof(UpdateAssetAmcCommand.IsActive)} {rule.Error}")
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateAssetAmcCommand.IsActive)} {rule.Error}");  
                        break;
                     case "MaxLength":
                        RuleFor(x => x.Period.ToString())
                            .MaximumLength(Period)
                            .WithMessage($"{nameof(UpdateAssetAmcCommand.Period)} {rule.Error} {Period}");
                        RuleFor(x => x.VendorCode)
                            .MaximumLength(VendorCode)
                            .WithMessage($"{nameof(UpdateAssetAmcCommand.VendorCode)} {rule.Error} {VendorCode}");
                        RuleFor(x => x.VendorName)
                            .MaximumLength(VendorName)
                            .WithMessage($"{nameof(UpdateAssetAmcCommand.VendorName)} {rule.Error} {VendorName}");
                        RuleFor(x => x.VendorPhone)
                            .MaximumLength(VendorPhone)
                            .WithMessage($"{nameof(UpdateAssetAmcCommand.VendorPhone)} {rule.Error} {VendorPhone}");
                        RuleFor(x => x.VendorEmail)
                            .MaximumLength(VendorEmail)
                            .WithMessage($"{nameof(UpdateAssetAmcCommand.VendorEmail)} {rule.Error} {VendorEmail}");
                        RuleFor(x => x.CoverageType.ToString())
                            .MaximumLength(CoverageType)
                            .WithMessage($"{nameof(UpdateAssetAmcCommand.CoverageType)} {rule.Error} {CoverageType}");
                        RuleFor(x => x.FreeServiceCount.ToString())
                            .MaximumLength(FreeServiceCount)
                            .WithMessage($"{nameof(UpdateAssetAmcCommand.FreeServiceCount)} {rule.Error} {FreeServiceCount}");
                         RuleFor(x => x.RenewalStatus.ToString())
                            .MaximumLength(RenewalStatus)
                            .WithMessage($"{nameof(UpdateAssetAmcCommand.RenewalStatus)} {rule.Error} {RenewalStatus}");
                        break;

                         case "MobileNumber": 
                         RuleFor(x => x.VendorPhone)
                            .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern)) 
                            .When(x => !string.IsNullOrEmpty(x.VendorPhone))
                            .WithMessage($"{nameof(VendorPhone)} {rule.Error}");
                            break;
                    case "Email":
                          RuleFor(x => x.VendorEmail)
                            .EmailAddress() 
                            .When(x => !string.IsNullOrEmpty(x.VendorEmail))
                            .WithMessage($"{nameof(VendorEmail)} {rule.Error}");
                             break;
                     case "ActivePolicy":
                           RuleFor(x => new { x.AssetId, x.Id })
                           .MustAsync(async (insurance, cancellation) => !await _assetAmcQueryRepository.ActiveAMCValidation(insurance.AssetId, insurance.Id))
                            .WithMessage($"{rule.Error}")
                            .When(x => x.IsActive == 1);
                            break;
                        default:
                        // Handle unknown rule (log or throw)
                        Log.Information("Warning: Unknown rule '{Rule}' encountered.", rule.Rule);
                        break;
                }
            }
        }
    }
}