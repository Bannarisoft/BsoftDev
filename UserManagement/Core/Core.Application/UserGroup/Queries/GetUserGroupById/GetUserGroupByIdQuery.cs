using Core.Application.Common.HttpResponse;
using Core.Application.UserGroup.Queries.GetUserGroup;
using MediatR;

namespace Core.Application.UserGroup.Queries.GetUserGroupById
{
    public class GetUserGroupByIdQuery : IRequest<ApiResponseDTO<UserGroupDto>>
    {
        public int Id { get; set; }
    }
}