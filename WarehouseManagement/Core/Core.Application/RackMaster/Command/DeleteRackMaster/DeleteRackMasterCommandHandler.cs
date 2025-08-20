using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IRackMaster;
using Core.Application.WarehouseMaster.Command.DeleteWarehouseMaster;
using MediatR;

namespace Core.Application.RackMaster.Command.DeleteRackMaster
{
    public class DeleteRackMasterCommandHandler : IRequestHandler<DeleteRackMasterCommand, bool>
    {

         public readonly IRackMasterCommandRepository _rackMasterCommandRepository;

         public DeleteRackMasterCommandHandler( IRackMasterCommandRepository rackMasterCommandRepository)
        {
            _rackMasterCommandRepository = rackMasterCommandRepository;
        }

        public async Task<bool> Handle(DeleteRackMasterCommand request, CancellationToken cancellationToken)
        {
       
             var entity = await _rackMasterCommandRepository.GetByIdAsync(request.Id);
            if (entity is null || entity.IsDeleted == Core.Domain.Common.BaseEntity.IsDelete.Deleted)
                return false; 
           
            var deleted = await _rackMasterCommandRepository.DeleteAsync(request.Id, entity);
            return deleted;
       
        }
    }
}