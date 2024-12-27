using Dapper;
using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Users.Queries.GetUsers
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery,List<UserDto>>
    {
        // private readonly IUserRepository _userRepository;
        private readonly IDbConnection _dbConnection;
        private readonly IMapper _mapper;

        // public GetUserQueryHandler(IUserRepository userRepository , IMapper mapper)
        // {
        //     _userRepository = userRepository;
        //     _mapper = mapper;
        // }
        public GetUserQueryHandler(IDbConnection dbConnection, IMapper mapper)
        {
            _dbConnection = dbConnection;
            _mapper = mapper;
        }
        public async Task<List<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            // var users = await _userRepository.GetAllUsersAsync();
            // var userList = _mapper.Map<List<UserDto>>(users);
            // return userList;
            const string sql = "SELECT * FROM AppSecurity.Users";

            var users = await _dbConnection.QueryAsync<User>(sql);
            var userDtos = _mapper.Map<List<UserDto>>(users);

            return userDtos;
        }
    }
}


