using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineMaster.Queries.GetMachineMaster
{
    public class GetMachineMasterQueryHandler : IRequestHandler<GetMachineMasterQuery, ApiResponseDTO<List<MachineMasterDto>>>
    {
        private readonly IMachineMasterQueryRepository _imachineMasterQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IDepartmentGrpcClient _departmentGrpcClient;


        public GetMachineMasterQueryHandler(IMachineMasterQueryRepository imachineMasterQueryRepository, IMapper mapper, IMediator mediator, IDepartmentGrpcClient departmentGrpcClient)
        {
            _imachineMasterQueryRepository = imachineMasterQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
            _departmentGrpcClient = departmentGrpcClient;
        }

        public async Task<ApiResponseDTO<List<MachineMasterDto>>> Handle(GetMachineMasterQuery request, CancellationToken cancellationToken)
        {
            var (MachineMaster, totalCount) = await _imachineMasterQueryRepository.GetAllMachineAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var machineMastersgroup = _mapper.Map<List<MachineMasterDto>>(MachineMaster);

            // // 🔥 Fetch departments using gRPC
            // var departments = await _departmentGrpcClient.GetAllDepartmentAsync(); // ✅ Clean call
            // var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);

            // var maintenanceRequestDictionary = new Dictionary<int, MachineMasterDto>();

            // //    🔥 Map department names with DataControl
            // var filteredmachineMasterDtos = machineMastersgroup
            //     .Where(p => departmentLookup.ContainsKey(p.DepartmentId))
            //     .Select(p => new MachineMasterDto
            //     {
            //         DepartmentId = p.DepartmentId,
            //         DepartmentName = departmentLookup[p.DepartmentId],
            //     })
            //     .ToList();
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetMachineMasterQuery",
                actionCode: "Get",
                actionName: MachineMaster.Count().ToString(),
                details: $"MachineMaster details was fetched.",
                module: "MachineMaster"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<MachineMasterDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = machineMastersgroup,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}