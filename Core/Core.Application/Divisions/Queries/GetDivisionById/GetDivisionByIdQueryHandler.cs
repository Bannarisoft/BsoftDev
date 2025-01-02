using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Text;
using Core.Application.Divisions.Queries.GetDivisions;
using System.Data;
using Dapper;

namespace Core.Application.Divisions.Queries.GetDivisionById
{
    public class GetDivisionByIdQueryHandler : IRequestHandler<GetDivisionByIdQuery,DivisionDTO>
    {
        // private readonly IDivisionRepository _divisionRepository;
        private readonly IDbConnection _dbConnection;
        private readonly IMapper _mapper;

         public GetDivisionByIdQueryHandler(IDbConnection dbConnection, IMapper mapper)
        {
            _dbConnection = dbConnection;
            _mapper =mapper;
        } 
        public async Task<DivisionDTO> Handle(GetDivisionByIdQuery request, CancellationToken cancellationToken)
        {
            var query = "SELECT * FROM AppData.Division WHERE Id = @Id";
        var divisionresult = await _dbConnection.QuerySingleOrDefaultAsync<DivisionDTO>(query, new { Id = request.Id });

         if (divisionresult == null)
        {
            return null;
        }
          return _mapper.Map<DivisionDTO>(divisionresult);
        }
    }
}