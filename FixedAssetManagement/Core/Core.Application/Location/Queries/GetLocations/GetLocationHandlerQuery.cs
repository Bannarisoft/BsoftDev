using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
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
        private readonly IDepartmentGrpcClient _departmentGrpcClient;

        public GetLocationHandlerQuery(ILocationQueryRepository locationQueryRepository, IMediator mediator, IMapper mapper, IDepartmentGrpcClient departmentService)
        {
            _locationQueryRepository = locationQueryRepository;
            _mediator = mediator;
            _mapper = mapper;
            _departmentGrpcClient = departmentService;

        }
        public async Task<ApiResponseDTO<List<LocationDto>>> Handle(GetLocationQuery request, CancellationToken cancellationToken)
        {
            var (locations, totalCount) = await _locationQueryRepository.GetAllLocationAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var locationList = _mapper.Map<List<LocationDto>>(locations);

            // 🔥 Fetch departments using gRPC
            var departments = await _departmentGrpcClient.GetAllDepartmentAsync();
            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);
            // var LocationDictionary = new Dictionary<int, LocationDto>();

            // 🔥 Map department names with DataControl to location

            foreach (var dto in locationList)
            {
                if (departmentLookup.TryGetValue(dto.DepartmentId, out var departmentName))
                {
                    dto.DepartmentName = departmentName;
                }
            }

            var filteredLocationDtos = locationList
                .Where(p => departmentLookup.ContainsKey(p.DepartmentId))
                .ToList();

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
                Data = filteredLocationDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}