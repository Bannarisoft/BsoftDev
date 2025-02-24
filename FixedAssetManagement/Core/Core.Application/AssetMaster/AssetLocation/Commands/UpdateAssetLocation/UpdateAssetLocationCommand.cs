using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetLocation.Queries.GetAssetLocation;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetLocation.Commands.UpdateAssetLocation
{
    public class UpdateAssetLocationCommand :   IRequest<ApiResponseDTO<AssetLocationDto>>  
    {

        public int AssetId { get; set; }
         public int UnitId { get; set; } 
        public int DepartmentId { get; set; }
        public int LocationId { get; set; }
        public int SubLocationId { get; set; } 
        public int CustodianId { get; set; }
        public int UserID { get; set; }     
    }
}