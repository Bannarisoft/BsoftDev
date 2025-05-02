using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMRS;
using Core.Application.MRS.Queries.GetDepartment;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MRS.Queries
{
    public class GetDepartmentbyIdQueryHandler : IRequestHandler<GetDepartmentbyIdQuery, ApiResponseDTO<List<MDepartmentDto>>>
{
    private readonly IMRSQueryRepository _imRSQueryRepository;        
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public GetDepartmentbyIdQueryHandler(IMRSQueryRepository imRSQueryRepository, IMapper mapper, IMediator mediator)
    {
        _imRSQueryRepository = imRSQueryRepository;            
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<ApiResponseDTO<List<MDepartmentDto>>> Handle(GetDepartmentbyIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _imRSQueryRepository.GetMDepartment(request.OldUnitcode);

        if (result == null || !result.Any())
        {
            return new ApiResponseDTO<List<MDepartmentDto>> { IsSuccess = false, Message = $"Department {request.OldUnitcode} not found." };
        }

        var departmentDtos = _mapper.Map<List<MDepartmentDto>>(result);

        var domainEvent = new AuditLogsDomainEvent(
            actionDetail: "GetById",
            actionCode: "GetDepartmentbyIdQuery",
            actionName: request.OldUnitcode,
            details: $"Department {request.OldUnitcode} was fetched.",
            module: "GetDepartment"
        );

        await _mediator.Publish(domainEvent, cancellationToken);

        return new ApiResponseDTO<List<MDepartmentDto>> { IsSuccess = true, Message = "Success", Data = departmentDtos };
    }
}
}