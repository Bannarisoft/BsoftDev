
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IUserGroup;
using Core.Application.UserGroup.Queries.GetUserGroup;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.UserGroup.Commands.CreateUserGroup
{
    public class CreateUserGroupCommandHandler  : IRequestHandler<CreateUserGroupCommand, ApiResponseDTO<UserGroupDto>>
{
    private readonly IMapper _mapper;
    private readonly IUserGroupCommandRepository _userGroupRepository;    
    private readonly IMediator _mediator; 

    // Constructor Injection
    public CreateUserGroupCommandHandler(IMapper mapper, IUserGroupCommandRepository userGroupRepository, IMediator mediator)
    {
        _mapper = mapper;
        _userGroupRepository = userGroupRepository; 
        _mediator = mediator;               
    }

    public async Task<ApiResponseDTO<UserGroupDto>> Handle(CreateUserGroupCommand request, CancellationToken cancellationToken)
    {
        var userGroupExists = await _userGroupRepository.GetUserGroupByCodeAsync(request.GroupName ?? string.Empty,request.GroupCode ?? string.Empty);        
        if (userGroupExists.Id !=0)
        {
            return new ApiResponseDTO<UserGroupDto>
            {
                IsSuccess = false,
                Message = "GroupCode already exists"
            };
        }
        var userGroupEntity = _mapper.Map<Core.Domain.Entities.UserGroup>(request);    
         
        var result = await _userGroupRepository.CreateAsync(userGroupEntity);
        if (result != null)
        {
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: result?.GroupCode ?? string.Empty,
                actionName: result?.GroupName ?? string.Empty,
                details: $"USerGroup '{result?.GroupName}' was created. GroupCode: {result?.GroupCode}",
                module:"USerGroup"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            
            var userGroupDto = _mapper.Map<UserGroupDto>(result);
            if (userGroupDto.Id > 0)
            {
                return new ApiResponseDTO<UserGroupDto>
                {
                    IsSuccess = true,
                    Message = "UserGroup created successfully",
                    Data = userGroupDto
                };
            }
        }
        return new ApiResponseDTO<UserGroupDto>
        {
            IsSuccess = false,
            Message = "UserGroup not created"
        };
    }
    }
}
