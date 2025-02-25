using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetAdditionalCost.Commands.CreateAssetAdditionalCost
{
    public class CreateAssetAdditionalCostCommand :IRequest<ApiResponseDTO<int>> 
    {
        public int AssetId { get; set; }   
        public int AssetSourceId { get; set; }   
        public decimal Amount { get; set; }
        public string? JournalNo { get; set; }
        public int? CostType { get; set; } 
    }
}