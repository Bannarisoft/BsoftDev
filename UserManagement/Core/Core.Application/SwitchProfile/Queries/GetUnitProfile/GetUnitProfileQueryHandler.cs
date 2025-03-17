using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IProfile;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.SwitchProfile.Queries.GetUnitProfile
{
    public class GetUnitProfileQueryHandler : IRequestHandler<GetUnitProfileQuery, ApiResponseDTO<List<GetUnitProfileDTO>>>
    {
        private readonly IProfileQuery _iProfileQuery;
        private readonly IMapper _mapper;
        private readonly IIPAddressService _ipAddressService;
        private readonly IMediator _mediator; 
        public GetUnitProfileQueryHandler(IProfileQuery iProfileQuery, IMapper mapper, IIPAddressService ipAddressService,IMediator mediator)
        {
            _iProfileQuery = iProfileQuery;
            _mapper = mapper;
            _ipAddressService = ipAddressService;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<List<GetUnitProfileDTO>>> Handle(GetUnitProfileQuery request, CancellationToken cancellationToken)
        {
            var userId = _ipAddressService.GetUserId();
            var result = await _iProfileQuery.GetUnit(userId);

              if (result is null || !result.Any() || result.Count == 0) 
                {
                      
                     return new ApiResponseDTO<List<GetUnitProfileDTO>>
                     {
                         IsSuccess = false,
                         Message = "Unit not found."
                     };
                }
                var unitDto = _mapper.Map<List<GetUnitProfileDTO>>(result);

                      var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetUnitProfileQuery",
                actionCode:"",        
                actionName: "GetUnitProfileQuery",                
                details: $"Unit was searched",
                module:"Unit"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

            
            return new ApiResponseDTO<List<GetUnitProfileDTO>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = unitDto
            }; 
        }
    }
}