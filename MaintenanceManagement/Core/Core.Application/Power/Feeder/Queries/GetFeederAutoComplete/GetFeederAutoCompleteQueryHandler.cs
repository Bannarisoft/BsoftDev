using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.Power.IFeeder;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Power.Feeder.Queries.GetFeederAutoComplete
{
    public class GetFeederAutoCompleteQueryHandler : IRequestHandler<GetFeederAutoCompleteQuery, ApiResponseDTO<List<GetFeederAutoCompleteDto>>>
    {
        private readonly IFeederQueryRepository _feederQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetFeederAutoCompleteQueryHandler(IFeederQueryRepository feederQueryRepository, IMapper mapper, IMediator mediator)
        {
            _feederQueryRepository = feederQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }
        
           public  async Task<ApiResponseDTO<List<GetFeederAutoCompleteDto>>> Handle(GetFeederAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            var machineGroup  = await _feederQueryRepository.GetFeederAutoComplete(request.SearchPattern);

                    if (machineGroup == null || !machineGroup.Any())
            {
                return new ApiResponseDTO<List<GetFeederAutoCompleteDto>>
                {
                    IsSuccess = false,
                    Message = $"No FeederGroup Masters found matching '{request.SearchPattern}'.",
                    Data = new List<GetFeederAutoCompleteDto>()
                };
            }

            var division = _mapper.Map<List<GetFeederAutoCompleteDto>>(machineGroup);
             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "",        
                    actionName: "", 
                    details: $"FeederGroup details was fetched.",
                    module:"FeederGroup"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<GetFeederAutoCompleteDto>> { IsSuccess = true, Message = "Success", Data = division }; 
        }



        
    }
}