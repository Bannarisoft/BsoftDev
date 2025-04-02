using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.ActivityMaster.Command.UpdateActivityMster
{
    public class UpdateActivityMasterCommand  : IRequest<ApiResponseDTO<int>> 
    {

     public UpdateActivityMasterDto? UpdateActivityMaster  { get; set; }
    }
}