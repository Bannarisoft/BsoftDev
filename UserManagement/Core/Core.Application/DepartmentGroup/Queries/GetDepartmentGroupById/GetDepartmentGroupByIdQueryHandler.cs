using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IDepartment;
using Core.Application.Common.Interfaces.IDepartmentGroup;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.DepartmentGroup.Queries.GetDepartmentGroupById
{
  public class GetDepartmentGroupByIdQueryHandler : IRequestHandler<GetDepartmentGroupByIdQuery, ApiResponseDTO<DepartmentGroupByIdDto>>
  {
    private readonly IDepartmentGroupQueryRepository _departmentGroupRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public GetDepartmentGroupByIdQueryHandler(IDepartmentGroupQueryRepository departmentGroupRepository, IMapper mapper, IMediator mediator)
    {
      _departmentGroupRepository = departmentGroupRepository;
      _mapper = mapper;
      _mediator = mediator;

    }

        public async Task<ApiResponseDTO<DepartmentGroupByIdDto>> Handle(GetDepartmentGroupByIdQuery request, CancellationToken cancellationToken)
        {
               var departmentGroup = await _departmentGroupRepository.GetDepartmentGroupByIdAsync(request.Id);
                    
                    if (departmentGroup == null)
                    {
                       
                        return new ApiResponseDTO<DepartmentGroupByIdDto>
                        {
                            IsSuccess = false,
                            Message = "Department not found.",
                            Data = null
                        };
                    }
            

              var deptDto = _mapper.Map<DepartmentGroupByIdDto>(departmentGroup);
 //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: deptDto.Id.ToString(),        
                    actionName: deptDto.DepartmentGroupName,                
                    details: $"Department '{deptDto.DepartmentGroupName}' was created. DepartmentCode: {deptDto.Id}",
                    module:"Department"
                );

                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<DepartmentGroupByIdDto> { IsSuccess = true, Message = "Success", Data = deptDto };

               
        }
    }
}