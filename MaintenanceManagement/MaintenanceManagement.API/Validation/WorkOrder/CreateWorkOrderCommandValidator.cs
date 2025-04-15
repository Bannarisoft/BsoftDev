using Core.Application.WorkOrder.Command.CreateWorkOrder;
using Core.Application.WorkOrder.Queries.GetWorkOrder;
using FluentValidation;
using MaintenanceManagement.API.Validation.Common;

namespace MaintenanceManagement.API.Validation.WorkOrder
{
    public class CreateWorkOrderCommandValidator : AbstractValidator<CreateWorkOrderCommand>
    {
         private readonly List<ValidationRule> _validationRules;

        public CreateWorkOrderCommandValidator(MaxLengthProvider maxLengthProvider)
        {
            // Get max lengths dynamically using MaxLengthProvider
            var woRemarksMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.WorkOrderMaster.WorkOrder>("Remarks")??1000;
            var woItemMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.WorkOrderMaster.WorkOrderItem>("ItemName")??100;                        
            var woTechnicianMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.WorkOrderMaster.WorkOrderTechnician>("TechnicianName")??100;  
            var woActivityMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.WorkOrderMaster.WorkOrderActivity>("Description")??250; 

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
                        RuleFor(x => x.WorkOrder.WorkOrderTypeId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateWorkOrderCommand.WorkOrder.WorkOrderTypeId)} {rule.Error}");                       
                        RuleFor(x => x.WorkOrder.RequestId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateWorkOrderCommand.WorkOrder.RequestId)} {rule.Error}");
                        RuleFor(x => x.WorkOrder.RequestTypeId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateWorkOrderCommand.WorkOrder.RequestTypeId)} {rule.Error}");
                        RuleFor(x => x.WorkOrder.PriorityId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateWorkOrderCommand.WorkOrder.PriorityId)} {rule.Error}");
                        RuleFor(x => x.WorkOrder.StatusId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateWorkOrderCommand.WorkOrder.StatusId)} {rule.Error}");
                        RuleFor(x => x.WorkOrder.RootCauseId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateWorkOrderCommand.WorkOrder.RootCauseId)} {rule.Error}");                        
                        //Item
                         RuleForEach(x => x.WorkOrder.WorkOrderItem)
                            .ChildRules(woItem =>
                            {
                                woItem.RuleFor(x => x.DepartmentId)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(WorkOrderItemDto.DepartmentId)} {rule.Error}");                                    
                                woItem.RuleFor(x => x.ItemCode)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(WorkOrderItemDto.ItemCode)} {rule.Error}"); 
                                 woItem.RuleFor(x => x.AvailableQty)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(WorkOrderItemDto.AvailableQty)} {rule.Error}");
                                woItem.RuleFor(x => x.UsedQty)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(WorkOrderItemDto.UsedQty)} {rule.Error}");
                            });
                        //Activity
                        RuleForEach(x => x.WorkOrder.WorkOrderActivity)
                            .ChildRules(woActivity =>
                            {
                                woActivity.RuleFor(x => x.ActivityId)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(WorkOrderActivityDto.ActivityId)} {rule.Error}");                                                                   
                        });
                          //Technician
                        RuleForEach(x => x.WorkOrder.WorkOrderTechnician)
                            .ChildRules(woTechnician =>
                            {
                                woTechnician.RuleFor(x => x.TechnicianId)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(WorkOrderTechnicianDto.TechnicianId)} {rule.Error}");    
                                woTechnician.RuleFor(x => x.TechnicianName)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(WorkOrderTechnicianDto.TechnicianName)} {rule.Error}");  
                                woTechnician.RuleFor(x => x.HoursSpent)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(WorkOrderTechnicianDto.HoursSpent)} {rule.Error}");                                                               
                        });
                        //WorkOrderSchedule
                        RuleForEach(x => x.WorkOrder.WorkOrderSchedule)
                            .ChildRules(woTechnician =>
                            {
                                woTechnician.RuleFor(x => x.RepairStartTime)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(WorkOrderScheduleDto.RepairStartTime)} {rule.Error}");    
                                woTechnician.RuleFor(x => x.RepairEndTime)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(WorkOrderScheduleDto.RepairEndTime)} {rule.Error}");
                            });
                        break;
                    case "MaxLength":                                              
                        RuleFor(x => x.WorkOrder.Remarks)
                            .MaximumLength(woRemarksMaxLength) 
                            .WithMessage($"{nameof(CreateWorkOrderCommand.WorkOrder.Remarks)} {rule.Error} {woRemarksMaxLength}");                                                                            
                        //Item
                        RuleForEach(x => x.WorkOrder.WorkOrderItem)
                            .ChildRules(woItem =>
                            {
                                woItem.RuleFor(x => x.ItemName)
                                    .MaximumLength(woItemMaxLength)
                                .WithMessage($"{nameof(WorkOrderItemDto.ItemName)} {rule.Error}{woItemMaxLength}");                              
                            });
                         //Technician
                        RuleForEach(x => x.WorkOrder.WorkOrderTechnician)
                            .ChildRules(woTechnician =>
                            {
                                woTechnician.RuleFor(x => x.TechnicianName)
                                    .MaximumLength(woTechnicianMaxLength)
                                .WithMessage($"{nameof(WorkOrderItemDto.ItemName)} {rule.Error}{woTechnicianMaxLength}");                              
                            });  
                          //Activity
                        RuleForEach(x => x.WorkOrder.WorkOrderActivity)
                            .ChildRules(woActivity =>
                            {
                                woActivity.RuleFor(x => x.Description)
                                    .MaximumLength(woActivityMaxLength)
                                .WithMessage($"{nameof(WorkOrderActivityDto.Description)} {rule.Error}{woActivityMaxLength}");                              
                            });                      
                        break;    
                     case "NumericOnly":       
                        RuleForEach(x => x.WorkOrder.WorkOrderItem)
                            .ChildRules(woItem =>
                            {                               
                                woItem.RuleFor(x => x.AvailableQty.ToString())
                                .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                                .WithMessage($"{nameof(WorkOrderItemDto.AvailableQty)} {rule.Error}");

                                 woItem.RuleFor(x => x.UsedQty.ToString())
                                .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                                .WithMessage($"{nameof(WorkOrderItemDto.UsedQty)} {rule.Error}");
                            });  
                        break;            
                     case "NumericWithDecimal":
                        RuleForEach(x => x.WorkOrder.WorkOrderTechnician)
                            .ChildRules(woTechnician =>
                            {                               
                                woTechnician.RuleFor(x => x.HoursSpent.ToString())
                                .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                                .WithMessage($"{nameof(WorkOrderTechnicianDto.HoursSpent)} {rule.Error}");
                            });                        
                        break;        
                }
            }  
        }
    }
}