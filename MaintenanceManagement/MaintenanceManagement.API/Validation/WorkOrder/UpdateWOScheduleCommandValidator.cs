

using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Application.WorkOrder.Command.UpdateWorkOrder.Schedule;
using FluentValidation;

namespace MaintenanceManagement.API.Validation.WorkOrder
{
    public class UpdateWOScheduleCommandValidator  : AbstractValidator<UpdateWOScheduleCommand>
    {
        private readonly IWorkOrderCommandRepository _workOrderRepository;

        public UpdateWOScheduleCommandValidator(IWorkOrderCommandRepository workOrderRepository)
        {
            _workOrderRepository = workOrderRepository;

            RuleFor(x => x.WOSchedule)
                .NotNull()
                .WithMessage("WorkOrder schedule cannot be null.");

            When(x => x.WOSchedule != null, () =>
            {
                RuleFor(x => x.WOSchedule.WorkOrderId)
                    .NotNull()
                    .WithMessage("WorkOrderId is required.")
                    .MustAsync(WorkOrderExists)
                    .WithMessage("WorkOrderId does not exist.");

                RuleFor(x => x.WOSchedule.StartTime)
                    .NotEmpty()
                    .WithMessage("StartTime is required.");

                RuleFor(x => x.WOSchedule.EndTime)
                    .NotEmpty()
                    .WithMessage("EndTime is required.");

                RuleFor(x => x.WOSchedule)
                    .Must(schedule => schedule.StartTime < schedule.EndTime)
                    .WithMessage("StartTime must be before EndTime.");
            });
        }

        private async Task<bool> WorkOrderExists(int? workOrderId, CancellationToken cancellationToken)
        {
            if (!workOrderId.HasValue)
                return false;

            var workOrder = await _workOrderRepository.GetByIdAsync(workOrderId.Value);
            return workOrder != null;
        }
    }
}