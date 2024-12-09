using BSOFT.Application.Users.Queries.GetUsers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.Users.Queries.GetUserById  
{
    public class GetUserByIdQuery : IRequest<UserVm>
    {
        public Guid UserId { get; set; }
    }
}