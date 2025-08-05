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

namespace Core.Application.WarehouseMaster.GetAllWarehouseMaster
{
    public class GetAllWarehouseMastersQueryHandler : IRequestHandler<GetAllWarehouseMastersQuery, ApiResponseDTO<List<WarehouseMasterDto>>>
    {

        private readonly IWarehouseMasterQueryRepository _iWarehouseMasterQueryRepository;


        private readonly IMiscMasterGrpcClient _miscMasterGrpcClient;
        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public GetAllWarehouseMastersQueryHandler(IWarehouseMasterQueryRepository warehouseMasterQueryRepository, IMapper mapper, IMediator mediator, IMiscMasterGrpcClient miscMasterGrpcClient)
        {
            _iWarehouseMasterQueryRepository = warehouseMasterQueryRepository;
            _mediator = mediator;
            _mapper = mapper;
           _miscMasterGrpcClient = miscMasterGrpcClient;
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

            var miscMasters = await _miscMasterGrpcClient.GetMiscMasterByIdAsync("WarehouseType");
            var dict = miscMasters.ToDictionary(x => x.Id, x => x.Description);

            foreach (var data in warehouseListdto)
            {
                if (dict.TryGetValue(data.WarehouseTypeId, out var warehouseTypeName) && warehouseTypeName != null)
                {
                    data.WarehouseTypeName = warehouseTypeName;
                }

                if (dict.TryGetValue(data.StorageTypeId, out var storageTypeName) && storageTypeName != null)
                {
                    data.StorageTypeName = storageTypeName;
                    //data.WarehouseTypeName = dict.GetValueOrDefault(data.WarehouseTypeId,))
                }               


            }
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