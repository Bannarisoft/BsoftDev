using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.External.IDepartment;
using Core.Application.Common.Interfaces.ILocation;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Location.Queries.GetLocations
{
    public class GetLocationHandlerQuery : IRequestHandler<GetLocationQuery, ApiResponseDTO<List<LocationDto>>>
    {
        private readonly ILocationQueryRepository _locationQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IDepartmentService _departmentService;

        public GetLocationHandlerQuery(ILocationQueryRepository locationQueryRepository, IMediator mediator, IMapper mapper, IDepartmentService departmentService)
        {
            _locationQueryRepository = locationQueryRepository;
            _mediator = mediator;
            _mapper = mapper;
            _departmentService = departmentService;

        }
        public async Task<ApiResponseDTO<List<LocationDto>>> Handle(GetLocationQuery request, CancellationToken cancellationToken)
        {
            var (locations, totalCount) = await _locationQueryRepository.GetAllLocationAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var locationList = _mapper.Map<List<LocationDto>>(locations);

            // 🔥 Fetch departments using HttpClientFactory
            var departments = await _departmentService.GetAllDepartmentAsync();
            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);
            var LocationDictionary = new Dictionary<int, LocationDto>();

            // 🔥 Map department names to location
            foreach (var data in locationList)
            {

                if (departmentLookup.TryGetValue(data.DepartmentId, out var departmentName) && departmentName != null)
                {
                    data.DepartmentName = departmentName;
                }

                LocationDictionary[data.DepartmentId] = data;

            }

            /*  // 🔥 Fetch departments using gRPC
             var departments = await _departmentGrpcClient.GetAllDepartmentsAsync();
             var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);

             // 🔥 Map department names to locations
             foreach (var location in locationList)
             {
                 if (departmentLookup.TryGetValue(location.DepartmentId, out var departmentName) && departmentName != null)
                 {
                     location.DepartmentName = departmentName;
                 }
             } */


            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetLocations",
                actionCode: "",
                actionName: "",
                details: $"Location details was fetched.",
                module: "Location"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<LocationDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = locationList,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}