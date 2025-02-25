using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.CreateAssetComposite
{
    public class CreateAssetCompositeCommand : IRequest<ApiResponseDTO<AssetMasterGeneralDTO>>
    {
        public AssetCompositeDto AssetComposite { get; set; } = null!;
    }
}