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

namespace Core.Application.Divisions.Queries.GetDivisionById
{
    public class GetDivisionByIdQueryHandler : IRequestHandler<GetDivisionByIdQuery,DivisionDTO>
    {
         private readonly IDivisionQueryRepository _divisionRepository;        
        private readonly IMapper _mapper;

         public GetDivisionByIdQueryHandler(IDivisionQueryRepository divisionRepository, IMapper mapper)
        {
            _divisionRepository = divisionRepository;
            _mapper =mapper;
        } 
        public async Task<DivisionDTO> Handle(GetDivisionByIdQuery request, CancellationToken cancellationToken)
        {
            /* var query = "SELECT * FROM AppData.Division WHERE Id = @Id";
        var divisionresult = await _dbConnection.QuerySingleOrDefaultAsync<DivisionDTO>(query, new { Id = request.Id });

         if (divisionresult == null)
        {
            return null;
        }
          return _mapper.Map<DivisionDTO>(divisionresult); */
        var result = await _divisionRepository.GetByIdAsync(request.Id);
          return _mapper.Map<DivisionDTO>(result);

        }
    }
}