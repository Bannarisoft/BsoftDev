using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.HSNMaster.Command.CreateHSNMaster;
using Core.Application.HSNMaster.Command.DeleteHSNMaster;
using Core.Application.HSNMaster.Command.UpdateHSNMaster;
using Core.Application.HSNMaster.Queries.GetAllHSNMaster;

namespace Core.Application.Common.Mappings
{
    public class HSNMasterProfile : Profile
    {

        public HSNMasterProfile()
        {

            CreateMap<Core.Domain.Entities.HSNMaster, HSNMasterDto>();

            CreateMap<CreateHSNMasterCommand, Core.Domain.Entities.HSNMaster>();

            CreateMap<UpdateHSNMasterCommand, Core.Domain.Entities.HSNMaster>();

            CreateMap<DeleteHSNMasterCommand, Core.Domain.Entities.HSNMaster>();



        }
    }
}