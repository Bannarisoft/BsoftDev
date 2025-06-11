using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.Power.IFeeder;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Power.Feeder.Queries.GetFeeder
{
    public class GetFeederQueryHandler : IRequestHandler<GetFeederQuery, ApiResponseDTO<List<GetFeederDto>>>
    {
        private readonly IFeederQueryRepository _feederQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IDepartmentAllGrpcClient _departmentAllGrpcClient;

        public GetFeederQueryHandler(IFeederQueryRepository feederQueryRepository, IMapper mapper, IMediator mediator, IDepartmentAllGrpcClient departmentAllGrpcClient)
        {
            _feederQueryRepository = feederQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
            _departmentAllGrpcClient = departmentAllGrpcClient;
        }

        public async Task<ApiResponseDTO<List<GetFeederDto>>> Handle(GetFeederQuery request, CancellationToken cancellationToken)
        {
            var (Feeder, totalCount) = await _feederQueryRepository.GetAllFeederAsync(request.PageNumber, request.PageSize, request.SearchTerm);            
            var Feederlist = _mapper.Map<List<GetFeederDto>>(Feeder);  

             var departments = await _departmentAllGrpcClient.GetDepartmentAllAsync();
            var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);
            // 🔥 Map department & unit names with DataControl to costCenters
            foreach (var dto in Feederlist)
            {
                if (departmentLookup.TryGetValue(dto.DepartmentId, out var deptName))
                    dto.DepartmentName = deptName;

            }
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetFeederGroupQuery",
                actionCode: "Get",        
                actionName: Feeder.Count().ToString(),
                details: $"FeederGroup details was fetched",
                module:"FeederGroup"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<GetFeederDto>> 
            { 
                IsSuccess = true, 
                Message = "Success", 
                Data = Feederlist ,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
            
        }
    }
}