using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ISubLocation;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.SubLocation.Queries.GetSubLocations
{
    public class GetSubLocationHandlerQuery : IRequestHandler<GetSubLocationQuery, ApiResponseDTO<List<SubLocationDto>>>
    {
        private readonly ISubLocationQueryRepository _sublocationQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        //private readonly IDepartmentGrpcClient _departmentGrpcClient; // ✅ Interface, not DepartmentServiceClient
        

        public GetSubLocationHandlerQuery(ISubLocationQueryRepository sublocationQueryRepository, IMapper mapper, IMediator mediator)
        {
            _sublocationQueryRepository = sublocationQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        
        }
        public async Task<ApiResponseDTO<List<SubLocationDto>>> Handle(GetSubLocationQuery request, CancellationToken cancellationToken)
        {
            var (sublocations, totalCount) = await _sublocationQueryRepository.GetAllSubLocationAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var sublocationList = _mapper.Map<List<SubLocationDto>>(sublocations);

         /*    // 🔥 Fetch departments using gRPC
            var departments = await _departmentGrpcClient.GetAllDepartmentsAsync();
            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);

            // 🔥 Map department names to sublocations
            foreach (var sublocation in sublocationList)
            {
                if (departmentLookup.TryGetValue(sublocation.DepartmentId, out var departmentName) && departmentName != null)
                {
                    sublocation.DepartmentName = departmentName;
                }
            }
 */
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetSubLocations",
                actionCode: "",
                actionName: "",
                details: $"SubLocation details was fetched.",
                module: "SubLocation"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<SubLocationDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = sublocationList,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}