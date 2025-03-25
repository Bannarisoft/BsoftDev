using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IShiftMasterDetail;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.ShiftMasterDetailCQRS.Commands.DeleteShiftMasterDetail
{
    public class DeleteShiftMasterDetailCommandHandler : IRequestHandler<DeleteShiftMasterDetailCommand, ApiResponseDTO<bool>>
    {
        private readonly IShiftMasterDetailCommand _shiftMasterDetailCommand;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public DeleteShiftMasterDetailCommandHandler(IShiftMasterDetailCommand shiftMasterDetailCommand, IMapper mapper, IMediator mediator)
        {
            _shiftMasterDetailCommand = shiftMasterDetailCommand;
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<bool>> Handle(DeleteShiftMasterDetailCommand request, CancellationToken cancellationToken)
        {
              var shiftMaster  = _mapper.Map<ShiftMasterDetail>(request);
            var shiftMasterresult = await _shiftMasterDetailCommand.DeleteAsync(request.Id,shiftMaster);


                  //Domain Event  
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Delete",
                        actionCode: "delete",
                        actionName: "Delete Shift Master details",
                        details: $"Delete Shift Master details",
                        module:"Shift Master details"
                    );               
                    await _mediator.Publish(domainEvent, cancellationToken);  

                 if(shiftMasterresult)
                {
                    return new ApiResponseDTO<bool>
                    {
                        IsSuccess = true, 
                        Message = "Shift Master detail deleted successfully."
                    };
                }

                return new ApiResponseDTO<bool>
                {
                    IsSuccess = false, 
                    Message = "Shift Master detail not deleted."
                };
        }
    }
}