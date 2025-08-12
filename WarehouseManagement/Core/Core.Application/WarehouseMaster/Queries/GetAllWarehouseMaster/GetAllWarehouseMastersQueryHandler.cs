using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IWarehouseMaster;
using Core.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore.Query;
using Contracts.Interfaces.External.IInvetoryManagement;
using Contracts.Interfaces.External.IUser;

namespace Core.Application.WarehouseMaster.GetAllWarehouseMaster
{
    public class GetAllWarehouseMastersQueryHandler : IRequestHandler<GetAllWarehouseMastersQuery, ApiResponseDTO<List<WarehouseMasterDto>>>
    {

        private readonly IWarehouseMasterQueryRepository _iWarehouseMasterQueryRepository;
        private readonly IMiscMasterGrpcClient _miscMasterGrpcClient;
        private readonly IUOMGrpcClient _uOMGrpcClient;

        private readonly ICityGrpcClient _cityGrpcClient;
        private readonly ICountryGrpcClient _countryGrpcClient;
        private readonly IStatesGrpcClient _stateGrpcClient;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public GetAllWarehouseMastersQueryHandler(IWarehouseMasterQueryRepository warehouseMasterQueryRepository, IMapper mapper, IMediator mediator, IMiscMasterGrpcClient miscMasterGrpcClient, IUOMGrpcClient uOMGrpcClient, ICityGrpcClient cityGrpcClient,
            ICountryGrpcClient countryGrpcClient, IStatesGrpcClient stateGrpcClient)
        {
            _iWarehouseMasterQueryRepository = warehouseMasterQueryRepository;
            _mediator = mediator;
            _mapper = mapper;
            _miscMasterGrpcClient = miscMasterGrpcClient;
            _uOMGrpcClient = uOMGrpcClient;           
            _cityGrpcClient = cityGrpcClient;
            _countryGrpcClient = countryGrpcClient;
            _stateGrpcClient = stateGrpcClient;
        }
         public async Task<ApiResponseDTO<List<WarehouseMasterDto>>> Handle(GetAllWarehouseMastersQuery request, CancellationToken cancellationToken)
        {

          
             // var departmentLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);
            // Get paginated + searched data from DB
            var (warehouseEntities, totalCount) = await _iWarehouseMasterQueryRepository.GetAllAsync(
                request.PageNumber,
                request.PageSize,
                request.SearchTerm
            );
            var warehouseListdto = _mapper.Map<List<WarehouseMasterDto>>(warehouseEntities);
            var whTask   = _miscMasterGrpcClient.GetMiscMasterByIdAsync("WarehouseType");
            var stTask   = _miscMasterGrpcClient.GetMiscMasterByIdAsync("StorageType");
            var areaTask = _miscMasterGrpcClient.GetMiscMasterByIdAsync("AreaType");
            var opTask   = _miscMasterGrpcClient.GetMiscMasterByIdAsync("OperationType");

            var uomTask    = _uOMGrpcClient.GetUOMAsync();                 // add ct if your client supports it
            var cityTask   = _cityGrpcClient.GetAllCityAsync();
            var stateTask  = _stateGrpcClient.GetAllStateAsync();
            var countryTask= _countryGrpcClient.GetAllCountryAsync();

            // await all
            await Task.WhenAll(whTask, stTask, areaTask, opTask, uomTask, cityTask, stateTask, countryTask);

            // build dicts
            var whDict   = whTask.Result.ToDictionary(x => x.Id, x => x.Description);
            var stDict   = stTask.Result.ToDictionary(x => x.Id, x => x.Description);
            var areaDict = areaTask.Result.ToDictionary(x => x.Id, x => x.Description);
            var opDict   = opTask.Result.ToDictionary(x => x.Id, x => x.Description);

            var uomDict    = uomTask.Result.ToDictionary(x => x.Id,     x => x.UOMName);
            var cityDict   = cityTask.Result.ToDictionary(x => x.CityId,   x => x.CityName);
            var stateDict  = stateTask.Result.ToDictionary(x => x.StateId,  x => x.StateName);
            var countryDict= countryTask.Result.ToDictionary(x => x.CountryId,x => x.CountryName);

            // enrich DTOs
            foreach (var d in warehouseListdto)
            {
                if (whDict.TryGetValue(d.WarehouseTypeId, out var whName) && whName != null)
                    d.WarehouseTypeName = whName;

                if (stDict.TryGetValue(d.StorageTypeId, out var stName) && stName != null)
                    d.StorageTypeName = stName;

                if (areaDict.TryGetValue(d.AreaTypeId, out var areaName) && areaName != null)
                    d.AreaTypeName = areaName;

                if (opDict.TryGetValue(d.OperationTypeId, out var opName) && opName != null)
                    d.OperationTypeName = opName;

                if (uomDict.TryGetValue(d.CapacityUOMId, out var uomName) && uomName != null)
                    d.CapacityUOMName = uomName;

                if (cityDict.TryGetValue(d.CityId, out var cityName) && cityName != null)
                    d.CityName = cityName;

                if (stateDict.TryGetValue(d.StateId, out var stateName) && stateName != null)
                    d.StateName = stateName;

                if (countryDict.TryGetValue(d.CountryId, out var countryName) && countryName != null)
                    d.CountryName = countryName;
            }
            // var miscMasters = await _miscMasterGrpcClient.GetMiscMasterByIdAsync("WarehouseType");          
            // var stTask   = _miscMasterGrpcClient.GetMiscMasterByIdAsync("StorageType");
            // var areaTask = _miscMasterGrpcClient.GetMiscMasterByIdAsync("AreaType");
            // var opTask   = _miscMasterGrpcClient.GetMiscMasterByIdAsync("OperationType");

            // var dict = miscMasters.ToDictionary(x => x.Id, x => x.Description);
            
            // var uOMs = await _uOMGrpcClient.GetUOMAsync();
            // var uOMDict = uOMs.ToDictionary(x => x.Id, x => x.UOMName);

            // var city = await _cityGrpcClient.GetAllCityAsync();
            // var cityDict = city.ToDictionary(x => x.CityId, x => x.CityName);
            // var state = await _stateGrpcClient.GetAllStateAsync();
            // var stateDict = state.ToDictionary(x => x.StateId, x => x.StateName);
            // var country = await _countryGrpcClient.GetAllCountryAsync();
            // var countryDict = country.ToDictionary(x => x.CountryId, x => x.CountryName);

            // foreach (var data in warehouseListdto)
            // {
            //     if (dict.TryGetValue(data.WarehouseTypeId, out var warehouseTypeName) && warehouseTypeName != null)
            //     {
            //         data.WarehouseTypeName = warehouseTypeName;
            //     }

            //     if (dict.TryGetValue(data.StorageTypeId, out var storageTypeName) && storageTypeName != null)
            //     {
            //         data.StorageTypeName = storageTypeName;
            //         //data.WarehouseTypeName = dict.GetValueOrDefault(data.WarehouseTypeId,))
            //     }

            //     if(dict.TryGetValue(data.AreaTypeId, out var areaTypeName) && areaTypeName != null) //data.WarehouseTypeName = dict.GetValueOrDefault(data.WarehouseTypeId  ,))
            //     {
            //         data.AreaTypeName = areaTypeName;
            //     }
            //     if(dict.TryGetValue(data.OperationTypeId , out var operationTypeName) && operationTypeName != null) //data.WarehouseTypeName = dict.GetValueOrDefault(data.WarehouseTypeId  ,))
            //     {
            //         data.OperationTypeName = operationTypeName;
            //     }

            //     if (uOMDict.TryGetValue(data.CapacityUOMId, out var uOMName) && uOMName != null)
            //     {
            //         data.CapacityUOMName = uOMName;
            //     }              
            //     if (cityDict.TryGetValue(data.CityId, out var cityName) && cityName != null)
            //     {
            //         data.CityName = cityName;
            //     }
            //     if (stateDict.TryGetValue(data.StateId, out var stateName) && stateName != null)
            //     {
            //         data.StateName = stateName;
            //     }
            //     if (countryDict.TryGetValue(data.CountryId, out var countryName) && countryName != null)
            //     {
            //         data.CountryName = countryName;
            //     }


            
            // Map to DTO
                //  var warehouseList = _mapper.Map<List<WarehouseMasterDto>>(warehouseEntities);

                // Domain Event for auditing
                var auditEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "",
                actionName: "",
                details: "Warehouse Master data fetched",
                module: "WarehouseMaster"
            );
            await _mediator.Publish(auditEvent, cancellationToken);

            // Wrap response
            return new ApiResponseDTO<List<WarehouseMasterDto>>
            {
                IsSuccess = true,
                Message = "Fetched successfully",
                Data = warehouseListdto,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        
    }
}