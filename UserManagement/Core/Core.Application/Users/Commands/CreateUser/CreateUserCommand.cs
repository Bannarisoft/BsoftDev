using Core.Application.Common.HttpResponse;
using Core.Application.Users.Queries.GetUsers;
using Core.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Users.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<ApiResponseDTO<UserDto>>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Mobile { get; set; }
        public string? EmailId { get; set; }
        public int UserGroupId { get; set; }
        public int EntityId { get; set; }
        public List<UserDivisionDTO> userDivisions { get; set; } = new();
        public List<UserCompanyDTO> UserCompanies { get; set; } = new();
        public List<UserRoleAllocationDTO> userRoleAllocations { get; set; } = new();
        public List<UserUnitDTO> userUnits { get; set; } = new();
        public List<UserDepartmentDTO> userDepartments { get; set; } = new();
    }
}