using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IUserGroup;
using Core.Application.UserGroup.Queries.GetUserGroup;
using Core.Domain.Enums.Common;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.UserGroup.Commands.DeleteUserGroup
{
    public class DeleteUserGroupCommandHandler : IRequestHandler<DeleteUserGroupCommand, ApiResponseDTO<UserGroupDto>>
    {
        private readonly IUserGroupCommandRepository _userGroupRepository;
        private readonly IUserGroupQueryRepository _userGroupQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        
        public DeleteUserGroupCommandHandler(IUserGroupCommandRepository userGroupRepository, IMapper mapper, IUserGroupQueryRepository userGroupQueryRepository, IMediator mediator)
        {
            _userGroupRepository = userGroupRepository;
            _mapper = mapper;
            _userGroupQueryRepository = userGroupQueryRepository;
            _mediator = mediator;
        }       
        public async Task<ApiResponseDTO<UserGroupDto>> Handle(DeleteUserGroupCommand request, CancellationToken cancellationToken)
        {
            var userGroup = await _userGroupQueryRepository.GetByIdAsync(request.Id);
            if (userGroup is null || userGroup.IsDeleted is Enums.IsDelete.Deleted)
            {
                return new ApiResponseDTO<UserGroupDto>
                {
                    IsSuccess = false,
                    Message = "Invalid GroupID. The specified GroupName does not exist or is inactive."
                };
            }         
          
            var userGroupDelete = _mapper.Map<Core.Domain.Entities.UserGroup>(request);
            var updateResult = await _userGroupRepository.DeleteAsync(request.Id, userGroupDelete);
            if (updateResult > 0)
            {
                var userGroupDto = _mapper.Map<UserGroupDto>(userGroupDelete); 
                //Domain Event  
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Delete",
                    actionCode: userGroupDto.GroupCode ?? string.Empty,
                    actionName: userGroupDto.GroupName ?? string.Empty,
                    details: $"UserGroup '{userGroupDto.GroupName}' was created. GroupCode: {userGroupDto.GroupCode}",
                    module:"UserGroup"
                );               
                await _mediator.Publish(domainEvent, cancellationToken);              
                return new ApiResponseDTO<UserGroupDto>
                {
                    IsSuccess = true,
                    Message = "UserGroup deleted successfully.",
                    Data = userGroupDto
                };
            }
            return new ApiResponseDTO<UserGroupDto>
            {
                IsSuccess = false,
                Message = "UserGroup deletion failed."
            };          
        }
    }
}