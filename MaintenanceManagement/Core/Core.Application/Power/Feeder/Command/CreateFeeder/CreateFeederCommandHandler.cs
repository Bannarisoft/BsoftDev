using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.Power.IFeeder;
using MediatR;

namespace Core.Application.Power.Feeder.Command.CreateFeeder
{
    public class CreateFeederCommandHandler   : IRequestHandler<CreateFeederCommand, int>
    {

        private readonly IFeederCommandRepository _feederCommandRepository;
        private readonly IMapper _mapper;

        public CreateFeederCommandHandler(IFeederCommandRepository feederCommandRepository, IMapper mapper)
        {
            _feederCommandRepository = feederCommandRepository;
            _mapper = mapper;
        }
           public async Task<int> Handle(CreateFeederCommand request, CancellationToken cancellationToken)
       {
            var entity = _mapper.Map<Core.Domain.Entities.Power.Feeder>(request);        

            int newId = await _feederCommandRepository.CreateAsync(entity);

            return newId > 0 ? newId : throw new ExceptionRules("Feeder Creation Failed.");
       }   


    }
}