using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUser;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServices.UserManagement;

namespace UserManagement.API.GrpcServices
{
    public class UserAllGrpcService : GetAllUsersJobService.GetAllUsersJobServiceBase
    {
        private readonly IUserQueryRepository _userQueryRepository;
        public UserAllGrpcService(IUserQueryRepository userQueryRepository)
        {
            _userQueryRepository = userQueryRepository;
        }
        public override async Task<UsersAllListResponse> GetUserAll(Empty request, ServerCallContext context)
    {
        var users = await _userQueryRepository.GetUser("");

        var response = new UsersAllListResponse();
        response.Users.AddRange(users.Select(d => new UsersAllDto
        {
            UserId = d.UserId,
            UserName = d.UserName

        }));

        return response;
    }
    }
}