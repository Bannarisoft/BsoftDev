using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IShiftMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.ShiftMasterCQRS.Commands.CreateShiftMaster
{
    public class CreateShiftMasterCommandHandler : IRequestHandler<CreateShiftMasterCommand, ApiResponseDTO<int>>
    {
        private readonly IShiftMasterCommand _shiftMasterCommand;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public CreateShiftMasterCommandHandler(IShiftMasterCommand shiftMasterCommand, IMediator mediator, IMapper mapper)
        {
            _shiftMasterCommand = shiftMasterCommand;
            _mediator = mediator;
            _mapper = mapper;
        }
        public async Task<ApiResponseDTO<int>> Handle(CreateShiftMasterCommand request, CancellationToken cancellationToken)
        {
            var shiftMaster  = _mapper.Map<Core.Domain.Entities.ShiftMaster>(request);

                var shiftMasterresult = await _shiftMasterCommand.CreateAsync(shiftMaster);
                
                
                if (shiftMasterresult > 0)
                {
                    var domainEvent = new AuditLogsDomainEvent(
                     actionDetail: "Create",
                     actionCode: "NewData",
                     actionName: "Shift Master Creation",
                     details: $"Shift Master Creation",
                     module:"Shift Master"
                 );
                 await _mediator.Publish(domainEvent, cancellationToken);
                 
                    return new ApiResponseDTO<int>
                    {
                        IsSuccess = true, 
                        Message = "Shift Master successfully", 
                        Data = shiftMasterresult
                    };
                }
               
                    return new ApiResponseDTO<int>
                    {
                        IsSuccess = false, 
                        Message = "Shift Master not created"
                    };
        }
    }
}