using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IWarehouseMaster;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.WarehouseMaster.Command.UpdateWarehouseMaster
{
    public class UpdateWarehouseMasterCommandHandler : IRequestHandler<UpdateWarehouseMasterCommand, ApiResponseDTO<bool>>
    {

        private readonly IWarehouseMasterCommandRepository _warehouseMasterCommandRepository;
        private readonly IWarehouseMasterQueryRepository _warehouseMasterQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;


        public UpdateWarehouseMasterCommandHandler(IWarehouseMasterCommandRepository warehouseMasterCommandRepository, IWarehouseMasterQueryRepository warehouseMasterQueryRepository, IMapper mapper, IMediator mediator)
        {
            _warehouseMasterCommandRepository = warehouseMasterCommandRepository;
            _warehouseMasterQueryRepository = warehouseMasterQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<bool>> Handle(UpdateWarehouseMasterCommand request, CancellationToken cancellationToken)
        {
            // Step 1: Check if entity exists
            var warehouse = await _warehouseMasterCommandRepository.GetByIdAsync(request.Id);
            if (warehouse == null)
            {
                return new ApiResponseDTO<bool>
                {
                    IsSuccess = false,
                    Message = "WarehouseMaster not found."
                };
            }

            // Step 2: Map update fields
            _mapper.Map(request, warehouse);

            // Step 3: Handle Allowed Item Group Updates
            warehouse.AllowedItemGroups.Clear();
            foreach (var itemGroupId in request.AllowedItemGroupIds)
            {
                warehouse.AllowedItemGroups.Add(new WarehouseItemGroupMapping
                {
                    ItemGroupId = itemGroupId,
                    IsActive = Core.Domain.Common.BaseEntity.Status.Active,
                    IsDeleted = Core.Domain.Common.BaseEntity.IsDelete.NotDeleted
                });
            }

            // Step 4: Save
            var updatedId = await _warehouseMasterCommandRepository.UpdateAsync(warehouse);

            // Step 5: Audit Log
            var audit = new AuditLogsDomainEvent(
                actionDetail: "Update",
                actionCode: "WAREHOUSE_UPDATE",
                actionName: request.WarehouseName,
                details: $"WarehouseMaster '{request.WarehouseName}' updated successfully with ID {updatedId}.",
                module: "WarehouseMaster");
            await _mediator.Publish(audit, cancellationToken);

            // Step 6: Return response

            return new ApiResponseDTO<bool>
            {
                IsSuccess = true,
                Message = "WarehouseMaster updated successfully."

            };
               
        }
    }
}