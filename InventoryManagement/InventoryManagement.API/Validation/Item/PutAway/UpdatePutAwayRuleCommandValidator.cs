using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.Common.Interfaces.Item.PutAway;
using Core.Application.Item.PutAway.Commands.CreatePutAwayRule;
using Core.Application.Item.PutAway.Commands.UpdatePutAwayRule;
using FluentValidation;
using InventoryManagement.API.Validation.Common;

namespace InventoryManagement.API.Validation.Item.PutAway
{
    public class UpdatePutAwayRuleCommandValidator : AbstractValidator<UpdatePutAwayRuleCommand>
    {
        private readonly List<Common.ValidationRule> _validationRules;
        private readonly IPutAwayRuleCommandRepository _ruleRepo;
        private readonly IMiscMasterQueryRepository _miscRepo;     

        public UpdatePutAwayRuleCommandValidator(
            MaxLengthProvider maxLengthProvider,
            IPutAwayRuleCommandRepository ruleRepo,
            IMiscMasterQueryRepository miscRepo            )
        {
            _ruleRepo = ruleRepo;
            _miscRepo = miscRepo;   

            _validationRules = ValidationRuleLoader.LoadValidationRules();
            if (_validationRules == null || !_validationRules.Any())
                throw new ArgumentException("Validation rules could not be loaded.");

            // must have Id
            RuleFor(x => x.Id).GreaterThan(0).WithMessage($"{nameof(UpdatePutAwayRuleCommand.Id)} is required.");

            // entity must exist
            RuleFor(x => x.Id)
                .MustAsync(async (id, ct) => await _ruleRepo.ExistsAsync(id, ct))
                .WithMessage("PutAway Rule not found.");

            foreach (var rule in _validationRules)
            {
                switch (rule.Rule)
                {
                    case "NotEmpty":
                        RuleFor(x => x.Body.UnitId)
                            .NotEmpty().WithMessage($"{nameof(CreatePutAwayRuleRequest.UnitId)} {rule.Error}");
                        RuleFor(x => x.Body.WarehouseId)
                            .NotEmpty().WithMessage($"{nameof(CreatePutAwayRuleRequest.WarehouseId)} {rule.Error}");
                        RuleFor(x => x.Body.ItemGroupId)
                            .NotEmpty().WithMessage($"{nameof(CreatePutAwayRuleRequest.ItemGroupId)} {rule.Error}");
                        RuleFor(x => x.Body.ItemCategoryId)
                            .NotEmpty().WithMessage($"{nameof(CreatePutAwayRuleRequest.ItemCategoryId)} {rule.Error}");
                        RuleFor(x => x.Body.Strategies)
                            .NotEmpty().WithMessage($"{nameof(CreatePutAwayRuleRequest.Strategies)} {rule.Error}");
                        break;

                /*     case "AlreadyExists":
                        // Scope uniqueness excluding this Id
                        RuleFor(x => x)
                           .MustAsync(async (cmd, ct) =>
                               !await _ruleRepo.ExistsScopeAsync(
                                   cmd.Body.UnitId, cmd.Body.WarehouseId, cmd.Body.ItemGroupId, cmd.Body.ItemCategoryId, cmd.Body.ItemId, cmd.Id, ct))
                           .WithMessage("Another PutAway rule already exists for the given scope.");
                        break; */
                }
            }

            // Strategy-level basic numeric checks
            RuleForEach(x => x.Body.Strategies).ChildRules(s =>
            {
                s.RuleFor(y => y.StorageTypeId).GreaterThan(0)
                    .WithMessage($"{nameof(CreatePutAwayStrategyRequest.StorageTypeId)} is required.");
                s.RuleFor(y => y.PriorityId).GreaterThan(0)
                    .WithMessage($"{nameof(CreatePutAwayStrategyRequest.PriorityId)} is required.");
                 s.RuleFor(y => y.TargetId).GreaterThan(0)
                    .WithMessage($"{nameof(CreatePutAwayStrategyRequest.TargetId)} is required.");
            });

            // Unique priority
            RuleFor(x => x.Body.Strategies.Select(s => s.PriorityId))
                .Must(p => p.Count() == p.Distinct().Count())
                .WithMessage("Priority must be unique within the rule.");           
        }
    }
}
