using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using System.Reflection;
using System.Text;

namespace Core.Application.Companies.Commands.DeleteCompany
{
    public class DeleteCompanyCommand : IRequest<int>
    {
        public int Id { get; set; }
        public byte IsActive { get; set; }
    }
}