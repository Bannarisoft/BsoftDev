using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.ExcelImport.PhysicalStockVerification;
using Core.Domain.Entities.AssetMaster;

namespace Core.Application.Common.Mappings.AssetMaster
{
    public class AssetAuditProfile : Profile
    {
        public AssetAuditProfile()
        {
            CreateMap<AssetAuditDto, AssetAudit>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
  
        }
    }
}