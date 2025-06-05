using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.Power.IFeederGroup;
using MediatR;

namespace Core.Application.Power.FeederGroup.Command.CreateFeederGroup
{
    public class CreateFeederGroupCommandHandler : IRequestHandler<CreateFeederGroupCommand, ApiResponseDTO<int>>
    {
        private readonly IFeederGroupCommandRepository _feederGroupCommandRepository;
        private readonly IMapper _mapper;
        public CreateFeederGroupCommandHandler(IFeederGroupCommandRepository repository, IMapper mapper)
        {
            _feederGroupCommandRepository = repository;
            _mapper = mapper;
        }

         public async Task<ApiResponseDTO<int>> Handle(CreateFeederGroupCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Core.Domain.Entities.Power.FeederGroup>(request);

       

        int newId = await _feederGroupCommandRepository.CreateAsync(entity);

        return new ApiResponseDTO<int>
        {
            IsSuccess = true,
            Message = "FeederGroup created successfully.",
            Data = newId
        };
    }   

    }
}