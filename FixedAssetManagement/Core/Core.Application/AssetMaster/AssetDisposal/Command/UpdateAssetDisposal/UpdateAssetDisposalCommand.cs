using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetDisposal.Command.UpdateAssetDisposal
{
    public class UpdateAssetDisposalCommand :IRequest<ApiResponseDTO<int>> 
    {
        public int Id { get; set; } 
        public DateTimeOffset DisposalDate { get; set; }
        public int? DisposalType { get; set; }  
        public string? DisposalReason { get; set; }
        public decimal? DisposalAmount { get; set; }
    }
}