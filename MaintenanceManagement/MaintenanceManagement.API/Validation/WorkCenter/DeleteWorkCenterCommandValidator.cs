using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IWorkCenter;
using Core.Application.WorkCenter.Command.DeleteWorkCenter;
using FluentValidation;
using MaintenanceManagement.API.Validation.Common;

namespace MaintenanceManagement.API.Validation.WorkCenter
{
    public class DeleteWorkCenterCommandValidator : AbstractValidator<DeleteWorkCenterCommand> 
    {
        private readonly List<ValidationRule> _validationRules;
        private readonly IWorkCenterQueryRepository _iWorkCenterQueryRepository;
         public DeleteWorkCenterCommandValidator(IWorkCenterQueryRepository iWorkCenterQueryRepository)
        {
            _iWorkCenterQueryRepository = iWorkCenterQueryRepository;
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
                        RuleFor(x => x.Id)
                            .NotEmpty()
                            .WithMessage($"{nameof(DeleteWorkCenterCommand.Id)} {rule.Error}");
                        break;
                    case "RecordNotFound":
                        RuleFor(x => x.Id)
                            .MustAsync(async (id, cancellation) => 
                                (await _iWorkCenterQueryRepository.GetByIdAsync(id)) != null) 
                            .WithName("Id")
                            .WithMessage($"{rule.Error}");
                            break;
                    case "SoftDelete":
                         RuleFor(x => x.Id)
                      .MustAsync(async (Id, cancellation) => !await _iWorkCenterQueryRepository.SoftDeleteValidation(Id))
                        .WithMessage($"{rule.Error}");
                        break;
                    default:
                        
                        break;
                }
            }
        }

    }
}