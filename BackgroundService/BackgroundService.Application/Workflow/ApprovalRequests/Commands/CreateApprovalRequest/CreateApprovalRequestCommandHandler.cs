using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRequest;
using MediatR;

namespace BackgroundService.Application.Workflow.ApprovalRequests.Commands.CreateApprovalRequest
{
    public class CreateApprovalRequestCommandHandler : IRequestHandler<CreateApprovalRequestCommand, bool>
    {
        private readonly IApprovalRequestCommand _approvalRequestCommand;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        public CreateApprovalRequestCommandHandler(IApprovalRequestCommand approvalRequestCommand, IMediator imediator, IMapper imapper)
        {
            _approvalRequestCommand = approvalRequestCommand;
            _imediator = imediator;
            _imapper = imapper;
        }
        public Task<bool> Handle(CreateApprovalRequestCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}