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
using Core.Application.Common.Interfaces.IDivision;

namespace Core.Application.Divisions.Queries.GetDivisions
{
    public class GetDivisionQueryHandler : IRequestHandler<GetDivisionQuery,List<DivisionDTO>>
    {
        private readonly IDivisionQueryRepository _divisionRepository;        
        private readonly IMapper _mapper;
        public GetDivisionQueryHandler(IDivisionQueryRepository divisionRepository, IMapper mapper)
        {
            _divisionRepository = divisionRepository;
            _mapper =mapper;
        }
        public async Task<List<DivisionDTO>> Handle(GetDivisionQuery requst, CancellationToken cancellationToken)
        {
            var users = await _divisionRepository.GetAllDivisionAsync();
            var userList = _mapper.Map<List<DivisionDTO>>(users);
            return userList;
        }
    }
}