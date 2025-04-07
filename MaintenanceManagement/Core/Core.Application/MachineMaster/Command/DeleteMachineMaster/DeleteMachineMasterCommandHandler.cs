using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineMaster;
using Core.Application.Common.Interfaces.IMaintenanceType;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineMaster.Command.DeleteMachineMaster
{
    public class DeleteMachineMasterCommandHandler : IRequestHandler<DeleteMachineMasterCommand, ApiResponseDTO<int>>
    {
        
        private readonly IMachineMasterCommandRepository _iMachineMasterCommandRepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
          public DeleteMachineMasterCommandHandler(IMachineMasterCommandRepository iMachineMasterCommandRepository, IMediator imediator, IMapper imapper)
        {
            _iMachineMasterCommandRepository = iMachineMasterCommandRepository;
            _imediator = imediator;
            _imapper = imapper;
        }

        public async Task<ApiResponseDTO<int>> Handle(DeleteMachineMasterCommand request, CancellationToken cancellationToken)
        {
            var machineMaster = _imapper.Map<Core.Domain.Entities.MachineMaster>(request);
            var result = await _iMachineMasterCommandRepository.DeleteAsync(request.Id,machineMaster);
            if (result == -1) 
            {
         
             return new ApiResponseDTO<int> { IsSuccess = false, Message = "MachineMaster not found."};
            }

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Delete",
                actionCode: machineMaster.Id.ToString(),
                actionName: machineMaster.MachineCode ?? "NULL",
                details: $"MachineMaster details was deleted",
                module: "MachineMaster");
            await _imediator.Publish(domainEvent);
            return new ApiResponseDTO<int>
            {
                IsSuccess = true,   
                Data = result,
                Message = "MachineMaster deleted successfully."
    
            };
        }
    }
}