using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineMaster.Queries.GetMachineMaster
{
    public class GetMachineMasterQueryHandler : IRequestHandler<GetMachineMasterQuery,ApiResponseDTO<List<MachineMasterDto>>>
    {
        private readonly IMachineMasterQueryRepository _imachineMasterQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 


         public GetMachineMasterQueryHandler(IMachineMasterQueryRepository imachineMasterQueryRepository, IMapper mapper, IMediator mediator)
        {
            _imachineMasterQueryRepository = imachineMasterQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;              

        }

        public async Task<ApiResponseDTO<List<MachineMasterDto>>> Handle(GetMachineMasterQuery request, CancellationToken cancellationToken)
        {
           var (MachineMaster, totalCount) = await _imachineMasterQueryRepository.GetAllMachineAsync(request.PageNumber, request.PageSize, request.SearchTerm);
               var machineMastersgroup = _mapper.Map<List<MachineMasterDto>>(MachineMaster);

            //     // 🔥 Fetch departments using gRPC
            //    var departments = await _departmentGrpcClient.GetAllDepartmentsAsync();
            //     var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);

            //     // 4. Enrich each DTO, unwrapping the nullable DepartmentId and setting DepartmentName
            //     foreach (var dto in machineMastersgroup)
            //     {
            //         // Only proceed if DepartmentId has a value
            //         if (dto.DepartmentId.HasValue)
            //         {
            //             var deptId = dto.DepartmentId.Value;              // unwrap the int?
            //             if (departmentLookup.TryGetValue(deptId, out var name))
            //             {
            //                 dto.DepartmentName = name;                    // set the *Name* property
            //             }
            //         }
            //     }

             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetMachineMasterQuery",
                    actionCode: "Get",        
                    actionName: MachineMaster.Count().ToString(),
                    details: $"MachineMaster details was fetched.",
                    module:"MachineMaster"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<MachineMasterDto>> 
            { 
                IsSuccess = true, 
                Message = "Success", 
                Data = machineMastersgroup ,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
                };
        }
    }
}