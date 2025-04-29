using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Application.MaintenanceRequest.Command.CreateMaintenanceRequest;
using Core.Application.MaintenanceRequest.Command.UpdateMaintenanceRequestCommand;
using Core.Application.MaintenanceRequest.Queries.GetExistingVendorDetails;
using Core.Application.MaintenanceRequest.Queries.GetExternalRequestById;
using Core.Application.MaintenanceRequest.Queries.GetMaintenanceRequest;

namespace Core.Application.Common.Mappings
{
    public class MaintenanceRequestProfile : Profile
    {
        
        private readonly IIPAddressService _ipAddressService;

        public MaintenanceRequestProfile()
        {
            CreateMap<CreateMaintenanceRequestCommand, Core.Domain.Entities.MaintenanceRequest>();
            CreateMap<Core.Domain.Entities.MaintenanceRequest,GetMaintenanceRequestDto>();
            // CreateMap<Core.Domain.Entities.MaintenanceRequest,GetMaintenanceRequestDto>()
            // .ForMember(dest => dest.RequestTypeName,opt => opt.MapFrom(src => src.MiscRequestType.Code))
            // .ForMember(dest => dest.MaintenanceTypeName,opt => opt.MapFrom(src => src.MiscMaintenanceType.Code))
            // .ForMember(dest => dest.MachineName,opt => opt.MapFrom(src => src.Machine.MachineName)) ;

           CreateMap<UpdateMaintenanceRequestCommand, Core.Domain.Entities.MaintenanceRequest>();

            CreateMap<Core.Domain.Entities.ExistingVendorDetails, GetExistingVendorDetailsDto>();
            CreateMap<Core.Domain.Entities.MaintenanceRequest, Core.Domain.Entities.WorkOrderMaster.WorkOrder>()
             .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.RequestId, opt => opt.MapFrom(src => src.Id))
            // .ForMember(dest => dest.WorkOrderDocNo, opt => opt.MapFrom(src => _workOrderQueryRepository.GetLatestWorkOrderDocNo(src.RequestTypeId)))
            .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.RequestStatusId))
            .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => _ipAddressService.GetCompanyId()))
            .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => _ipAddressService.GetUnitId()));


            CreateMap<GetExternalRequestByIdDto,Core.Domain.Entities.WorkOrderMaster.WorkOrder>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.RequestId,  opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.RequestStatusId))
            .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
            .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.UnitId));
            
        }
    }
}