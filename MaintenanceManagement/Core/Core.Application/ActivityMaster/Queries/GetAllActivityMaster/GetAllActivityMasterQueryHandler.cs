using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IActivityMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.ActivityMaster.Queries.GetAllActivityMaster
{
    public class GetAllActivityMasterQueryHandler : IRequestHandler<GetAllActivityMasterQuery, ApiResponseDTO<List<GetAllActivityMasterDto>>>
    {
        private readonly IActivityMasterQueryRepository _activityMasterQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IDepartmentGrpcClient _departmentGrpcClient;
         private readonly IUnitGrpcClient _unitGrpcClient;
          private readonly IIPAddressService _ipAddressService;
        public GetAllActivityMasterQueryHandler(IActivityMasterQueryRepository activityMasterQueryRepository, IMapper mapper, IMediator mediator, IDepartmentGrpcClient departmentGrpcClient, IUnitGrpcClient unitGrpcClient, IIPAddressService ipAddressService)
        {
            _activityMasterQueryRepository = activityMasterQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
            _departmentGrpcClient = departmentGrpcClient;
            _unitGrpcClient = unitGrpcClient;
            _ipAddressService = ipAddressService;
        }
        public async Task<ApiResponseDTO<List<GetAllActivityMasterDto>>> Handle(GetAllActivityMasterQuery request, CancellationToken cancellationToken)
        {

            var unitId = _ipAddressService.GetUnitId();
            // Fetch data from repository
            var (activities, totalCount) = await _activityMasterQueryRepository.GetAllActivityMasterAsync(request.PageNumber, request.PageSize, request.SearchTerm);

            // Map domain entities to DTOs
            var activityList = _mapper.Map<List<GetAllActivityMasterDto>>(activities);

            // 🔥 Fetch departments using gRPC
            var departments = await _departmentGrpcClient.GetAllDepartmentAsync();
            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);

             var units = await _unitGrpcClient.GetAllUnitAsync();
              var unitLookup = units.ToDictionary(u => u.UnitId, u => u.UnitName);

          //  var activityMasterDictionary = new Dictionary<int, GetAllActivityMasterDto>();


            //    🔥 Map department names with DataControl
            foreach (var data in activityList)
            {

                if (departmentLookup.TryGetValue(data.DepartmentId, out var departmentName) && departmentName != null)
                {

                    data.DepartmentName = departmentName;
                }
              
                if (unitLookup.TryGetValue(data.UnitId, out var unitName) && unitName != null)
                {
                    data.UnitName = unitName;
                }
               //   activityMasterDictionary[data.DepartmentId] = data;
               
            }

             var filteredActivities = activityList
            .Where(p => departmentLookup.ContainsKey(p.DepartmentId))
            .ToList();


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
                Data = filteredActivities,
                TotalCount = filteredActivities.Count(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }


    }
}