using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces.IPurchaseIndent;
using MediatR;

namespace Core.Application.PurchaseIndents.Queries.GetPurchaseIndentById
{
    public class GetPurchaseIndentByIdQueryHandler : IRequestHandler<GetPurchaseIndentByIdQuery, IndentByIdDto>
    {
        private readonly IPurchaseIndentQuery _purchaseIndentQuery;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public GetPurchaseIndentByIdQueryHandler(IPurchaseIndentQuery purchaseIndentQuery, IMediator mediator, IMapper mapper)
        {
            _purchaseIndentQuery = purchaseIndentQuery;
            _mediator = mediator;
            _mapper = mapper;
        }
        public async Task<IndentByIdDto> Handle(GetPurchaseIndentByIdQuery request, CancellationToken cancellationToken)
        {
             var result = await _purchaseIndentQuery.GetByIdAsync(request.Id);     
               
            var Indent = _mapper.Map<IndentByIdDto>(result);
            
            return Indent;
        }
    }
}