using BSOFT.Application.Units.Queries.GetUnits;
using BSOFT.Domain.Entities;
using BSOFT.Domain.Interfaces;
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
                Address1 = request.Address1,
                Address2 = request.Address2,
                Address3 = request.Address3,
                CoId = request.CoId,
                DivId = request.DivId,
                UnitHeadName = request.UnitHeadName,
                Mobile = request.Mobile,
                Email = request.Email,
                IsActive = request.IsActive  

            };

            var result = await _unitRepository.CreateAsync(unitEntity);
            return _mapper.Map<UnitVm>(result);
        }

    }
}