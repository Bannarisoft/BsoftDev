using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMRS;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MRS.Queries.GetCategory
{
    public class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, ApiResponseDTO<List<MCategoryDto>>>
    {
        private readonly IMRSQueryRepository _imRSQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public GetCategoryQueryHandler(IMRSQueryRepository imRSQueryRepository, IMapper mapper, IMediator mediator)
        {
        _imRSQueryRepository = imRSQueryRepository;            
        _mapper = mapper;
        _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<MCategoryDto>>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        {
            var result = await _imRSQueryRepository.GetCategory(request.OldUnitcode);

        if (result == null || !result.Any())
        {
            return new ApiResponseDTO<List<MCategoryDto>> { IsSuccess = false, Message = $"Category {request.OldUnitcode} not found." };
        }

        var departmentDtos = _mapper.Map<List<MCategoryDto>>(result);

        var domainEvent = new AuditLogsDomainEvent(
            actionDetail: "GetById",
            actionCode: "GetCategoryQuery",
            actionName: request.OldUnitcode,
            details: $"Category {request.OldUnitcode} was fetched.",
            module: "GetCategory"
        );

        await _mediator.Publish(domainEvent, cancellationToken);

        return new ApiResponseDTO<List<MCategoryDto>> { IsSuccess = true, Message = "Success", Data = departmentDtos };
        }
    }
}