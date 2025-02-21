using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecification;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecificationAutoComplete
{
    public class GetAssetSpecificationAutoCompleteQuery : IRequest<ApiResponseDTO<List<AssetSpecificationAutoCompleteDTO>>> 
    {
        public string? SearchPattern { get; set; }
    }
}