using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetAmc.Command.CreateAssetAmc;
using FAM.API.Validation.Common;
using FluentValidation;

namespace FAM.API.Validation.AssetMaster.AssetAmc
{
    public class CreateAssetAmcCommandValidator : AbstractValidator<CreateAssetAmcCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        public CreateAssetAmcCommandValidator(MaxLengthProvider maxLengthProvider)
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
                        RuleFor(x => x.AssetId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetAmcCommand.AssetId)} {rule.Error}");
                        RuleFor(x => x.StartDate)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetAmcCommand.StartDate)} {rule.Error}");
                        RuleFor(x => x.Period)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetAmcCommand.Period)} {rule.Error}");
                         RuleFor(x => x.VendorCode)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetAmcCommand.VendorCode)} {rule.Error}");
                        RuleFor(x => x.VendorName)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetAmcCommand.VendorName)} {rule.Error}"); 
                        RuleFor(x => x.CoverageType)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetAmcCommand.CoverageType)} {rule.Error}"); 
                         RuleFor(x => x.RenewalStatus)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetAmcCommand.RenewalStatus)} {rule.Error}"); 
                        break;
                     case "MaxLength":
                        RuleFor(x => x.AssetId.ToString())
                            .MaximumLength(AssetId)
                            .WithMessage($"{nameof(CreateAssetAmcCommand.AssetId)} {rule.Error} {AssetId}");
                        RuleFor(x => x.Period.ToString())
                            .MaximumLength(Period)
                            .WithMessage($"{nameof(CreateAssetAmcCommand.Period)} {rule.Error} {Period}");
                        RuleFor(x => x.VendorCode)
                            .MaximumLength(VendorCode)
                            .WithMessage($"{nameof(CreateAssetAmcCommand.VendorCode)} {rule.Error} {VendorCode}");
                        RuleFor(x => x.VendorName)
                            .MaximumLength(VendorName)
                            .WithMessage($"{nameof(CreateAssetAmcCommand.VendorName)} {rule.Error} {VendorName}");
                        RuleFor(x => x.VendorPhone)
                            .MaximumLength(VendorPhone)
                            .WithMessage($"{nameof(CreateAssetAmcCommand.VendorPhone)} {rule.Error} {VendorPhone}");
                        RuleFor(x => x.VendorEmail)
                            .MaximumLength(VendorEmail)
                            .WithMessage($"{nameof(CreateAssetAmcCommand.VendorEmail)} {rule.Error} {VendorEmail}");
                        RuleFor(x => x.CoverageType.ToString())
                            .MaximumLength(CoverageType)
                            .WithMessage($"{nameof(CreateAssetAmcCommand.CoverageType)} {rule.Error} {CoverageType}");
                        RuleFor(x => x.FreeServiceCount.ToString())
                            .MaximumLength(FreeServiceCount)
                            .WithMessage($"{nameof(CreateAssetAmcCommand.FreeServiceCount)} {rule.Error} {FreeServiceCount}");
                         RuleFor(x => x.RenewalStatus.ToString())
                            .MaximumLength(RenewalStatus)
                            .WithMessage($"{nameof(CreateAssetAmcCommand.RenewalStatus)} {rule.Error} {RenewalStatus}");
                        break;

                    case "MobileNumber": 
                         RuleFor(x => x.VendorPhone)
                            .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern)) 
                            .When(x => !string.IsNullOrEmpty(x.VendorPhone))
                            .WithMessage($"{nameof(VendorPhone)} {rule.Error}");
                            break;
                    case "Email":
                          RuleFor(x => x.VendorEmail)
                            .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern)) 
                            .When(x => !string.IsNullOrEmpty(x.VendorEmail))
                            .WithMessage($"{nameof(VendorEmail)} {rule.Error}");
                             break;


                        default:
                        // Handle unknown rule (log or throw)
                        Console.WriteLine($"Warning: Unknown rule '{rule.Rule}' encountered.");
                        break;
                }
            }
        }
    }
}