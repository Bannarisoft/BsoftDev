using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IDepartmentGroup;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.DepartmentGroup.Command.DeleteDepartmentGroup
{
    public class DeleteDepartmentGroupCommandHandler : IRequestHandler<DeleteDepartmentGroupCommand, ApiResponseDTO<bool>>
    {
        private readonly IDepartmentGroupCommandRepository _departmentGroupCommandRepository;
        private readonly IMapper _Imapper;
        private readonly IDepartmentGroupQueryRepository _departmentGroupQueryRepository;
        private readonly IMediator _mediator;


        public DeleteDepartmentGroupCommandHandler(IDepartmentGroupCommandRepository departmentGroupCommandRepository, IDepartmentGroupQueryRepository departmentGroupQueryRepository, IMediator mediator, IMapper mapper)
        {

            _departmentGroupCommandRepository = departmentGroupCommandRepository;
            _Imapper = mapper;
            _departmentGroupQueryRepository = departmentGroupQueryRepository;
            _mediator = mediator;

        }
      
        public async Task<ApiResponseDTO<bool>>Handle(DeleteDepartmentGroupCommand request, CancellationToken cancellationToken)
      {       

                
            // Map request to entity and delete
            var updatedDepartmentgroup = _Imapper.Map<Core.Domain.Entities.DepartmentGroup>(request);
            var result = await _departmentGroupCommandRepository.DeleteAsync(request.Id, updatedDepartmentgroup);

                if (!result)
                {
                   
                    return new ApiResponseDTO<bool>
                    {
                        IsSuccess = false,
                        Message = "Failed to delete department group.",
                        Data = false
                    };
                }
            // Publish domain event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Delete",
                actionCode: updatedDepartmentgroup.Id.ToString(),
                actionName: "",
                details: $"Department ID: {request.Id} was changed to status inactive.",
                module: "Department"
            );

            await _mediator.Publish(domainEvent, cancellationToken);
         
          return new ApiResponseDTO<bool>
            {
                IsSuccess = true,
                Message = "Department group deleted successfully.",
                Data = true
            };
      }
   
    }
}