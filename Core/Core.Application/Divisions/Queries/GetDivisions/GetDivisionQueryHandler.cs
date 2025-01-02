using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Text;
using System.Data;
using Dapper;

namespace Core.Application.Divisions.Queries.GetDivisions
{
    public class GetDivisionQueryHandler : IRequestHandler<GetDivisionQuery,List<DivisionDTO>>
    {
        // private readonly IDivisionRepository _divisionRepository;
        private readonly IDbConnection _dbConnection;
        private readonly IMapper _mapper;
        public GetDivisionQueryHandler(IDbConnection dbConnection, IMapper mapper)
        {
            _dbConnection = dbConnection;
            _mapper =mapper;
        }
        public async Task<List<DivisionDTO>> Handle(GetDivisionQuery requst, CancellationToken cancellationToken)
        {
            const string query = @"
            SELECT 
                Id, 
                ShortName,
                Name,
                CompanyId,
                IsActive
            FROM AppData.Division";
            var divisions = await _dbConnection.QueryAsync<DivisionDTO>(query);
            
            var divisionlist = _mapper.Map<List<DivisionDTO>>(divisions);

            return divisionlist.AsList();
        }
    }
}