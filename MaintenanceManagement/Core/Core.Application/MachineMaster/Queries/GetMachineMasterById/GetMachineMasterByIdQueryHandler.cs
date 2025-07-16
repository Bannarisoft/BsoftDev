using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineMaster;
using Core.Application.MachineMaster.Queries.GetMachineMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineMaster.Queries.GetMachineMasterById
{
    public class GetMachineMasterByIdQueryHandler : IRequestHandler<GetMachineMasterByIdQuery, MachineMasterDto>
    {
        
        private readonly IMachineMasterQueryRepository _imachineMasterQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;        


        public GetMachineMasterByIdQueryHandler(IMachineMasterQueryRepository imachineMasterQueryRepository, IMapper mapper, IMediator mediator)
        {
            _imachineMasterQueryRepository = imachineMasterQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;            
        }

        public async Task<MachineMasterDto> Handle(GetMachineMasterByIdQuery request, CancellationToken cancellationToken)
        {
             var result = await _imachineMasterQueryRepository.GetByIdAsync(request.Id);
          
            // Map a single entity
            var machineMaster = _mapper.Map<MachineMasterDto>(result);
       
          //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "GetMachineMasterByIdQuery",        
                    actionName: machineMaster.Id.ToString(),
                    details: $"MachineMaster details {machineMaster.Id} was fetched.",
                    module:"MachineMaster"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
          return machineMaster;
        }
    }
}