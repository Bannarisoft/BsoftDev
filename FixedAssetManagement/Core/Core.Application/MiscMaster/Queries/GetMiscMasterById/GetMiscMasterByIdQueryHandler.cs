using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MiscMaster.Queries.GetMiscMasterById
{
    public class GetMiscMasterByIdQueryHandler : IRequestHandler<GetMiscMasterByIdQuery, ApiResponseDTO<GetMiscMasterDto>>
    {

        private readonly IMiscMasterQueryRepository  _miscMasterQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

         public GetMiscMasterByIdQueryHandler(IMiscMasterQueryRepository miscMasterQueryRepository, IMapper mapper, IMediator mediator)
        {
            _miscMasterQueryRepository = miscMasterQueryRepository;
            _mapper =mapper;
            _mediator = mediator;
        } 

          public async  Task<ApiResponseDTO<GetMiscMasterDto>> Handle(GetMiscMasterByIdQuery request, CancellationToken cancellationToken)
        {
                  
            var result = await _miscMasterQueryRepository.GetByIdAsync(request.Id);
            if (result is null )
            {
                  return new ApiResponseDTO<GetMiscMasterDto>
                    {
                        IsSuccess = false,
                        Message = $"MiscTypeMaster with Id {request.Id} not found.",
                        Data = null
                    };
            }
           
            var misctypemaster = _mapper.Map<GetMiscMasterDto>(result);

            //Domain Event
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "GetById",
                        actionCode: "",        
                        actionName: "",
                        details: $"MiscTypeMaster details {misctypemaster.Id} was fetched.",
                        module:"MiscTypeMaster"
                    );
                    await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<GetMiscMasterDto> 
            {
                 IsSuccess = true, 
                Message = "Success", 
                Data = misctypemaster
             };
        }

        
    }
}