using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ICostCenter;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.CostCenter.Command.UpdateCostCenter
{
    public class UpdateCostCenterCommandHandler  : IRequestHandler<UpdateCostCenterCommand, ApiResponseDTO<int>>
    {
        private readonly ICostCenterCommandRepository _iCostCenterCommandRepository;
        private readonly ICostCenterQueryRepository _iCostCenterQueryRepository;
        private readonly IMapper _Imapper;
        private readonly IMediator _mediator; 

        public UpdateCostCenterCommandHandler(ICostCenterCommandRepository iCostCenterCommandRepository, ICostCenterQueryRepository iCostCenterQueryRepository, IMapper imapper, IMediator mediator)
        {
            _iCostCenterCommandRepository = iCostCenterCommandRepository;
            _iCostCenterQueryRepository = iCostCenterQueryRepository;
            _Imapper = imapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<int>> Handle(UpdateCostCenterCommand request, CancellationToken cancellationToken)
        {
            // 🔹 First, check if the ID exists in the database
            // var existingcostcenter = await _iCostCenterQueryRepository.GetByIdAsync(request.Id);
            // if (existingcostcenter is null)
            // {
        
            // return new ApiResponseDTO<int>
            // {
            //     IsSuccess = false,
            //     Message = "CostCenter Id not found / CostCenter is deleted ."
            // };
            // }
       
            var costCenter = _Imapper.Map<Core.Domain.Entities.CostCenter>(request);
            var result = await _iCostCenterCommandRepository.UpdateAsync(request.Id, costCenter);
            if (result <= 0) // CostCenter not found
            {
               
                return new ApiResponseDTO<int> { IsSuccess = false, Message = "CostCenter  not found." };
            }
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Update",
                actionCode: costCenter.CostCenterCode,
                actionName: costCenter.CostCenterName,
                details: $"CostCenter details was updated",
                module: "CostCenter");
            await _mediator.Publish(domainEvent, cancellationToken);
           
            return new ApiResponseDTO<int> { IsSuccess = true, Message = "CostCenter Updated Successfully.", Data = result };   
        }
    }
}