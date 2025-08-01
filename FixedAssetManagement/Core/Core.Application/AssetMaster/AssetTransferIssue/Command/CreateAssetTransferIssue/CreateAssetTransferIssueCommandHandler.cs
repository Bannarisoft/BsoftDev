using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Events.Workflow;
using Contracts.Interfaces.External.IUser;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetTransferIssue;
using Core.Domain.Entities.AssetMaster;
using Core.Domain.Events;
using FluentValidation;
using MediatR;
using static Core.Domain.Common.MiscEnumEntity;

namespace Core.Application.AssetMaster.AssetTransferIssue.Command.CreateAssetTransferIssue
{
    public class CreateAssetTransferIssueCommandHandler : IRequestHandler<CreateAssetTransferIssueCommand,int>
    {
       private readonly  IAssetTransferCommandRepository _assetTransferCommandRepository;
        private readonly IMapper _mapper;  
        private readonly IMediator _Imediator;
         private readonly IIPAddressService _ipAddressService;
        private readonly ITimeZoneService _timeZoneService; 
        private readonly IValidator<CreateAssetTransferIssueCommand> _validator; 
      
        private readonly IEventPublisher _eventPublisher;
        private readonly IUnitGrpcClient _unitGrpcClient;
        private readonly IDepartmentAllGrpcClient _departmentAllGrpcClient;
        public CreateAssetTransferIssueCommandHandler(IAssetTransferCommandRepository assetTransferCommandRepository, IMapper mapper,
         IMediator Imediator, IIPAddressService ipAddressService, ITimeZoneService timeZoneService,
         IValidator<CreateAssetTransferIssueCommand> validator, IEventPublisher eventPublisher, IUnitGrpcClient unitGrpcClient,
         IDepartmentAllGrpcClient departmentAllGrpcClient)
        {
            _assetTransferCommandRepository = assetTransferCommandRepository;
            _mapper = mapper;
            _Imediator = Imediator;
            _ipAddressService = ipAddressService;
            _timeZoneService = timeZoneService;
            _validator = validator;
            _eventPublisher = eventPublisher;
            _unitGrpcClient = unitGrpcClient;
            _departmentAllGrpcClient = departmentAllGrpcClient;

        }
     public async Task<int> Handle(CreateAssetTransferIssueCommand request, CancellationToken cancellationToken)
        {
                
            string currentIp = _ipAddressService.GetSystemIPAddress();
            int userId = _ipAddressService.GetUserId(); 
            string username = _ipAddressService.GetUserName();
            var systemTimeZoneId = _timeZoneService.GetSystemTimeZone();
            var currentTime = _timeZoneService.GetCurrentTime(systemTimeZoneId); 
            // 🔹 Map Command to Entity
             var assetTransferIssueHdr = _mapper.Map<Core.Domain.Entities.AssetMaster.AssetTransferIssueHdr>(request.AssetTransferIssueHdrDto); 
             assetTransferIssueHdr.CreatedIP = currentIp;
             assetTransferIssueHdr.CreatedDate = currentTime;
             assetTransferIssueHdr.CreatedBy = userId;
             assetTransferIssueHdr.CreatedByName = username;
              var result =  await _assetTransferCommandRepository.CreateAssetTransferAsync(assetTransferIssueHdr);

            

              //Domain Event
                  var domainEvent = new AuditLogsDomainEvent(
                      actionDetail: "Create",
                      actionCode: assetTransferIssueHdr.Id.ToString(),
                      actionName: "Asset Transfer",
                      details: $"Asset Transfer '{assetTransferIssueHdr.Id}' was created. ",
                      module:"Asset Transfer"
                  );     

                  await _Imediator.Publish(domainEvent, cancellationToken);
                var assetTransferApprovalReqDto =  _mapper.Map<AssetTransferApprovalRequestDto>(request.AssetTransferIssueHdrDto); 
                      var units = await _unitGrpcClient.GetAllUnitAsync();
                      var departments = await _departmentAllGrpcClient.GetDepartmentAllAsync();

                     var unitDict = units.ToDictionary(u => u.UnitId, u => u.UnitName);
                     var departDict = departments.ToDictionary(u => u.DepartmentId, u => u.DepartmentName);

           
                    if (unitDict.TryGetValue(assetTransferIssueHdr.FromUnitId, out var FromUnitName))
                   {
                       assetTransferApprovalReqDto.FromUnitName = FromUnitName;
                   }
                     if (unitDict.TryGetValue(assetTransferIssueHdr.ToUnitId, out var ToUnitName))
                     {
                         assetTransferApprovalReqDto.ToUnitName = ToUnitName;
                     }

                         if (departDict.TryGetValue(assetTransferIssueHdr.FromDepartmentId, out var FromDepartmentName))
                   {
                       assetTransferApprovalReqDto.FromDepartmentName = FromDepartmentName;
                   }
                     if (departDict.TryGetValue(assetTransferIssueHdr.ToDepartmentId, out var ToDepartmentName))
                     {
                         assetTransferApprovalReqDto.ToDepartmentName = ToDepartmentName;
                     }
               
              string serializedPayload = JsonSerializer.Serialize(assetTransferApprovalReqDto, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                });
            
                  if (result > 0)
            {
                var correlationId = Guid.NewGuid();
                var @event = new TransactionCreatedEvent
                {
                    CorrelationId = correlationId,
                    ModuleTypeName = ApprovalRequest.AssetTransfer,
                    ModuleTransactionId = result,
                    UnitId = request.AssetTransferIssueHdrDto.FromUnitId,
                    DepartmentId = request.AssetTransferIssueHdrDto.FromDepartmentId,
                    Payload = serializedPayload
                };

                await _eventPublisher.SaveEventAsync(@event);
                await _eventPublisher.PublishPendingEventsAsync();

                return result;
            }
                 throw new Exception("Asset Transfer not created");
        }
        
    }
}