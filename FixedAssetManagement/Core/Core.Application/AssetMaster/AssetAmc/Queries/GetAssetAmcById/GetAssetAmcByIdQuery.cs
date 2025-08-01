using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetAmc.Queries.GetAssetAmc;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetAmc.Queries.GetAssetAmcById
{
    public class GetAssetAmcByIdQuery : IRequest<AssetAmcDto>
    {
        public int Id {get; set;}
    }
}