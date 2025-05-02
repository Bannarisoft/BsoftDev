using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMRS;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MRS.Command.CreateMRS
{
    public class CreateMRSCommandHandler : IRequestHandler<CreateMRSCommand, ApiResponseDTO<int>>
    {
        private readonly IMRSCommandRepository _imrscommandrepository;
        private readonly IMRSQueryRepository _imrsqueryrepository;
        private readonly IMediator _imediator;
        public CreateMRSCommandHandler(IMRSCommandRepository imrscommandrepository, IMediator imediator, IMRSQueryRepository imrsqueryrepository)
        {
            _imrscommandrepository = imrscommandrepository;
            _imediator = imediator;
            _imrsqueryrepository = imrsqueryrepository;
           
        }
        public async Task<ApiResponseDTO<int>> Handle(CreateMRSCommand request, CancellationToken cancellationToken)
        {
            if (request.Header == null)
            {
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Header cannot be null.",
                    Data = 0
                };
            }            
            var irno = await _imrscommandrepository.InsertMRSAsync(request.Header);
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: request.Header.IrDate.ToString(),
                actionName: request.Header.Refno.ToString(),
                details: $"MRS details was created",
                module: "MRS");
            await _imediator.Publish(domainEvent, cancellationToken);

            return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "MRS inserted successfully.",
                Data = irno
            };
        }
    }
}