using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.UserSession
{
    public class GetUserSessionQuery : IRequest<UserSessions>
    {
        public int UserId { get; set; }
    }
}