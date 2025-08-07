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

namespace Core.Application.PurchaseIndents.Command.DeletePurchaseIndent
{
    public class DeletePurchaseIndentCommandHandler : IRequestHandler<DeletePurchaseIndentCommand, bool>
    {
        private readonly IPurchaseIndentCommand _purchaseIndentCommand;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        private readonly ILogServiceCommand _logServiceCommand;
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
        public DeletePurchaseIndentCommandHandler(IPurchaseIndentCommand purchaseIndentCommand, IMediator imediator, IMapper imapper,
            ILogServiceCommand logServiceCommand, IMiscMasterQueryRepository miscMasterQueryRepository)
        {
            _purchaseIndentCommand = purchaseIndentCommand;
            _imediator = imediator;
            _imapper = imapper;
            _logServiceCommand = logServiceCommand;
            _miscMasterQueryRepository = miscMasterQueryRepository;
        }
        public async Task<bool> Handle(DeletePurchaseIndentCommand request, CancellationToken cancellationToken)
        {
             var Indent = _imapper.Map<IndentHeader>(request);
            var result = await _purchaseIndentCommand.DeleteAsync(request.Id,Indent);

            var StatusMisc = await _miscMasterQueryRepository.GetMiscMasterByName(MiscEnumEntity.Status, MiscEnumEntity.Deleted);
             var IndentLog = new IndentLog
            {
                IndentHeaderId = request.Id,
                ActionType = "Deleted",
                ActionRemarks = "Indent Deleted",
                StatusId = StatusMisc.Id
            };

                await _logServiceCommand.CreateAsync(IndentLog);
        
            return result == true ? result : throw new ExceptionRules("Indent deletion failed.");
        }
    }
}