using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMRS;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MRS.Queries.GetSubDepartment
{
    public class GetSubDepartmentQueryHandler : IRequestHandler<GetSubDepartmentQuery, ApiResponseDTO<List<MSubDepartment>>>
    {
        private readonly IMRSQueryRepository _imRSQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public GetSubDepartmentQueryHandler(IMRSQueryRepository imRSQueryRepository, IMapper mapper, IMediator mediator)
        {
        _imRSQueryRepository = imRSQueryRepository;            
        _mapper = mapper;
        _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<MSubDepartment>>> Handle(GetSubDepartmentQuery request, CancellationToken cancellationToken)
        {
        var result = await _imRSQueryRepository.GetSubDepartment(request.OldUnitcode);

        if (result == null || !result.Any())
        {
            return new ApiResponseDTO<List<MSubDepartment>> { IsSuccess = false, Message = $"SubDepartment {request.OldUnitcode} not found." };
        }

        var departmentDtos = _mapper.Map<List<MSubDepartment>>(result);

        var domainEvent = new AuditLogsDomainEvent(
            actionDetail: "GetById",
            actionCode: "GetSubDepartmentQuery",
            actionName: request.OldUnitcode,
            details: $"SubDepartment {request.OldUnitcode} was fetched.",
            module: "GetSubDepartment"
        );

        await _mediator.Publish(domainEvent, cancellationToken);

        return new ApiResponseDTO<List<MSubDepartment>> { IsSuccess = true, Message = "Success", Data = departmentDtos };
        }
    }
}