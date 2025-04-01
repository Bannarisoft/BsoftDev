using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IShiftMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.ShiftMasters.Commands.UpdateShiftMaster
{
    public class UpdateShiftMasterCommandHandler : IRequestHandler<UpdateShiftMasterCommand, ApiResponseDTO<bool>>
    {
        private readonly IShiftMasterCommand _shiftMasterCommand;
         private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public UpdateShiftMasterCommandHandler(IShiftMasterCommand shiftMasterCommand, IMapper mapper, IMediator mediator)
        {
            _shiftMasterCommand = shiftMasterCommand;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<bool>> Handle(UpdateShiftMasterCommand request, CancellationToken cancellationToken)
        {
             var shiftMaster  = _mapper.Map<Core.Domain.Entities.ShiftMaster>(request);
         
                var shiftMasterresult = await _shiftMasterCommand.UpdateAsync(shiftMaster);

                
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Update",
                        actionCode: "update",
                        actionName: "Update ShiftMaster",
                        details: $"Update ShiftMaster",
                        module:"ShiftMaster"
                    );               
                    await _mediator.Publish(domainEvent, cancellationToken); 
              
                if(shiftMasterresult)
                {
                    return new ApiResponseDTO<bool>
                    {
                        IsSuccess = true, 
                        Message = "ShiftMaster updated successfully."
                    };
                }

                return new ApiResponseDTO<bool>
                {
                    IsSuccess = false, 
                    Message = "ShiftMaster not updated."
                };
        }
    }
}