using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IRoleEntitlement;
using Core.Application.RoleEntitlements.Queries.GetRoleEntitlements;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.RoleEntitlements.Queries.GetRoleEntitlementById
{
    public class GetEntitlementByIdQueryHandler : IRequestHandler<GetRoleEntitlementByIdQuery, ApiResponseDTO<RoleEntitlementDto>>
    {
        private readonly IRoleEntitlementQueryRepository _roleEntitlementRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public GetEntitlementByIdQueryHandler(IRoleEntitlementCommandRepository roleEntitlementCommandRepository, IRoleEntitlementQueryRepository roleEntitlementQueryRepository, IMapper mapper, IMediator mediator)
        {
            _roleEntitlementRepository = roleEntitlementQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<RoleEntitlementDto>> Handle(GetRoleEntitlementByIdQuery request, CancellationToken cancellationToken)
        {
              var roleEntitlement = await _roleEntitlementRepository.GetByIdAsync(request.Id);
            if (roleEntitlement == null)
            {
                return new ApiResponseDTO<RoleEntitlementDto>
                {
                    IsSuccess = false,
                    Message = "RoleEntitlement not found"
                };
            }
            var roleEntitlementDto = _mapper.Map<RoleEntitlementDto>(roleEntitlement);
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode: roleEntitlementDto.RoleName,        
                actionName: roleEntitlementDto.ModuleName,                
                details: $"RoleEntitlement '{roleEntitlementDto.RoleName}' was created. ModuleName: {roleEntitlementDto.ModuleName}",
                module:"RoleEntitlement"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<RoleEntitlementDto>
            {
                IsSuccess = true,
                Message = "RoleEntitlement fetched successfully",
                Data = roleEntitlementDto
            };           

        }
    }
}