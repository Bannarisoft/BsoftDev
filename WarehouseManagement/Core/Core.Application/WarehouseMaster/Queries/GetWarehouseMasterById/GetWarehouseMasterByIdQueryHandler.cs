using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Interfaces.External.IInvetoryManagement;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IWarehouseMaster;
using Core.Application.WarehouseMaster.GetAllWarehouseMaster;
using MediatR;

namespace Core.Application.WarehouseMaster.GetWarehouseMasterById
{
    public class GetWarehouseMasterByIdQueryHandler : IRequestHandler<GetWarehouseMasterByIdQuery, ApiResponseDTO<WarehouseMasterDto>>
    {

        private readonly IWarehouseMasterQueryRepository _warehouseMasterQueryRepository;
        private readonly IMiscMasterGrpcClient _miscMasterGrpcClient;
        private readonly IMapper _mapper;

        public GetWarehouseMasterByIdQueryHandler(IWarehouseMasterQueryRepository warehouseMasterQueryRepository, IMiscMasterGrpcClient miscMasterGrpcClient,
            IMapper mapper)
        {
            _warehouseMasterQueryRepository = warehouseMasterQueryRepository;
            _miscMasterGrpcClient = miscMasterGrpcClient;
            _mapper = mapper;
        }
        
        public async Task<ApiResponseDTO<WarehouseMasterDto>> Handle(GetWarehouseMasterByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _warehouseMasterQueryRepository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                return new ApiResponseDTO<WarehouseMasterDto>
                {
                    IsSuccess = false,
                    Message = "Warehouse Master not found"
                };
            }

            var dto = _mapper.Map<WarehouseMasterDto>(entity);

            // Get WarehouseType name from gRPC
           var miscMasters = await _miscMasterGrpcClient.GetMiscMasterByIdAsync("WarehouseType");
            var dict = miscMasters.ToDictionary(x => x.Id, x => x.Description);
           
            if (dict.TryGetValue(dto.WarehouseTypeId, out var typeName))
            {
                dto.WarehouseTypeName = typeName;
            }

            if (dict.TryGetValue(dto.StorageTypeId, out var storageTypeName))
            {
                dto.StorageTypeName = storageTypeName;
            }

            return new ApiResponseDTO<WarehouseMasterDto>
            {
                IsSuccess = true,
                Message = "Fetched successfully",
                Data = dto
            };
        }

    }
}