using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineMaster.Queries.GetMachineMaster
{
    public class GetMachineMasterQueryHandler : IRequestHandler<GetMachineMasterQuery,ApiResponseDTO<List<MachineMasterDto>>>
    {
        private readonly IMachineMasterQueryRepository _imachineMasterQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
         public GetMachineMasterQueryHandler(IMachineMasterQueryRepository imachineMasterQueryRepository, IMapper mapper, IMediator mediator)
        {
            _imachineMasterQueryRepository = imachineMasterQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;   
        }

        public async Task<ApiResponseDTO<List<MachineMasterDto>>> Handle(GetMachineMasterQuery request, CancellationToken cancellationToken)
        {
           var (MachineMaster, totalCount) = await _imachineMasterQueryRepository.GetAllMachineAsync(request.PageNumber, request.PageSize, request.SearchTerm);
               var machineMastersgroup = _mapper.Map<List<MachineMasterDto>>(MachineMaster);

             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetMachineMasterQuery",
                    actionCode: "Get",        
                    actionName: MachineMaster.Count().ToString(),
                    details: $"MachineMaster details was fetched.",
                    module:"MachineMaster"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<MachineMasterDto>> 
            { 
                IsSuccess = true, 
                Message = "Success", 
                Data = machineMastersgroup ,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
                };
        }
    }
}