using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.MaintenanceRequest.Command.CreateMaintenanceRequest;
using Core.Application.MaintenanceRequest.Command.UpdateMaintenanceRequestCommand;
using Core.Application.MaintenanceRequest.Queries.GetExistingVendorDetails;
using Core.Application.MaintenanceRequest.Queries.GetMaintenanceRequest;

namespace Core.Application.Common.Mappings
{
    public class MaintenanceRequestProfile : Profile
    {
        

        public MaintenanceRequestProfile()
        {
            CreateMap<CreateMaintenanceRequestCommand, Core.Domain.Entities.MaintenanceRequest>();
            
            CreateMap<Core.Domain.Entities.MaintenanceRequest,GetMaintenanceRequestDto>()
            .ForMember(dest => dest.RequestTypeName,opt => opt.MapFrom(src => src.MiscRequestType.Code))
            .ForMember(dest => dest.MaintenanceTypeName,opt => opt.MapFrom(src => src.MiscMaintenanceType.Code))
            .ForMember(dest => dest.MachineName,opt => opt.MapFrom(src => src.Machine.MachineName));
           
           CreateMap<UpdateMaintenanceRequestCommand, Core.Domain.Entities.MaintenanceRequest>()
            .ForMember(dest => dest.RequestTypeId, opt => opt.MapFrom(src => src.RequestTypeId))
            .ForMember(dest => dest.MaintenanceTypeId, opt => opt.MapFrom(src => src.MaintenanceTypeId))
            .ForMember(dest => dest.MachineId, opt => opt.MapFrom(src => src.MachineId))
            .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
            // .ForMember(dest => dest.SourceId, opt => opt.MapFrom(src => src.SourceId))
            // .ForMember(dest => dest.VendorId, opt => opt.MapFrom(src => src.VendorId))
            // .ForMember(dest => dest.OldVendorId, opt => opt.MapFrom(src => src.OldVendorId))
            .ForMember(dest => dest.Remarks, opt => opt.MapFrom(src => src.Remarks))
            .ForMember(dest => dest.RequestId, opt => opt.MapFrom(src => src.RequestId));

            CreateMap<Core.Domain.Entities.ExistingVendorDetails, GetExistingVendorDetailsDto>();
          

           
            
        }
    }
}