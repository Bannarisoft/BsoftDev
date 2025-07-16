using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IUser;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.EntityLevelAdmin.Commands.CreateEntityLevelAdmin
{
    public class CreateEntityLevelAdminCommandHandler : IRequestHandler<CreateEntityLevelAdminCommand, ApiResponseDTO<int>>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IUserCommandRepository _userRepository;
        private readonly IUserQueryRepository _userQueryRepository;
        public CreateEntityLevelAdminCommandHandler(IMediator mediator, IUserCommandRepository userRepository, IMapper mapper, IUserQueryRepository userQueryRepository)
        {
            _mediator = mediator;
            _userRepository = userRepository;
            _mapper = mapper;
            _userQueryRepository = userQueryRepository;
        }
        public async Task<ApiResponseDTO<int>> Handle(CreateEntityLevelAdminCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userQueryRepository.GetByUsernameAsync(request.Email);
            if (existingUser != null)
            {
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "User already exists."
                    
                };
            }
            

            var userEntity = _mapper.Map<User>(request);
            

            var createdUser = await _userRepository.CreateAsync(userEntity);

              if (createdUser == null)
            {
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Failed to create user. Please try again."
                };
            }
             var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: "Create Entity Level Admin",
                actionName: "Create Entity Level Admin",
                details: $"User '{createdUser.UserName}' was created.",
                module:"User"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

             return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "Entity Level Admin created successfully",
                Data = createdUser.UserId
            };
        }
    }
}