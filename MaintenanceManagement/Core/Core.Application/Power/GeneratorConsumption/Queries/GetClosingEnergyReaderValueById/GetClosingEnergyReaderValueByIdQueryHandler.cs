using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces.Power.IPowerConsumption;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Power.GeneratorConsumption.Queries.GetClosingEnergyReaderValueById
{
    public class GetClosingEnergyReaderValueByIdQueryHandler : IRequestHandler<GetClosingEnergyReaderValueByIdQuery, GetClosingEnergyReaderValueDto>
    {
        private readonly IPowerConsumptionQueryRepository _powerConsumptionQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public GetClosingEnergyReaderValueByIdQueryHandler(IPowerConsumptionQueryRepository powerConsumptionQueryRepository, IMapper mapper, IMediator mediator)
        {
            _powerConsumptionQueryRepository = powerConsumptionQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public Task<GetClosingEnergyReaderValueDto> Handle(GetClosingEnergyReaderValueByIdQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}