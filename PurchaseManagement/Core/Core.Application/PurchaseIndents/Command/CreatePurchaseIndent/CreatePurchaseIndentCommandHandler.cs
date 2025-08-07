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

namespace Core.Application.PurchaseIndents.Command.CreatePurchaseIndent
{
    public class CreatePurchaseIndentCommandHandler : IRequestHandler<CreatePurchaseIndentCommand, int>
    {
        private readonly IMapper _imapper;
        private readonly IMediator _mediator;
        private readonly IPurchaseIndentCommand _purchaseIndentCommand;
        private readonly ILogServiceCommand _logServiceCommand;
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
        public CreatePurchaseIndentCommandHandler(IPurchaseIndentCommand purchaseIndentCommand, IMapper imapper,
        IMediator mediator, ILogServiceCommand logServiceCommand, IMiscMasterQueryRepository miscMasterQueryRepository)
        {
            _purchaseIndentCommand = purchaseIndentCommand;
            _imapper = imapper;
            _mediator = mediator;
            _logServiceCommand = logServiceCommand;
            _miscMasterQueryRepository = miscMasterQueryRepository;
        }
        public async Task<int> Handle(CreatePurchaseIndentCommand request, CancellationToken cancellationToken)
        {
            var IndentHeader = _imapper.Map<IndentHeader>(request);
            
            var result = await _purchaseIndentCommand.CreateAsync(IndentHeader);

            var StatusMisc = await _miscMasterQueryRepository.GetMiscMasterByName(MiscEnumEntity.Status, MiscEnumEntity.Open);

            var IndentLog = new IndentLog
            {
                IndentHeaderId = result,
                ActionType = "Created",
                ActionRemarks = "Indent Created",
                NewData = JsonSerializer.Serialize(request),
                StatusId = StatusMisc.Id
            };

                await _logServiceCommand.CreateAsync(IndentLog);
            
            return result > 0 ? result : throw new ExceptionRules("Indent Creation Failed.");
        }
    }
}