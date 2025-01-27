using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.Users.Queries.GetUsers;
using MediatR;

namespace Core.Application.Users.Commands.ForgotUserPassword
{
    public class ForgotUserPasswordCommand   : IRequest<List<ApiResponseDTO<UserDto>>>
    {
         public string UserName { get; set; }
       //  public string VerificationCode { get; set; }
    }
}