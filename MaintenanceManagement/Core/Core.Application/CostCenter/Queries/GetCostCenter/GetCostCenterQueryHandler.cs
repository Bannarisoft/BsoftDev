using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ICostCenter;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.CostCenter.Queries.GetCostCenter
{
    public class GetCostCenterQueryHandler : IRequestHandler<GetCostCenterQuery,ApiResponseDTO<List<CostCenterDto>>>
    {
        private readonly ICostCenterQueryRepository _iCostCenterQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetCostCenterQueryHandler(ICostCenterQueryRepository iCostCenterQueryRepository, IMapper mapper, IMediator mediator)
        {
            _iCostCenterQueryRepository = iCostCenterQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<CostCenterDto>>> Handle(GetCostCenterQuery request, CancellationToken cancellationToken)
        {
              var (CostCenter, totalCount) = await _iCostCenterQueryRepository.GetAllCostCenterGroupAsync(request.PageNumber, request.PageSize, request.SearchTerm);
               var costCentersgrouplist = _mapper.Map<List<CostCenterDto>>(CostCenter);

             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetCostCenter",
                    actionCode: "Get",        
                    actionName: CostCenter.Count().ToString(),
                    details: $"CostCenter details was fetched.",
                    module:"CostCenter"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<CostCenterDto>> 
            { 
                IsSuccess = true, 
                Message = "Success", 
                Data = costCentersgrouplist ,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
                };
        }
    }
}