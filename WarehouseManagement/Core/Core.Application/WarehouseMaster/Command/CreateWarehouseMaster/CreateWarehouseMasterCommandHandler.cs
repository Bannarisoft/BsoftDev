using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Core.Application.Common.Interfaces.IWarehouseMaster;
using Core.Application.WarehouseMaster.GetAllWarehouseMaster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Core.Domain.Entities;
using Contracts.Interfaces.External.IInvetoryManagement;


namespace Core.Application.WarehouseMaster.Command.CreateWarehouseMaster
{
    public class CreateWarehouseMasterCommandHandler : IRequestHandler<CreateWarehouseMasterCommand, int>
    {

        private readonly IWarehouseCodeGenerator _warehouseCodeGenerator;
        private readonly IWarehouseMasterCommandRepository _warehouseMasterCommandRepository;
        private readonly IMapper _mapper;

        private readonly IItemGroupGrpcClient  _itemGroupGrpcClient;



        public CreateWarehouseMasterCommandHandler(IWarehouseCodeGenerator warehouseCodeGenerator, IWarehouseMasterCommandRepository warehouseMasterCommandRepository, IMapper mapper, IItemGroupGrpcClient itemGroupGrpcClient)
        {
            _warehouseCodeGenerator = warehouseCodeGenerator;
            _warehouseMasterCommandRepository = warehouseMasterCommandRepository;
            _mapper = mapper;
            _itemGroupGrpcClient = itemGroupGrpcClient;
        }
        public async Task<int> Handle(CreateWarehouseMasterCommand request, CancellationToken cancellationToken)
        {
            // Generate unique warehouse code
            var warehouseCode = await _warehouseCodeGenerator.GenerateAsync(
                request.UnitId, 
                request.WarehouseTypeId
            );

            // Map request to entity
            var warehouse = _mapper.Map<Core.Domain.Entities.WarehouseMaster>(request);
            warehouse.WarehouseCode = warehouseCode;
            

              if (request.AllowedItemGroupIds == null || !request.AllowedItemGroupIds.Any())
            {
                var itemGroups = await _itemGroupGrpcClient.GetAllItemGroupsAsync();
                request.AllowedItemGroupIds = itemGroups
                    .Where(g => g.IsActive) // Only active ones
                    .Select(g => g.Id)
                    .ToList();
            }

            // Add allowed item group mappings
            foreach (var itemGroupId in request.AllowedItemGroupIds)
            {
                warehouse.AllowedItemGroups.Add(new WarehouseItemGroupMapping
                {
                    ItemGroupId = itemGroupId,
                    IsActive = Core.Domain.Common.BaseEntity.Status.Active,
                    IsDeleted = Core.Domain.Common.BaseEntity.IsDelete.NotDeleted

                });
            }

            // Save warehouse and mappings
            var newId = await _warehouseMasterCommandRepository.CreateAsync(warehouse);
            return newId;
        }
       
        
    }
}