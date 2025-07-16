using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetGroup.Command.UpdateAssetGroup
{
    public class UpdateAssetGroupCommand : IRequest<ApiResponseDTO<int>>
    {
        public int Id { get; set; }
        public string? GroupName { get; set; }
        public int SortOrder { get; set; }
        public byte IsActive { get; set; }
        public decimal? GroupPercentage { get; set; }
    }
}