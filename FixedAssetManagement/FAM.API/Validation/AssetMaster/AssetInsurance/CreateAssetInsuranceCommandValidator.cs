using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetInsurance.Commands.CreateAssetInsurance;
using FAM.API.Validation.Common;
using FluentValidation;

namespace FAM.API.Validation.AssetMaster.AssetInsurance
{
    public class CreateAssetInsuranceCommandValidator  : AbstractValidator<CreateAssetInsuranceCommand>
    {
          private readonly List<ValidationRule> _validationRules;
            public CreateAssetInsuranceCommandValidator()
        {    
            
            _validationRules = new List<ValidationRule>();
            _validationRules = ValidationRuleLoader.LoadValidationRules();
                 if (!_validationRules.Any())
            {
                throw new ArgumentException("Validation rules could not be loaded.");
            }

            foreach (var rule in _validationRules)
            {
                switch (rule.Rule)
                {
                    case "NotEmpty":
                        RuleFor(x => x.AssetId)
                            .NotEmpty().WithMessage("Asset ID is required.");

                        RuleFor(x => x.PolicyNo)
                            .NotEmpty().WithMessage("Policy number is required.");

                        RuleFor(x => x.StartDate)
                            .LessThan(x => x.EndDate).WithMessage("Start date must be before end date.");

                        RuleFor(x => x.EndDate)
                            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");

                        RuleFor(x => x.PolicyAmount)
                            .NotEmpty().WithMessage("Policy amount is required.")
                            .Matches("^\\d+(\\.\\d{1,2})?$").WithMessage("Invalid policy amount format.");

                        RuleFor(x => x.VendorCode)
                            .NotEmpty().WithMessage("Vendor code is required.");

                        RuleFor(x => x.RenewedDate)
                            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("Renewed date must be after or equal to the start date.");

                        RuleFor(x => x.RenewalStatus)
                            .NotEmpty().WithMessage("Renewal status is required.");

                        RuleFor(x => x.InsuranceStatus)
                            .NotEmpty().WithMessage("Insurance status is required.");
                        break;

                }
            }    



        }

    }
}