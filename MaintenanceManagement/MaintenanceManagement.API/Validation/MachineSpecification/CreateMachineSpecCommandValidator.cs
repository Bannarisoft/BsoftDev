using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.IMachineSpecification;
using Core.Application.MachineSpecification.Command.CreateMachineSpecfication;
using FluentValidation;
using MaintenanceManagement.API.Validation.Common;

namespace MaintenanceManagement.API.Validation.MachineSpecification
{
    public class CreateMachineSpecCommandValidator : AbstractValidator<CreateMachineSpecficationCommand>
    {
          private readonly List<ValidationRule> _validationRules;
          private readonly IMachineSpecificationCommandRepository _iMachineSpecificationRepository;

        public CreateMachineSpecCommandValidator(MaxLengthProvider maxLengthProvider, IMachineSpecificationCommandRepository iMachineSpecificationRepository)
        {
            _iMachineSpecificationRepository = iMachineSpecificationRepository; 
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
                        RuleFor(x => x.SpecificationId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateMachineSpecficationCommand.SpecificationId)} {rule.Error}");
                        RuleFor(x => x.MachineId)
                           .NotEmpty()
                           .WithMessage($"{nameof(CreateMachineSpecficationCommand.MachineId)} {rule.Error}");
                        break;

                    case "MinLength":
                        RuleFor(x => x.SpecificationId)
                            .GreaterThanOrEqualTo(1)
                            .WithMessage($"{nameof(CreateMachineSpecficationCommand.SpecificationId)} {rule.Error} {0}");
                        RuleFor(x => x.MachineId)
                            .GreaterThanOrEqualTo(1)
                            .WithMessage($"{nameof(CreateMachineSpecficationCommand.MachineId)} {rule.Error} {0}");
                        break;

                    case "AlreadyExists":
                        RuleFor(x => x)
                            .MustAsync(async (model, cancellation) =>
                                !await _iMachineSpecificationRepository
                                    .IsDuplicateSpecificationAsync(model.MachineId, model.SpecificationId))
                            .WithName("SpecificationId")
                            .WithMessage($"{rule.Error}");
                        break;


                }
            }
        }
    }
}