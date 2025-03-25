using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ICostCenter;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.CostCenter.Command.DeleteCostCenter
{
    public class DeleteCostCenterCommandHandler : IRequestHandler<DeleteCostCenterCommand, ApiResponseDTO<int>>
    {
        private readonly ICostCenterCommandRepository _iCostCenterCommandRepository;
        private readonly ICostCenterQueryRepository _iCostCenterQueryRepository;
        private readonly IMapper _Imapper;
        private readonly IMediator _mediator; 

        public DeleteCostCenterCommandHandler(ICostCenterCommandRepository iCostCenterCommandRepository, ICostCenterQueryRepository iCostCenterQueryRepository, IMapper imapper, IMediator mediator)
        {
            _iCostCenterCommandRepository = iCostCenterCommandRepository;
            _iCostCenterQueryRepository = iCostCenterQueryRepository;
            _Imapper = imapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<int>> Handle(DeleteCostCenterCommand request, CancellationToken cancellationToken)
        {
            // 🔹 First, check if the ID exists in the database
            // var existingcostcenter = await _iCostCenterQueryRepository.GetByIdAsync(request.Id);
            // if (existingcostcenter is null)
            // {
              
            //     return new ApiResponseDTO<int>
            //     {
            //         IsSuccess = false,
            //         Message = "CostCenter Id not found / CostCenter is deleted ."
            //     };
            // }

            var costCenterGroup = _Imapper.Map<Core.Domain.Entities.CostCenter>(request);
            var result = await _iCostCenterCommandRepository.DeleteAsync(request.Id,costCenterGroup);
            if (result == -1) 
            {
         
             return new ApiResponseDTO<int> { IsSuccess = false, Message = "CostCenter not found."};
            }

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Delete",
                actionCode: costCenterGroup.CostCenterCode,
                actionName: costCenterGroup.CostCenterName,
                details: $"CostCenter details was deleted",
                module: "CostCenter");
            await _mediator.Publish(domainEvent);
          

            return new ApiResponseDTO<int>
            {
                IsSuccess = true,   
                Data = result,
                Message = "CostCenter deleted successfully."
    
            };
        }


    }
}