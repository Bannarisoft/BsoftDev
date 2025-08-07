using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IPurchaseIndent;
using MediatR;

namespace Core.Application.PurchaseIndents.Queries.GetAllPurchaseIndent
{
    public class GetAllPurchaseIndentQueryHandler : IRequestHandler<GetAllPurchaseIndentQuery, ApiResponseDTO<List<IndentDto>>>
    {
        private readonly IPurchaseIndentQuery _purchaseIndentQuery;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IUnitGrpcClient _unitGrpcClient;
        public GetAllPurchaseIndentQueryHandler(IPurchaseIndentQuery purchaseIndentQuery, IMediator mediator, IMapper mapper, IUnitGrpcClient unitGrpcClient)
        {
            _purchaseIndentQuery = purchaseIndentQuery;
            _mediator = mediator;
            _mapper = mapper;
            _unitGrpcClient = unitGrpcClient;
        }
        public async Task<ApiResponseDTO<List<IndentDto>>> Handle(GetAllPurchaseIndentQuery request, CancellationToken cancellationToken)
        {
            
            
            var (Indent, TotalCount) = await _purchaseIndentQuery.GetAllPurchaseIndentAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var IndentDto = _mapper.Map<List<IndentDto>>(Indent);

            var Units = await _unitGrpcClient.GetAllUnitAsync();
            var UnitLookup = Units.ToDictionary(d => d.UnitId, d => d.UnitName);

             foreach (var dto in IndentDto)
        {
            if (UnitLookup.TryGetValue(dto.UnitId, out var UnitName))
            {
                dto.UnitName = UnitName;
            }
        }

                var FilteredIndent = IndentDto
            .Where(p => UnitLookup.ContainsKey(p.UnitId))
            .ToList();

            return new ApiResponseDTO<List<IndentDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = FilteredIndent,
                TotalCount = TotalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}