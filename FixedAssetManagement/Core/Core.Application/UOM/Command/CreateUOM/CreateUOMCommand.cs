using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.UOM.Queries.GetUOMs;
using MediatR;

namespace Core.Application.UOM.Command.CreateUOM
{
    public class CreateUOMCommand : IRequest<ApiResponseDTO<UOMDto>>
    {
        public string? Code { get; set; }
        public string? UOMName { get; set; }
        public int SortOrder { get; set; }
        public int UOMTypeId { get; set; }
    }
}