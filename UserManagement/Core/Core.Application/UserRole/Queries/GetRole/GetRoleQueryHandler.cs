
using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;
using System.Data;
using Core.Application.Common.Interfaces.IUserRole;
using Core.Application.Common.HttpResponse;
using Core.Domain.Events;
using Microsoft.Extensions.Logging;


namespace Core.Application.UserRole.Queries.GetRole
{
    public class GetRoleQueryHandler :IRequestHandler<GetRoleQuery,ApiResponseDTO<List<GetUserRoleDto>>>
    {
       
     private readonly IUserRoleQueryRepository _userRoleRepository;
     private readonly IMapper _mapper;
     
       private readonly IMediator _mediator; 
         private readonly ILogger<GetRoleQueryHandler> _logger;
   


       public GetRoleQueryHandler(IUserRoleQueryRepository userRoleRepository, IMapper mapper, IMediator mediator, ILogger<GetRoleQueryHandler> logger)
        {
            _userRoleRepository = userRoleRepository;
            _mapper =mapper;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<ApiResponseDTO<List<GetUserRoleDto>>> Handle(GetRoleQuery request ,CancellationToken cancellationToken )
        {

            _logger.LogInformation("Starting GetRoleQueryHandler.");

        //     _logger.LogInformation("Fetching user roles from repository.");
                var roles = await _userRoleRepository.GetAllRoleAsync();

                if (roles is null)
                {
                    _logger.LogWarning("No user roles found.");
                    return new ApiResponseDTO<List<GetUserRoleDto>>
                    {
                        IsSuccess = false,
                        Message = "No roles found",
                        Data = null
                    };
                }

                _logger.LogInformation("Mapping user roles to DTO.");
                var roleList = _mapper.Map<List<GetUserRoleDto>>(roles);

                // Publish domain event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "",
                    actionName: "",
                    details: "UserRole details were fetched.",
                    module: "UserRole"
                );

                _logger.LogInformation("Publishing AuditLogsDomainEvent.");
                await _mediator.Publish(domainEvent, cancellationToken);

                _logger.LogInformation("Returning success response with fetched user roles.");
                return new ApiResponseDTO<List<GetUserRoleDto>>
                {
                    IsSuccess = true,
                    Message = "Success",
                    Data = roleList
                };
       
       
        }



  }
}