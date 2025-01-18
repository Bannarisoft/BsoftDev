using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using Core.Application.Common.Interfaces.IDepartment;
using Core.Application.Departments.Queries.GetDepartments;
using Core.Domain.Events;
using Core.Application.Common;
using Core.Application.Common.HttpResponse;

namespace Core.Application.Departments.Queries.GetDepartmentAutoCompleteSearch
{

    public class GetDepartmentAutoCompleteSearchQueryHandler : IRequestHandler<GetDepartmentAutoCompleteSearchQuery, ApiResponseDTO<List<DepartmentDto>>>
    {
        private readonly IDepartmentQueryRepository _departmentRepository;
        private readonly IMapper _mapper;

          private readonly IMediator _mediator; 
        public GetDepartmentAutoCompleteSearchQueryHandler(IDepartmentQueryRepository divisionRepository,IMapper mapper, IMediator mediator)
        {
             _mapper =mapper;

            _departmentRepository = divisionRepository;    
            _mediator = mediator;        
        }


        public async Task<ApiResponseDTO<List<DepartmentDto>>> Handle(GetDepartmentAutoCompleteSearchQuery request, CancellationToken cancellationToken)
        {

          
                  var result = await _departmentRepository.GetAllDepartmentAutoCompleteSearchAsync(request.SearchPattern);
                  var department =_mapper.Map<ApiResponseDTO<List<DepartmentDto>>>(result);   

 var deptDto = _mapper.Map<List<DepartmentDto>>(result);
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAutoComplete",
                    actionCode:"",        
                    actionName: request.SearchPattern,                
                    details: $"Department '{request.SearchPattern}' was searched",
                    module:"Department"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
                return new ApiResponseDTO<List<DepartmentDto>> { IsSuccess = true, Message = "Success", Data = department.Data };  

        }
    }
         
}

