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
            CreateMap<Core.Domain.Entities.MaintenanceRequest,GetMaintenanceRequestDto>();
            // CreateMap<Core.Domain.Entities.MaintenanceRequest,GetMaintenanceRequestDto>()
            // .ForMember(dest => dest.RequestTypeName,opt => opt.MapFrom(src => src.MiscRequestType.Code))
            // .ForMember(dest => dest.MaintenanceTypeName,opt => opt.MapFrom(src => src.MiscMaintenanceType.Code))
            // .ForMember(dest => dest.MachineName,opt => opt.MapFrom(src => src.Machine.MachineName)) ;

           CreateMap<UpdateMaintenanceRequestCommand, Core.Domain.Entities.MaintenanceRequest>();

            CreateMap<Core.Domain.Entities.ExistingVendorDetails, GetExistingVendorDetailsDto>();
          

           
            
        }
    }
}