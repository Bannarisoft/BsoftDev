using Core.Application.Common.HttpResponse;
using Core.Application.Users.Commands.CreateUser;
using Core.Application.Users.Queries.GetUsers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Users.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest<ApiResponseDTO<bool>>
    {
    public int UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public byte IsActive { get; set; }
    public string Mobile { get; set; }
    public string EmailId { get; set; }
    public int UserGroupId { get; set; }
    public int EntityId { get; set; }
    public List<UserDivisionDTO> userDivisions { get; set; }
    public List<UserCompanyDTO> UserCompanies  { get; set; }
    public List<UserRoleAllocationDTO> userRoleAllocations { get; set; }
    public List<UserUnitDTO> userUnits { get; set; }
    public List<UserDepartmentDTO> userDepartments { get; set; } 

    }
}