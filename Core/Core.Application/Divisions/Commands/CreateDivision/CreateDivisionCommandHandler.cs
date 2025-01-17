using Core.Application.Divisions.Queries.GetDivisions;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IDivision;
using Core.Application.Common.HttpResponse;

namespace Core.Application.Divisions.Commands.CreateDivision
{
    public class CreateDivisionCommandHandler : IRequestHandler<CreateDivisionCommand, ApiResponseDTO<int>>
    {
         private readonly IDivisionCommandRepository _divisionRepository;
        private readonly IMapper _imapper;

        public CreateDivisionCommandHandler(IDivisionCommandRepository divisionRepository, IMapper imapper)
        {
            _divisionRepository = divisionRepository;
            _imapper = imapper;
        }

        public async Task<ApiResponseDTO<int>> Handle(CreateDivisionCommand request, CancellationToken cancellationToken)
        {
           
                 var division  = _imapper.Map<Division>(request);

                var divisionresult = await _divisionRepository.CreateAsync(division);
                if (divisionresult > 0)
                {
                    return new ApiResponseDTO<int>{IsSuccess = true, Message = "Division created successfully", Data = divisionresult};
                }
               
                    return new ApiResponseDTO<int>{IsSuccess = false, Message = "Division not created"};
           
        }
    }
}