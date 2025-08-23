using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Interfaces.External.IInvetoryManagement;
using Contracts.Interfaces.External.IUser;
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
        private readonly IUOMGrpcClient _uOMGrpcClient;

        private readonly ICityGrpcClient _cityGrpcClient;
        private readonly ICountryGrpcClient _countryGrpcClient;
        private readonly IStatesGrpcClient _stateGrpcClient; 
        private readonly IMapper _mapper;

        public GetWarehouseMasterByIdQueryHandler(IWarehouseMasterQueryRepository warehouseMasterQueryRepository, IMiscMasterGrpcClient miscMasterGrpcClient,
            IMapper mapper, IUOMGrpcClient uOMGrpcClient, ICityGrpcClient cityGrpcClient, ICountryGrpcClient countryGrpcClient, IStatesGrpcClient stateGrpcClient)
        {
            _warehouseMasterQueryRepository = warehouseMasterQueryRepository;
            _miscMasterGrpcClient = miscMasterGrpcClient;
            _mapper = mapper;
            _uOMGrpcClient = uOMGrpcClient;
            _cityGrpcClient = cityGrpcClient;
            _countryGrpcClient = countryGrpcClient; 
            _stateGrpcClient = stateGrpcClient; 
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
              // fire lookups in parallel
            var whTask     = _miscMasterGrpcClient.GetMiscMasterByIdAsync("WarehouseType");
            var stTask     = _miscMasterGrpcClient.GetMiscMasterByIdAsync("StorageType");
            var areaTask   = _miscMasterGrpcClient.GetMiscMasterByIdAsync("AreaType");
            var opTask     = _miscMasterGrpcClient.GetMiscMasterByIdAsync("OperationType");

            var uomTask    = _uOMGrpcClient.GetUOMAsync();
            var cityTask   = _cityGrpcClient.GetAllCityAsync();
            var stateTask  = _stateGrpcClient.GetAllStateAsync();
            var countryTask= _countryGrpcClient.GetAllCountryAsync();

            await Task.WhenAll(whTask, stTask, areaTask, opTask, uomTask, cityTask, stateTask, countryTask);

            // build dicts
            var whDict      = whTask.Result.ToDictionary(x => x.Id,      x => x.Description);
            var stDict      = stTask.Result.ToDictionary(x => x.Id,      x => x.Description);
            var areaDict    = areaTask.Result.ToDictionary(x => x.Id,    x => x.Description);
            var opDict      = opTask.Result.ToDictionary(x => x.Id,      x => x.Description);

            var uomDict     = uomTask.Result.ToDictionary(x => x.Id,     x => x.UOMName);
            var cityDict    = cityTask.Result.ToDictionary(x => x.CityId,    x => x.CityName);
            var stateDict   = stateTask.Result.ToDictionary(x => x.StateId,   x => x.StateName);
            var countryDict = countryTask.Result.ToDictionary(x => x.CountryId,x => x.CountryName);

            // fill names
            if (whDict.TryGetValue(dto.WarehouseTypeId, out var whName) && whName != null)
                dto.WarehouseTypeName = whName;

            if (stDict.TryGetValue(dto.StorageTypeId, out var stName) && stName != null)
                dto.StorageTypeName = stName;

            if (areaDict.TryGetValue(dto.AreaTypeId, out var areaName) && areaName != null)
                dto.AreaTypeName = areaName;

            if (opDict.TryGetValue(dto.OperationTypeId, out var opName) && opName != null)
                dto.OperationTypeName = opName;

            if (uomDict.TryGetValue(dto.CapacityUOMId, out var uomName) && uomName != null)
                dto.CapacityUOMName = uomName;

            if (cityDict.TryGetValue(dto.CityId, out var cityName) && cityName != null)
                dto.CityName = cityName;

            if (stateDict.TryGetValue(dto.StateId, out var stateName) && stateName != null)
                dto.StateName = stateName;

            if (countryDict.TryGetValue(dto.CountryId, out var countryName) && countryName != null)
                dto.CountryName = countryName;

        //     // Get WarehouseType name from gRPC
        //    var miscMasters = await _miscMasterGrpcClient.GetMiscMasterByIdAsync("WarehouseType");
        //     var dict = miscMasters.ToDictionary(x => x.Id, x => x.Description);
           
        //     if (dict.TryGetValue(dto.WarehouseTypeId, out var typeName))
        //     {
        //         dto.WarehouseTypeName = typeName;
        //     }

        //     if (dict.TryGetValue(dto.StorageTypeId, out var storageTypeName))
        //     {
        //         dto.StorageTypeName = storageTypeName;
        //     }

            return new ApiResponseDTO<WarehouseMasterDto>
            {
                IsSuccess = true,
                Message = "Fetched successfully",
                Data = dto
            };
        }

    }
}