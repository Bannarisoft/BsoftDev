using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetGroup.Command.CreateAssetGroup
{
    public class CreateAssetGroupCommand :IRequest<ApiResponseDTO<int>> 
    {
        public string? Code { get; set; }
        public string? GroupName { get; set; }
       
    }
}