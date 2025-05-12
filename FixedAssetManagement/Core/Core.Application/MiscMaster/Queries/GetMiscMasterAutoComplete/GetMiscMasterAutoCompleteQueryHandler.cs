using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using Core.Application.MiscTypeMaster.Queries.GetMiscTypeMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MiscMaster.Queries.GetMiscMasterAutoComplete
{
    public class GetMiscMasterAutoCompleteQueryHandler : IRequestHandler<GetMiscMasterAutoCompleteQuery,ApiResponseDTO<List<GetMiscMasterAutoCompleteDto>>>
    {
         private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
     public GetMiscMasterAutoCompleteQueryHandler(IMiscMasterQueryRepository miscMasterQueryRepository, IMapper mapper, IMediator mediator)
         {
            _miscMasterQueryRepository =miscMasterQueryRepository;
            _mapper =mapper;
            _mediator = mediator;
         }


          public  async Task<ApiResponseDTO<List<GetMiscMasterAutoCompleteDto>>> Handle(GetMiscMasterAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            var miscTypeMasters  = await _miscMasterQueryRepository.GetMiscMaster( request.MiscTypeCode,request.MiscTypeName);

                    if (miscTypeMasters == null )
            {
                return new ApiResponseDTO<List<GetMiscMasterAutoCompleteDto>>
                {
                    IsSuccess = false,
                    Message = $"No Misc Type Masters found for TypeCode '{request.MiscTypeCode}' and TypeName '{request.MiscTypeName}'.",
                    Data = new List<GetMiscMasterAutoCompleteDto>()
                };
            }

            var division = _mapper.Map<List<GetMiscMasterAutoCompleteDto>>(miscTypeMasters);
             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "",        
                    actionName: "", 
                    details: $"Misc Type details was fetched.",
                    module:"MiscType"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<GetMiscMasterAutoCompleteDto>> { IsSuccess = true, Message = "Success", Data = division }; 
        }
        
    }
}