using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Common.Interfaces.IPurchaseIndent;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.PurchaseIndents.Command.CreatePurchaseIndent
{
    public class CreatePurchaseIndentCommandHandler : IRequestHandler<CreatePurchaseIndentCommand, int>
    {
        private readonly IMapper _imapper;
        private readonly IMediator _mediator;
        private readonly IPurchaseIndentCommand _purchaseIndentCommand;
        public CreatePurchaseIndentCommandHandler(IPurchaseIndentCommand purchaseIndentCommand, IMapper imapper, IMediator mediator)
        {
            _purchaseIndentCommand = purchaseIndentCommand;
            _imapper = imapper;
            _mediator = mediator;
        }
        public async Task<int> Handle(CreatePurchaseIndentCommand request, CancellationToken cancellationToken)
        {
            var IndentHeader = _imapper.Map<IndentHeader>(request);
            
            var result = await _purchaseIndentCommand.CreateAsync(IndentHeader);
            
            return result > 0 ? result : throw new ExceptionRules("Indent Header Creation Failed.");
        }
    }
}