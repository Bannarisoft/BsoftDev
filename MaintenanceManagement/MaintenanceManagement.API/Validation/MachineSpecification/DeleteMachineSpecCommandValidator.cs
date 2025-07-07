using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IMachineSpecification;
using Core.Application.MachineSpecification.DeleteMachineSpecfication;
using FluentValidation;
using MaintenanceManagement.API.Validation.Common;

namespace MaintenanceManagement.API.Validation.MachineSpecification
{
    public class DeleteMachineSpecCommandValidator : AbstractValidator<DeleteMachineSpecficationCommand>
    {

        private readonly List<ValidationRule> _validationRules;
        private readonly IMachineSpecificationQueryRepository _imachineSpecificationQueryRepository;

        public DeleteMachineSpecCommandValidator(IMachineSpecificationQueryRepository imachineSpecificationQueryRepository)
        {
            _validationRules = ValidationRuleLoader.LoadValidationRules();
            _imachineSpecificationQueryRepository = imachineSpecificationQueryRepository;
            if (_validationRules == null || !_validationRules.Any())
            {
                throw new InvalidOperationException("Validation rules could not be loaded.");
            }
                foreach (var rule in _validationRules)
            {
                switch (rule.Rule)
                {
                    case "NotEmpty":
                        RuleFor(x => x.Id)
                            .NotEmpty()
                            .WithMessage($"{nameof(DeleteMachineSpecficationCommand.Id)} {rule.Error}");
                        break;
                    case "RecordNotFound":
                        RuleFor(x => x.Id)
                            .MustAsync(async (id, cancellation) => 
                                (await _imachineSpecificationQueryRepository.GetBySpecificationIdAsync(id)) != null) 
                            .WithName("Id")
                            .WithMessage($"{rule.Error}");
                            break;
                    default:
                        break;
                }
            }
        }
        
    }
}