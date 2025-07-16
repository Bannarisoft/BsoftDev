using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IShiftMaster;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.ShiftMasters.Commands.DeleteShiftMaster
{
    public class DeleteShiftMasterCommandHandler : IRequestHandler<DeleteShiftMasterCommand, ApiResponseDTO<bool>>
    {
        private readonly IShiftMasterCommand _shiftMasterCommand;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public DeleteShiftMasterCommandHandler(IShiftMasterCommand shiftMasterCommand, IMapper mapper, IMediator mediator)
        {
            _shiftMasterCommand = shiftMasterCommand;
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<bool>> Handle(DeleteShiftMasterCommand request, CancellationToken cancellationToken)
        {
              var shiftMaster  = _mapper.Map<Core.Domain.Entities.ShiftMaster>(request);
            var shiftMasterresult = await _shiftMasterCommand.DeleteAsync(request.Id,shiftMaster);


                  //Domain Event  
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Delete",
                        actionCode: "delete",
                        actionName: "Delete Shift Master",
                        details: $"Delete Shift Master",
                        module:"Shift Master"
                    );               
                    await _mediator.Publish(domainEvent, cancellationToken);  

                 if(shiftMasterresult)
                {
                    return new ApiResponseDTO<bool>
                    {
                        IsSuccess = true, 
                        Message = "Shift Master deleted successfully."
                    };
                }

                return new ApiResponseDTO<bool>
                {
                    IsSuccess = false, 
                    Message = "Shift Master not deleted."
                };
        }
    }
}