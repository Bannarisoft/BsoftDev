using FluentValidation;
using Core.Application.Units.Commands.CreateUnit;
using System.Text.RegularExpressions;
using Core.Application.Units.Queries.GetUnits;
namespace BSOFT.API.Validation.Common.Unit
{
    public class CreateUnitCommandValidator : AbstractValidator<CreateUnitCommand>
    {
         private readonly List<ValidationRule> _validationRules;
      
        public CreateUnitCommandValidator(MaxLengthProvider maxLengthProvider)
        {
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
                        RuleFor(x => x.UnitDto.UnitName)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateUnitCommand.UnitDto.UnitName)} {rule.Error}");
                        RuleFor(x => x.UnitDto.ShortName)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateUnitCommand.UnitDto.ShortName)} {rule.Error}");
                        RuleFor(x => x.UnitDto.CompanyId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateUnitCommand.UnitDto.CompanyId)} {rule.Error}");
                        RuleFor(x => x.UnitDto.DivisionId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateUnitCommand.UnitDto.DivisionId)} {rule.Error}");
                        RuleFor(x => x.UnitDto.UnitHeadName)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateUnitCommand.UnitDto.UnitHeadName)} {rule.Error}");
                        RuleFor(x => x.UnitDto.CINNO)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateUnitCommand.UnitDto.CINNO)} {rule.Error}");
                        RuleForEach(x => x.UnitDto.UnitAddressDto)
                            .SetValidator(new UnitAddressDtoValidator());
                        RuleForEach(x => x.UnitDto.UnitContactsDto)
                            .SetValidator(new UnitContactsDtoValidator());
                        break;
                    case "MaxLength":
                        // Apply MaxLength validation using dynamic max length values
                        RuleFor(x => x.UnitDto.UnitName)
                            .MaximumLength(50) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(CreateUnitCommand.UnitDto.UnitName)} {rule.Error} {50}");
                        RuleFor(x => x.UnitDto.ShortName)
                            .MaximumLength(10) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(CreateUnitCommand.UnitDto.ShortName)} {rule.Error} {10}");
                        RuleFor(x => x.UnitDto.CompanyId.ToString())
                            .MaximumLength(4) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(CreateUnitCommand.UnitDto.CompanyId)} {rule.Error} {4}");
                        RuleFor(x => x.UnitDto.DivisionId.ToString())
                            .MaximumLength(4) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(CreateUnitCommand.UnitDto.DivisionId)} {rule.Error} {4}");
                        RuleFor(x => x.UnitDto.UnitHeadName)
                            .MaximumLength(50) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(CreateUnitCommand.UnitDto.UnitHeadName)} {rule.Error} {50}");
                        RuleFor(x => x.UnitDto.CINNO)
                            .MaximumLength(50) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(CreateUnitCommand.UnitDto.CINNO)} {rule.Error} {50}");
                        RuleForEach(x => x.UnitDto.UnitAddressDto)
                            .SetValidator(new UnitAddressDtoValidator());
                        RuleForEach(x => x.UnitDto.UnitContactsDto)
                            .SetValidator(new UnitContactsDtoValidator());
                        break;

                    
                    default:
                        // Handle unknown rule (log or throw)
                        Console.WriteLine($"Warning: Unknown rule '{rule.Rule}' encountered.");
                        break;
                }
            }

        }
    }

    public class UnitAddressDtoValidator : AbstractValidator<UnitAddressDto>
    {
         private readonly List<ValidationRule> _validationRulesAddress;
        public UnitAddressDtoValidator()
        {
            _validationRulesAddress = ValidationRuleLoader.LoadValidationRules();
            if (_validationRulesAddress == null || !_validationRulesAddress.Any())
            {
                throw new InvalidOperationException("Validation rules could not be loaded.");
            }
              foreach (var rule in _validationRulesAddress)
            {
                switch (rule.Rule)
                {
                    case "MobileNumber": 
                RuleFor(x => x.ContactNumber) 
                    .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern)) 
                    .WithMessage($"{nameof(UnitAddressDto.ContactNumber)} {rule.Error}"); 
                RuleFor(x => x.AlternateNumber)
                    .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern)) 
                    .When(x => !string.IsNullOrEmpty(x.AlternateNumber))
                    .WithMessage($"{nameof(UnitAddressDto.AlternateNumber)} {rule.Error}"); 
                     break;
                    
                    case "NotEmpty":
                RuleFor(x => x.CountryId.ToString())
                    .NotEmpty()
                    .WithMessage($"{nameof(UnitAddressDto.CountryId)} {rule.Error}");
                RuleFor(x => x.StateId.ToString())
                    .NotEmpty()
                    .WithMessage($"{nameof(UnitAddressDto.StateId)} {rule.Error}");
                RuleFor(x => x.CityId.ToString())
                    .NotEmpty()
                    .WithMessage($"{nameof(UnitAddressDto.CityId)} {rule.Error}");
                RuleFor(x => x.AddressLine1)
                    .NotEmpty()
                    .WithMessage($"{nameof(UnitAddressDto.AddressLine1)} {rule.Error}");
                RuleFor(x => x.PinCode.ToString())
                    .NotEmpty()
                    .WithMessage($"{nameof(UnitAddressDto.PinCode)} {rule.Error}");
                RuleFor(x => x.ContactNumber)
                    .NotEmpty()
                    .WithMessage($"{nameof(UnitAddressDto.ContactNumber)} {rule.Error}");
                    break;

                     case "MaxLength":
                RuleFor(x => x.AddressLine1)
                    .MaximumLength(250)
                    .WithMessage($"{nameof(UnitAddressDto.AddressLine1)} {rule.Error} {250}");
                 RuleFor(x => x.AddressLine2)
                    .MaximumLength(250)
                    .WithMessage($"{nameof(UnitAddressDto.AddressLine2)} {rule.Error} {250}");
                RuleFor(x => x.CountryId.ToString())
                    .MaximumLength(4)
                    .WithMessage($"{nameof(UnitAddressDto.CountryId)} {rule.Error} {4}");
                RuleFor(x => x.StateId.ToString())
                    .MaximumLength(4)
                    .WithMessage($"{nameof(UnitAddressDto.StateId)} {rule.Error} {4}");
                RuleFor(x => x.CityId.ToString())
                    .MaximumLength(4)
                    .WithMessage($"{nameof(UnitAddressDto.CityId)} {rule.Error} {4}");
                 RuleFor(x => x.PinCode.ToString())
                    .MaximumLength(10)
                    .WithMessage($"{nameof(UnitAddressDto.PinCode)} {rule.Error} {10}");
                RuleFor(x => x.ContactNumber)
                    .MaximumLength(40)
                    .WithMessage($"{nameof(UnitAddressDto.ContactNumber)} {rule.Error} {40}");
                RuleFor(x => x.AlternateNumber)
                    .MaximumLength(40)
                    .WithMessage($"{nameof(UnitAddressDto.AlternateNumber)} {rule.Error} {40}"); 
                     break;  
                       
                    default:
                  // Handle unknown rule (log or throw)
                    Console.WriteLine($"Warning: Unknown rule '{rule.Rule}' encountered.");
                    break;
                         
                }
            }
       
        }   
    }

      public class UnitContactsDtoValidator : AbstractValidator<UnitContactsDto>
    {
         private readonly List<ValidationRule> _validationRulescontacts;
        public UnitContactsDtoValidator()
        {
            _validationRulescontacts = ValidationRuleLoader.LoadValidationRules();
            if (_validationRulescontacts == null || !_validationRulescontacts.Any())
            {
                throw new InvalidOperationException("Validation rules could not be loaded.");
            }
              foreach (var rule in _validationRulescontacts)
            {
                switch (rule.Rule)
                {
                     case "MobileNumber":
                RuleFor(x => x.PhoneNo) 
                    .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern)) 
                    .WithMessage($"{nameof(UnitContactsDto.PhoneNo)} {rule.Error}"); 
                     break;
                     case "Email":
                RuleFor(x => x.Email) 
                    .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern)) 
                    .WithMessage($"{nameof(UnitContactsDto.Email)} {rule.Error}");
                     break;

                     case "NotEmpty":
                RuleFor(x => x.Name)
                    .NotEmpty()     
                    .WithMessage($"{nameof(UnitContactsDto.Name)} {rule.Error}");    
                RuleFor(x => x.Designation)
                    .NotEmpty()
                    .WithMessage($"{nameof(UnitContactsDto.Designation)} {rule.Error}");
                RuleFor(x => x.Email)
                    .NotEmpty()
                    .WithMessage($"{nameof(UnitContactsDto.Email)} {rule.Error}");
                RuleFor(x => x.PhoneNo)
                    .NotEmpty()
                    .WithMessage($"{nameof(UnitContactsDto.PhoneNo)} {rule.Error}");
                    break;

                     case "MaxLength":
                RuleFor(x => x.Name)
                    .MaximumLength(50) 
                    .WithMessage($"{nameof(UnitContactsDto.Name)} {rule.Error} {50}"); 
                RuleFor(x => x.Designation)
                    .MaximumLength(50)
                    .WithMessage($"{nameof(UnitContactsDto.Designation)} {rule.Error} {50}");
                RuleFor(x => x.Email)
                    .MaximumLength(200)
                    .WithMessage($"{nameof(UnitContactsDto.Email)} {rule.Error} {200}");
                RuleFor(x => x.PhoneNo)
                    .MaximumLength(40)
                    .WithMessage($"{nameof(UnitContactsDto.PhoneNo)} {rule.Error} {40}");
                 RuleFor(x => x.Remarks)
                    .MaximumLength(250)
                    .When(x => !string.IsNullOrEmpty(x.Remarks))
                    .WithMessage($"{nameof(UnitContactsDto.Remarks)} {rule.Error} {250}");
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

