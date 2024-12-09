using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.Users.Commands.DeleteUser
{
    public class DeleteUserCommand : IRequest<int>
    {
        public Guid Id { get; set; }
    }
}
