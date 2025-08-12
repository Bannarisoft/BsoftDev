using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IWarehouseMaster;
using MediatR;

namespace Core.Application.WarehouseMaster.Command.DeleteWarehouseMaster
{
    public class DeleteWarehouseMasterCommandHandler : IRequestHandler<DeleteWarehouseMasterCommand, bool>
    {

        public readonly IWarehouseMasterCommandRepository _warehouseMasterCommandRepository;

      public DeleteWarehouseMasterCommandHandler( IWarehouseMasterCommandRepository warehouseMasterCommandRepository)
        {
            _warehouseMasterCommandRepository = warehouseMasterCommandRepository;
        }

        public async Task<bool> Handle(DeleteWarehouseMasterCommand request, CancellationToken cancellationToken)
        {
             var entity = await _warehouseMasterCommandRepository.GetByIdAsync(request.Id);
            if (entity is null || entity.IsDeleted == Core.Domain.Common.BaseEntity.IsDelete.Deleted)
                return false; 

           
            var deleted = await _warehouseMasterCommandRepository.DeleteAsync(request.Id, entity);
            return deleted;
        }
    }
}