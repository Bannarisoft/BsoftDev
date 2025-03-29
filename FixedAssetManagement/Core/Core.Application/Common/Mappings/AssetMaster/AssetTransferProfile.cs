using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.AssetMaster.AssetTransferIssue.Command.CreateAssetTransferIssue;
using Core.Application.AssetMaster.AssetTransferIssue.Command.UpdateAssetTransferIssue;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssertByCategory;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetCategoryByDeptId;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings.AssetMaster
{
    public class AssetTransferProfile : Profile
    
    {
        public AssetTransferProfile()
        {

            CreateMap<Core.Domain.Entities.AssetMaster.AssetTransferIssue, AssetTransferDto>(); 

            CreateMap<AssetTransferIssueHdrDto, Core.Domain.Entities.AssetMaster.AssetTransferIssueHdr>()
             .ForMember(dest => dest.AssetTransferIssueDtl, opt => opt.MapFrom(src => src.AssetTransferIssueDtls));
            CreateMap<AssetTransferIssueDtlDto, Core.Domain.Entities.AssetMaster.AssetTransferIssueDtl>();
            



             // ✅ Ensure mapping from AssetTransferJsonDto -> AssetTransferIssueHdr
            CreateMap<AssetTransferJsonDto, Core.Domain.Entities.AssetMaster.AssetTransferIssueHdr>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) ;// Ignore ID if not required
               // .ForMember(dest => dest.SomeOtherField, opt => opt.MapFrom(src => src.SomeSourceField)); // Customize field mappings as needed

            // ✅ Ensure mapping from UpdateAssetTransferIssueCommand -> AssetTransferIssueHdr
            CreateMap<UpdateAssetTransferHdrDto, Core.Domain.Entities.AssetMaster.AssetTransferIssueHdr>()
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore()) // Ignore modified date if set manually
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.AssetTransferIssueDtl, opt => opt.MapFrom(src => src.AssetTransferIssueDtl));

            CreateMap<UpdateAssetTransferDtlDto, Core.Domain.Entities.AssetMaster.AssetTransferIssueDtl>();

            CreateMap< AssetMasterDto , GetAssetMasterDto>();

            CreateMap<GetAssetMasterDto, GetCategoryByDeptIdDto>(); 


          

        }
        
    }
}