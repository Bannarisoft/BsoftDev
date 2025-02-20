using Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecification;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetSpecification.Commands.DeleteAssetSpecification
{ 
    public class DeleteAssetSpecificationCommand :  IRequest<ApiResponseDTO<AssetSpecificationDTO>>  
    {
         public int Id { get; set; }    
    }
}