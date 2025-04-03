using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineMaster.Command.UpdateMachineMaster
{
    public class UpdateMachineMasterCommandHandler : IRequestHandler<UpdateMachineMasterCommand, ApiResponseDTO<int>>
    {
        private readonly IMachineMasterCommandRepository _iMachineMasterCommandRepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        
        public UpdateMachineMasterCommandHandler(IMachineMasterCommandRepository iMachineMasterCommandRepository, IMediator imediator, IMapper imapper)
        {
            _iMachineMasterCommandRepository = iMachineMasterCommandRepository;
            _imediator = imediator;
            _imapper = imapper;
        }

        public async Task<ApiResponseDTO<int>> Handle(UpdateMachineMasterCommand request, CancellationToken cancellationToken)
        {
             var machineMaster = _imapper.Map<Core.Domain.Entities.MachineMaster>(request);
            var result = await _iMachineMasterCommandRepository.UpdateAsync(request.Id, machineMaster);
            if (result <= 0) // CostCenter not found
            {
               
                return new ApiResponseDTO<int> { IsSuccess = false, Message = "MachineMaster  not found." };
            }
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Update",
                actionCode: machineMaster.Id.ToString(),
                actionName: machineMaster.MachineName ?? "NULL",
                details: $"MachineMaster details was updated",
                module: "MachineMaster");
            await _imediator.Publish(domainEvent, cancellationToken);
           
            return new ApiResponseDTO<int> { IsSuccess = true, Message = "MachineMaster Updated Successfully.", Data = result }; 
        }
    }
}