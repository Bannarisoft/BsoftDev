using BSOFT.Application.Units.Queries.GetUnits;
using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace BSOFT.Application.Units.Commands.CreateUnit
{
    public class CreateUnitCommandHandler :  IRequestHandler<CreateUnitCommand, UnitVm>

    {
        private readonly IUnitRepository _unitRepository;
        private readonly IMapper _mapper;

         public CreateUnitCommandHandler(IUnitRepository unitRepository, IMapper mapper)
        {
            _unitRepository = unitRepository;
            _mapper = mapper;
        }

        public async Task<UnitVm> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
        {
            var unitEntity = new BSOFT.Domain.Entities.Unit
            {
                UnitName = request.UnitName,
                ShortName = request.ShortName,
            
                IsActive = request.IsActive  

            };

            var result = await _unitRepository.CreateAsync(unitEntity);
            return _mapper.Map<UnitVm>(result);
        }

    }
}