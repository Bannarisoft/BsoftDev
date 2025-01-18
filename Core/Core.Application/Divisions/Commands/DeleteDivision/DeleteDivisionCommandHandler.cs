using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using MediatR;
using AutoMapper;
using Core.Application.Common.Interfaces.IDivision;
using Core.Application.Common.HttpResponse;

namespace Core.Application.Divisions.Commands.DeleteDivision
{
    public class DeleteDivisionCommandHandler : IRequestHandler<DeleteDivisionCommand, ApiResponseDTO<bool>>
    {
        private readonly IDivisionCommandRepository _divisionRepository;
        private readonly IMapper _imapper;
        public DeleteDivisionCommandHandler(IDivisionCommandRepository divisionRepository, IMapper imapper)
        {
            _divisionRepository = divisionRepository;
            _imapper = imapper;
        }
         public async Task<ApiResponseDTO<bool>> Handle(DeleteDivisionCommand request, CancellationToken cancellationToken)
        {
            var division  = _imapper.Map<Division>(request);
            var divisionresult = await _divisionRepository.DeleteAsync(request.Id, division);
          
                 if(divisionresult)
                {
                    return new ApiResponseDTO<bool>{IsSuccess = true, Message = "Division updated successfully.", Data = true};
                }

                return new ApiResponseDTO<bool>{IsSuccess = false, Message = "Division not updated.", Data = false};
        }
    }
}