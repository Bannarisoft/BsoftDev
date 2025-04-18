

using Core.Application.WorkOrder.Command.UpdateWorkOrder;
using FluentValidation;
using MaintenanceManagement.API.Validation.Common;

namespace MaintenanceManagement.API.Validation.WorkOrder
{
    public class UpdateWorkOrderCommandValidator  : AbstractValidator<UpdateWorkOrderCommand>
    {
         private readonly List<ValidationRule> _validationRules;

        public UpdateWorkOrderCommandValidator(MaxLengthProvider maxLengthProvider)
        {
            // Get max lengths dynamically using MaxLengthProvider
            var woRemarksMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.WorkOrderMaster.WorkOrder>("AssetCode")??1000;
            var woItemMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.WorkOrderMaster.WorkOrderItem>("ItemName")??250;                        
            var woTechnicianMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.WorkOrderMaster.WorkOrderTechnician>("TechnicianName")??100;  
            var woActivityMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.WorkOrderMaster.WorkOrderActivity>("Description")??100; 
            var woCheckListMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.WorkOrderMaster.WorkOrderCheckList>("Description")??1000; 

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
                         RuleFor(x => x.WorkOrder.CompanyId)
                            .NotEmpty()
                            .WithMessage($"{nameof(WorkOrderUpdateDto.CompanyId)} {rule.Error}"); 
                        RuleFor(x => x.WorkOrder.UnitId)
                            .NotEmpty()
                            .WithMessage($"{nameof(WorkOrderUpdateDto.UnitId)} {rule.Error}");                        
                        //Item
                         RuleForEach(x => x.WorkOrder.WorkOrderItem)
                            .ChildRules(woItem =>
                            {
                                woItem.RuleFor(x => x.DepartmentId)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(WorkOrderItemUpdateDto.DepartmentId)} {rule.Error}");                                                                  
                                 woItem.RuleFor(x => x.AvailableQty)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(WorkOrderItemUpdateDto.AvailableQty)} {rule.Error}");
                                woItem.RuleFor(x => x.UsedQty)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(WorkOrderItemUpdateDto.UsedQty)} {rule.Error}");
                            });
                        //Activity
                        RuleForEach(x => x.WorkOrder.WorkOrderActivity)
                            .ChildRules(woActivity =>
                            {
                                woActivity.RuleFor(x => x.ActivityId)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(WorkOrderActivityUpdateDto.ActivityId)} {rule.Error}");                                                                   
                        });
                          //Technician
                        RuleForEach(x => x.WorkOrder.WorkOrderTechnician)
                            .ChildRules(woTechnician =>
                            {                               
                                woTechnician.RuleFor(x => x.HoursSpent)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(WorkOrderTechnicianUpdateDto.HoursSpent)} {rule.Error}");                                                               
                        });
                        //WorkOrderSchedule
                        RuleForEach(x => x.WorkOrder.WorkOrderSchedule)
                            .ChildRules(woTechnician =>
                            {
                                woTechnician.RuleFor(x => x.RepairStartTime)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(WorkOrderScheduleUpdateDto.RepairStartTime)} {rule.Error}");    
                                woTechnician.RuleFor(x => x.RepairEndTime)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(WorkOrderScheduleUpdateDto.RepairEndTime)} {rule.Error}");
                            });
                        break;
                    case "MaxLength":                                              
                        RuleFor(x => x.WorkOrder.Remarks)
                            .MaximumLength(woRemarksMaxLength) 
                            .WithMessage($"{nameof(UpdateWorkOrderCommand.WorkOrder.Remarks)} {rule.Error} {woRemarksMaxLength}");                                                                            
                        //Item
                        RuleForEach(x => x.WorkOrder.WorkOrderItem)
                            .ChildRules(woItem =>
                            {
                                woItem.RuleFor(x => x.ItemName)
                                    .MaximumLength(woItemMaxLength)
                                .WithMessage($"{nameof(WorkOrderItemUpdateDto.ItemName)} {rule.Error}{woItemMaxLength}");                              
                            });
                         //Technician
                        RuleForEach(x => x.WorkOrder.WorkOrderTechnician)
                            .ChildRules(woTechnician =>
                            {
                                woTechnician.RuleFor(x => x.TechnicianName)
                                    .MaximumLength(woTechnicianMaxLength)
                                .WithMessage($"{nameof(WorkOrderItemUpdateDto.ItemName)} {rule.Error}{woTechnicianMaxLength}");                              
                            }); 
                         //Activity
                        RuleForEach(x => x.WorkOrder.WorkOrderActivity)
                            .ChildRules(woActivity =>
                            {
                                woActivity.RuleFor(x => x.Description)
                                    .MaximumLength(woActivityMaxLength)
                                .WithMessage($"{nameof(WorkOrderActivityUpdateDto.Description)} {rule.Error}{woActivityMaxLength}");                              
                            });     
                         //CheckList
                        RuleForEach(x => x.WorkOrder.WorkOrderCheckList)
                            .ChildRules(woCheckList =>
                            {
                                woCheckList.RuleFor(x => x.Description)
                                    .MaximumLength(woActivityMaxLength)
                                .WithMessage($"{nameof(WorkOrderCheckListUpdateDto.Description)} {rule.Error}{woCheckListMaxLength}");                              
                            });                      
                        break;    
                     case "NumericOnly":       
                        RuleFor(x => x.WorkOrder.TotalManPower)
                            .InclusiveBetween(1, int.MaxValue)
                            .WithMessage($"{nameof(UpdateWorkOrderCommand.WorkOrder.TotalManPower)} {rule.Error}");           
                        RuleFor(x => x.WorkOrder.TotalSpentHours)
                            .InclusiveBetween(1, int.MaxValue)
                            .WithMessage($"{nameof(UpdateWorkOrderCommand.WorkOrder.TotalSpentHours)} {rule.Error}");  
                        //Item
                        RuleForEach(x => x.WorkOrder.WorkOrderItem)
                            .ChildRules(woItem =>
                            {                               
                                woItem.RuleFor(x => x.AvailableQty.ToString())
                                .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                                .WithMessage($"{nameof(WorkOrderItemUpdateDto.AvailableQty)} {rule.Error}");

                                 woItem.RuleFor(x => x.UsedQty.ToString())
                                .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                                .WithMessage($"{nameof(WorkOrderItemUpdateDto.UsedQty)} {rule.Error}");
                            });                      
                        //Technician                     
                        RuleForEach(x => x.WorkOrder.WorkOrderTechnician)
                            .ChildRules(woTechnician =>
                            {                               
                                woTechnician.RuleFor(x => x.HoursSpent.ToString())
                                .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                                .WithMessage($"{nameof(WorkOrderTechnicianUpdateDto.HoursSpent)} {rule.Error}");
                            });       
                        break;        
                }
            }  
        }
    }
}