using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Common.Interfaces.ILogService;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.Common.Interfaces.IPurchaseIndent;
using Core.Domain.Common;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.PurchaseIndents.Command.UpdatePurchaseIndent
{
    public class UpdatePurchaseIndentCommandHandler : IRequestHandler<UpdatePurchaseIndentCommand, bool>
    {
        private readonly IPurchaseIndentCommand _purchaseIndentCommand;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        public UpdatePurchaseIndentCommandHandler(IPurchaseIndentCommand purchaseIndentCommand, IMediator imediator, IMapper imapper)
        {
            _purchaseIndentCommand = purchaseIndentCommand;
            _imediator = imediator;
            _imapper = imapper;
        }
        public async Task<bool> Handle(UpdatePurchaseIndentCommand request, CancellationToken cancellationToken)
        {
            var Indent = _imapper.Map<IndentHeader>(request);
            
            var result = await _purchaseIndentCommand.UpdateAsync(Indent,JsonSerializer.Serialize(request));
            
          
            return result == true ? result : throw new ExceptionRules("Indent update failed."); 
        }
    }
}