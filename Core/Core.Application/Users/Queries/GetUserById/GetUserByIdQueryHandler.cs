using Core.Application.Users.Queries.GetUsers;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using AutoMapper;
using MediatR;
using Dapper;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Users.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery,UserDto>
    {
        // private readonly IUserRepository _userRepository;
        private readonly IDbConnection _dbConnection;
        private readonly IMapper _mapper;

        // public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
        // {
        //     _userRepository = userRepository;
        //     _mapper = mapper;
        // }
        public GetUserByIdQueryHandler(IDbConnection dbConnection, IMapper mapper)
        {
            _dbConnection = dbConnection;
            _mapper = mapper;
        }
        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
        //   var user = await _userRepository.GetByIdAsync(request.UserId);
        //   return _mapper.Map<UserDto>(user);
            var query = "SELECT * FROM AppSecurity.Users WHERE UserId = @UserId";
            var user = await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { UserId = request.UserId });
            return _mapper.Map<UserDto>(user);
        }
    }
}