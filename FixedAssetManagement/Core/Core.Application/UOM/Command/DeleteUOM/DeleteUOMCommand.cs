using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.UOM.Queries.GetUOMs;
using MediatR;

namespace Core.Application.UOM.Command.DeleteUOM
{
    public class DeleteUOMCommand : IRequest<ApiResponseDTO<UOMDto>>
    {
        public int Id { get; set; }
        
    }
}