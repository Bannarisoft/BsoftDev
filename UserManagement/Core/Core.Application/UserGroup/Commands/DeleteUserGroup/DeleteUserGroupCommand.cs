using Core.Application.Common.HttpResponse;
using Core.Application.UserGroup.Queries.GetUserGroup;
using MediatR;

namespace Core.Application.UserGroup.Commands.DeleteUserGroup
{
    public class DeleteUserGroupCommand :  IRequest<ApiResponseDTO<UserGroupDto>>  
       {
                public int Id { get; set; }                
       }
}