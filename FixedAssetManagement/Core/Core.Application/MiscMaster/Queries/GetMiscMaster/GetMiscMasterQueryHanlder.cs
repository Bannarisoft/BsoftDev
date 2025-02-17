using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Domain.Events;
using MediatR;
using Core.Application.MiscMaster.Queries.GetMiscMaster;

namespace Core.Application.MiscMaster.Queries.GetMiscMaster
{
    public class GetMiscMasterQueryHanlder :IRequestHandler<GetMiscMasterQuery,ApiResponseDTO<List<GetMiscMasterDto>>> 
    {

         private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
            private readonly IMapper _mapper;
            private readonly IMediator _mediator;
     
        public GetMiscMasterQueryHanlder (IMiscMasterQueryRepository miscMasterQueryRepository, IMapper mapper, IMediator mediator)
        {
             _miscMasterQueryRepository = miscMasterQueryRepository;
             _mapper =mapper;
             _mediator = mediator;
        }


        public async  Task<ApiResponseDTO<List<GetMiscMasterDto>>> Handle(GetMiscMasterQuery request, CancellationToken cancellationToken)
        {
              //var (miscmaster, totalCount) = await _miscMasterQueryRepository.GetAllMiscMasterAsync(request.PageNumber, request.PageSize, request.SearchTerm);             
            var (miscmaster, totalCount) = await _miscMasterQueryRepository.GetAllMiscMasterAsync(request.PageNumber, request.PageSize, request.SearchTerm); 
            var MiscMasterlist = _mapper.Map<List<GetMiscMasterDto>>(miscmaster);
            
             //Domain Event
                 var domainEvent = new AuditLogsDomainEvent(
                     actionDetail: "GetAll",
                     actionCode: "",
                     actionName: "",
                     details: $"MiscTypeMaster details was fetched.",
                     module:"MiscTypeMaster"
                 );
                 await _mediator.Publish(domainEvent, cancellationToken);
            var response =MiscMasterlist.ToList();
                return new ApiResponseDTO<List<GetMiscMasterDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = response,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}