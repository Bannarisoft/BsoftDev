using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IMaintenanceRequest;
using Core.Application.Common.Interfaces.IWorkOrder;
using MediatR;

namespace Core.Application.MaintenanceRequest.Command.CreateExternalRequestWorkOrder
{
    public class CreateExternalRequestWorkOrderCommandHandler : IRequestHandler<CreateExternalRequestWorkOrderCommand, ApiResponseDTO<List<int>>>
    {

        
       private readonly IMaintenanceRequestCommandRepository  _maintenanceRequestCommandRepository;
       private readonly IMapper _imapper;
       private readonly IMediator _mediator;
       private readonly IMaintenanceRequestQueryRepository  _maintenanceRequestQueryRepository;
       private readonly IWorkOrderCommandRepository _workOrderCommandRepository;
         private readonly IWorkOrderQueryRepository _workOrderQueryRepository;
        private readonly IIPAddressService _ipAddressService;
        public CreateExternalRequestWorkOrderCommandHandler(IMaintenanceRequestCommandRepository maintenanceRequestCommandRepository,IMapper mapper,IMediator mediator,IMaintenanceRequestQueryRepository maintenanceRequestQueryRepository,IWorkOrderCommandRepository workOrderCommandRepository,IWorkOrderQueryRepository workOrderQueryQueryRepository,IIPAddressService ipAddressService)
        {
             _maintenanceRequestCommandRepository = maintenanceRequestCommandRepository;
             _imapper = mapper;
             _mediator = mediator;
             _maintenanceRequestQueryRepository = maintenanceRequestQueryRepository;
             _workOrderCommandRepository = workOrderCommandRepository;
             _workOrderQueryRepository = workOrderQueryQueryRepository;
             _ipAddressService = ipAddressService;
        }

                public async Task<ApiResponseDTO<List<int>>> Handle(CreateExternalRequestWorkOrderCommand request, CancellationToken cancellationToken)
            {
                // var createdIds = new List<int>();             
                //             var requestIds = request.Ids
                // .Select(id => id)
                // .ToList();                

                // if (!requestIds.Any())
                // {
                //     return CreateErrorResponse("No valid IDs found.");
                // }

                // var companyId = _ipAddressService.GetCompanyId();
                // var unitId = _ipAddressService.GetUnitId();

                // Fetch external requests
                var externalRequests = await _maintenanceRequestQueryRepository.GetExternalRequestByIdAsync(request.Ids);

                // Fetch the Misc Open Status once
                var statusList = await _maintenanceRequestQueryRepository.GetMaintenanceOpenstatusAsync();
                var openStatus = statusList.FirstOrDefault();
                
                if (openStatus == null)
                {
                    return CreateErrorResponse("Open status not found in MiscMaster.");
                }

                // Process each external request
                foreach (var externalRequest in externalRequests)
                {
                    var docNo = await _workOrderQueryRepository.GetLatestWorkOrderDocNo(externalRequest.MaintenanceTypeId);

                    var workOrder = new Core.Domain.Entities.WorkOrderMaster.WorkOrder
                    {
                        
                        WorkOrderDocNo = docNo,
                        RequestId = externalRequest.Id,
                        StatusId = openStatus.Id,
                        MiscStatus = openStatus,
                        CompanyId = externalRequest.CompanyId,
                        UnitId = externalRequest.UnitId
                    };

                    var result = await _workOrderCommandRepository.CreateAsync(workOrder, cancellationToken);
                    // if (result?.Id > 0)
                    // {
                    //     createdIds.Add(result.Id);
                    // }
                }

                return new ApiResponseDTO<List<int>>
                {
                    IsSuccess = true,
                    Message = $" Work Order(s) created successfully from external requests"
                };
            }

            // Utility method to simplify error response creation
            private ApiResponseDTO<List<int>> CreateErrorResponse(string message)
            {
                return new ApiResponseDTO<List<int>>
                {
                    IsSuccess = false,
                    Message = message,
                    Data = new List<int>()
                };
            }


        //  public async Task<ApiResponseDTO<List<int>>> Handle(CreateExternalRequestWorkOrderCommand request, CancellationToken cancellationToken)
        // {
        //     var createdIds = new List<int>();
        //     var companyId = _ipAddressService.GetCompanyId();
        //     var unitId = _ipAddressService.GetUnitId();

        //     // Fetch single maintenance request by ID
        //     var externalRequest = await _maintenanceRequestQueryRepository.GetByIdAsync(request.Id);
        //     if (externalRequest == null)
        //     {
        //         return new ApiResponseDTO<List<int>>
        //         {
        //             IsSuccess = false,
        //             Message = $"No maintenance request found for ID {request.Id}",
        //             Data = createdIds
        //         };
        //     }

        //     // Get new WorkOrderDocNo
        //     var docNo = await _workOrderQueryRepository.GetLatestWorkOrderDocNo(externalRequest.MaintenanceTypeId);

        //     // Get Misc Open Status
        //     var statusList = await _maintenanceRequestQueryRepository.GetMaintenanceOpenstatusAsync();
        //     var openStatus = statusList.FirstOrDefault();

        //     if (openStatus == null)
        //     {
        //         return new ApiResponseDTO<List<int>>
        //         {
        //             IsSuccess = false,
        //             Message = "Open status not found in MiscMaster.",
        //             Data = createdIds
        //         };
        //     }

        //     var workOrder = new  Core.Domain.Entities.WorkOrderMaster.WorkOrder
        //     {
        //         Id = 0,
        //         WorkOrderDocNo = docNo,
        //         RequestId = externalRequest.Id,
        //         StatusId = openStatus.Id,
        //         MiscStatus = openStatus,
        //         CompanyId = companyId,
        //         UnitId = unitId
        //     };

        //     var result = await _workOrderCommandRepository.CreateAsync(workOrder, cancellationToken);
        //     if (result?.Id > 0)
        //     {
        //         createdIds.Add(result.Id);
        //     }

        //     return new ApiResponseDTO<List<int>>
        //     {
        //         IsSuccess = true,
        //         Message = "Work Order created successfully.",
        //         Data = createdIds
        //     };
        // }
    //    public async Task<ApiResponseDTO<List<int>>> Handle(CreateExternalRequestWorkOrderCommand request, CancellationToken cancellationToken)
    //     {
    //         var createdIds = new List<int>();

    //         var companyId = _ipAddressService.GetCompanyId();
    //         var unitId = _ipAddressService.GetUnitId();

    //         // 🔹 Step 1: Fetch all external requests by IDs
    //         var externalRequests = await _maintenanceRequestQueryRepository.GetByIdAsync(request.Id); // Ensure your request model has `List<int> Ids`

    //         foreach (var externalRequest in externalRequests)
    //         {
    //             var docNo = await _workOrderQueryRepository.GetLatestWorkOrderDocNo(externalRequest.MaintenanceTypeId);

    //             // Fetch the open status from the Misc status table
    //             var statusList = await _maintenanceRequestQueryRepository.GetMaintenanceOpenstatusAsync();
    //             var openStatus = statusList.FirstOrDefault();

    //             // Ensure MiscStatus is set to a valid MiscMaster object
    //             var miscStatus = new Core.Domain.Entities.MiscMaster
    //             {
    //                 Id = openStatus?.Id ?? 0,          // Set the appropriate Id
    //                 Description = openStatus?.Description ?? string.Empty // Set the Description (or any other property)
    //             };

    //             var workOrder = new Core.Domain.Entities.WorkOrderMaster.WorkOrder
    //             {
    //                 Id = 0,
    //                 WorkOrderDocNo = docNo,
    //                 RequestId = externalRequest.Id,
    //                 StatusId = openStatus?.Id ?? 0,  // Default to 0 if null
    //                 MiscStatus = miscStatus,  // Assign the valid MiscMaster object
    //                 CompanyId = companyId,
    //                 UnitId = unitId
    //             };

    //             var result = await _workOrderCommandRepository.CreateAsync(workOrder, cancellationToken);
    //             if (result?.Id > 0) // Make sure `CreateAsync` returns the WorkOrder object with `Id` set
    //             {
    //                 createdIds.Add(result.Id);
    //             }
    //         }

    //         return new ApiResponseDTO<List<int>>
    //         {
    //             IsSuccess = true,
    //             Message = $"{createdIds.Count} Work Order(s) created successfully from external requests",
    //             Data = createdIds
    //         };
    //     }

    //   public CreateExternalRequestWorkOrderCommandHandler( IMaintenanceRequestCommandRepository maintenanceRequestCommandRepository, IMapper imapper, IMediator mediator, IMaintenanceRequestQueryRepository maintenanceRequestQueryRepository, IWorkOrderCommandRepository workOrderCommandRepository , IWorkOrderQueryRepository workOrderQueryQueryRepository , IIPAddressService ipAddressService )
    //    {
    //        _maintenanceRequestCommandRepository = maintenanceRequestCommandRepository;
    //        _imapper = imapper;
    //        _mediator = mediator;
    //        _maintenanceRequestQueryRepository = maintenanceRequestQueryRepository;
    //        _workOrderCommandRepository = workOrderCommandRepository;
    //        _workOrderQueryRepository = workOrderQueryQueryRepository;
    //        _ipAddressService = ipAddressService;
    //    }

    //         public async Task<ApiResponseDTO<List<int>>> Handle(CreateExternalRequestWorkOrderCommand request, CancellationToken cancellationToken)
    //     {
    //         var createdIds = new List<int>();

    //         var companyId = _ipAddressService.GetCompanyId();
    //         var unitId = _ipAddressService.GetUnitId();

    //         // 🔹 Step 1: Fetch all external requests by IDs
    //         var externalRequests = await _maintenanceRequestQueryRepository.GetByIdAsync(request.Id); // Ensure your request model has `List<int> Ids`

    //         // 🔹 Step 2: Fetch the WorkOrder open status from Misc
        

    //         foreach (var externalRequest in externalRequests)
    //         {
             

    //             var docNo = await _workOrderQueryRepository.GetLatestWorkOrderDocNo(externalRequest.MaintenanceTypeId);

    //              var statusList = await _maintenanceRequestQueryRepository.GetMaintenanceOpenstatusAsync();
    //             var openStatus = statusList.FirstOrDefault();

    //             var workOrder = new Core.Domain.Entities.WorkOrderMaster.WorkOrder
    //             {
    //                 Id = 0,
    //                 WorkOrderDocNo = docNo,
    //                 RequestId = externalRequest.Id,                  
    //                 StatusId = externalRequest.StatusId, // required field                                   
    //                 CompanyId = companyId,
    //                 UnitId = unitId
    //             };

    //             var result = await _workOrderCommandRepository.CreateAsync(workOrder, cancellationToken);
    //             if (result?.Id > 0)
    //                 {
    //                     createdIds.Add(result.Id);
    //                 }
    //         }

    //         return new ApiResponseDTO<List<int>>
    //         {
    //             IsSuccess = true,
    //             Message = $"{createdIds.Count} Work Order(s) created successfully from external requests",
    //             Data = createdIds
    //         };
    //     }
                    
        
    }
}