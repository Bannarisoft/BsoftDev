using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IUserGroup;
using Core.Application.UserGroup.Queries.GetUserGroup;
using Core.Domain.Enums.Common;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.UserGroup.Commands.UpdateUesrGroup
{
    public class UpdateUserGroupCommandHandler  : IRequestHandler<UpdateUserGroupCommand,ApiResponseDTO<UserGroupDto>>    
    {
        private readonly IUserGroupCommandRepository _userGroupRepository;
        private readonly IMapper _mapper;
        private readonly IUserGroupQueryRepository _userGroupQueryRepository;
        private readonly IMediator _mediator; 

        public UpdateUserGroupCommandHandler(IUserGroupCommandRepository userGroupRepository, IMapper mapper, IUserGroupQueryRepository userGroupQueryRepository, IMediator mediator)
        {
            _userGroupRepository = userGroupRepository;
             _mapper = mapper;
            _userGroupQueryRepository = userGroupQueryRepository;
            _mediator = mediator;
        }       
        public async Task<ApiResponseDTO<UserGroupDto>> Handle(UpdateUserGroupCommand request, CancellationToken cancellationToken)
        {
            var userGroup = await _userGroupQueryRepository.GetByIdAsync(request.Id);
            if (userGroup is null)
                return new ApiResponseDTO<UserGroupDto>
                {
                    IsSuccess = false,
                    Message = "UserGroup not found"
                };
            var oldGroupName = userGroup.GroupName;
            userGroup.GroupName = request.GroupName;
            if (userGroup is null || userGroup.IsDeleted is Enums.IsDelete.Deleted)
            {
                return new ApiResponseDTO<UserGroupDto>
                {
                    IsSuccess = false,
                    Message = "Invalid UserGroupID. The specified UserGroup does not exist or is inactive."
                };
            }   
                      
            if ((byte)userGroup.IsActive != request.IsActive)
            {    
                 userGroup.IsActive =  (Enums.Status)request.IsActive;             
                await _userGroupRepository.UpdateAsync(userGroup.Id, userGroup);
                if (request.IsActive is 0)
                {
                    return new ApiResponseDTO<UserGroupDto>
                    {
                        IsSuccess = true,
                        Message = "UserGroupCode DeActivated."
                    };
                }
                else{
                    return new ApiResponseDTO<UserGroupDto>
                    {
                        IsSuccess = true,
                        Message = "UserGroupCode Activated."
                    }; 
                }                                     
            }
        
            var updatedUserGroupEntity = _mapper.Map<Core.Domain.Entities.UserGroup>(request);
            
            var updateResult = await _userGroupRepository.UpdateAsync(request.Id, updatedUserGroupEntity);            
            var updatedUserGroups = await _userGroupQueryRepository.GetByIdAsync(request.Id);
            
            if (updatedUserGroups != null)
            {
                var userGroupDto = _mapper.Map<UserGroupDto>(updatedUserGroups);
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: userGroupDto.GroupCode ?? string.Empty,
                    actionName: userGroupDto.GroupName ?? string.Empty,                            
                    details: $"UserGroup '{oldGroupName}' was updated to '{userGroupDto.GroupName}'.  UserGroupCode: {userGroupDto.GroupCode}",
                    module:"UserGroup"
                );            
                await _mediator.Publish(domainEvent, cancellationToken);
                if(updateResult>0)
                {
                    return new ApiResponseDTO<UserGroupDto>
                    {
                        IsSuccess = true,
                        Message = "UserGroup updated successfully",
                        Data = userGroupDto
                    };
                }
                return new ApiResponseDTO<UserGroupDto>
                {
                    IsSuccess = false,
                    Message = "UserGroup not updated."
                }; 
            }
            else
            {
                return new ApiResponseDTO<UserGroupDto>
                {
                    IsSuccess = false,
                    Message = "UserGroup update failed"
                };
            }                   
        }
    }
}