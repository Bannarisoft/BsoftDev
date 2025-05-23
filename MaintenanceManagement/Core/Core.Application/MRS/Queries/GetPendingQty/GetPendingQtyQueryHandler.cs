using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMRS;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MRS.Queries.GetPendingQty
{
    public class GetPendingQtyQueryHandler :  IRequestHandler<GetPendingQtyQuery,ApiResponseDTO<GetPendingQtyDto>>
    {
        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IMRSQueryRepository _imRSQueryRepository;  

        public GetPendingQtyQueryHandler(IMRSQueryRepository imRSQueryRepository, IMapper mapper, IMediator mediator)
        {
            _imRSQueryRepository = imRSQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<GetPendingQtyDto>> Handle(GetPendingQtyQuery request, CancellationToken cancellationToken)
        {
            var result = await _imRSQueryRepository.GetPendingIssueAsync(request.OldUnitcode, request.ItemCode);

            // if (result == null)
            // {
            //     return new ApiResponseDTO<GetPendingQtyDto>
            //     {
            //         IsSuccess = false,
            //         Message = $"Pending issue not found for ItemCode: {request.ItemCode} in UnitCode: {request.OldUnitcode}."
                    
            //     };
            // }

            var pendingQtyDto = _mapper.Map<GetPendingQtyDto>(result);

            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetPendingQty",
                actionCode: "GetPendingQtyQuery",
                actionName: request.ItemCode,
                details: $"PendingQty for ItemCode {request.ItemCode} in UnitCode {request.OldUnitcode} was fetched.",
                module: "IssueRequestPending"
            );

            await _mediator.Publish(domainEvent, cancellationToken);

            return new ApiResponseDTO<GetPendingQtyDto>
            {
                IsSuccess = true,
                Message = "Success",
                Data = pendingQtyDto
            };
        }
    }
}