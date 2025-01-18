using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Text;
using Core.Application.Divisions.Queries.GetDivisions;
using Core.Application.Common.Interfaces.IDivision;
using Core.Application.Common.HttpResponse;

namespace Core.Application.Divisions.Queries.GetDivisionById
{
    public class GetDivisionByIdQueryHandler : IRequestHandler<GetDivisionByIdQuery,ApiResponseDTO<DivisionDTO>>
    {
         private readonly IDivisionQueryRepository _divisionRepository;        
        private readonly IMapper _mapper;

         public GetDivisionByIdQueryHandler(IDivisionQueryRepository divisionRepository, IMapper mapper)
        {
            _divisionRepository = divisionRepository;
            _mapper =mapper;
        } 
        public async Task<ApiResponseDTO<DivisionDTO>> Handle(GetDivisionByIdQuery request, CancellationToken cancellationToken)
        {
            
        var result = await _divisionRepository.GetByIdAsync(request.Id);
        var division = _mapper.Map<DivisionDTO>(result);
          return new ApiResponseDTO<DivisionDTO> { IsSuccess = true, Message = "Success", Data = division };

        }
    }
}