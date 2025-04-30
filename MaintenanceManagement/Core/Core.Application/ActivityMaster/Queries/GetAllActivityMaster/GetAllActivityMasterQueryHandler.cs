using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.ActivityMaster.Queries.GetAllActivityMaster;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.External.IDepartment;
using Core.Application.Common.Interfaces.IActivityMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.ActivityMaster.Queries.GetAllActivityMaster
{
    public class GetAllActivityMasterQueryHandler  : IRequestHandler<GetAllActivityMasterQuery, ApiResponseDTO<List<GetAllActivityMasterDto>>>
    {
        private readonly IActivityMasterQueryRepository _activityMasterQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
          private readonly IDepartmentService _departmentService;


        public GetAllActivityMasterQueryHandler(IActivityMasterQueryRepository activityMasterQueryRepository, IMapper mapper, IMediator mediator , IDepartmentService departmentService)
        {
            _activityMasterQueryRepository = activityMasterQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
            _departmentService = departmentService;
        }
         public async Task<ApiResponseDTO<List<GetAllActivityMasterDto>>> Handle(GetAllActivityMasterQuery request, CancellationToken cancellationToken)
        {
            // Fetch data from repository
            var (activities, totalCount) = await _activityMasterQueryRepository.GetAllActivityMasterAsync(request.PageNumber, request.PageSize, request.SearchTerm);

            // Map domain entities to DTOs
            var activityList = _mapper.Map<List<GetAllActivityMasterDto>>(activities);


             var departments = await _departmentService.GetAllDepartmentAsync();
            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);

            var activityMasterDictionary = new Dictionary<int, GetAllActivityMasterDto>();

                 foreach (var data in activityList)
            {
              
                    if (departmentLookup.TryGetValue(data.DepartmentId, out var departmentName )&& departmentName != null)
                    {
                        data.Department = departmentName;
                    }

                    activityMasterDictionary[data.DepartmentId] = data;
                
            }

            // Publish domain event for auditing
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "",
                actionName: "",
                details: "Activity Master details were fetched.",
                module: "ActivityMaster"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

            // Return API response
            return new ApiResponseDTO<List<GetAllActivityMasterDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = activityList,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        
    }
}