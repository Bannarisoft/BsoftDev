using AutoMapper;
using Core.Application.AssetMaster.AssetTranferIssueApproval.Commands.UpdateAssetTranferIssueApproval;
using Core.Application.AssetMaster.AssetTranferIssueApproval.Queries.GetAssetTransferIssueApproval;
using Core.Application.AssetMaster.AssetTranferIssueApproval.Queries.GetAssetTransferIssueById;

namespace Core.Application.Common.Mappings.AssetMaster
{
    public class AssetIssueTransferApproval : Profile
    {
        public AssetIssueTransferApproval()
        {
           CreateMap<Core.Domain.Entities.AssetMaster.AssetTransferIssueApproval, AssetTransferIssueApprovalDto>();
           CreateMap<Core.Domain.Entities.AssetMaster.AssetTransferIssueApproval, AssetTransferIssueByIdDto>();
           CreateMap<UpdateAssetTranferIssueApprovalCommand, Core.Domain.Entities.AssetMaster.AssetTransferIssueHdr>()
           .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
           .ForMember(dest => dest.Id, opt => opt.Ignore()); // Ignore Id to prevent mapping issues 
      
        }
    }
}