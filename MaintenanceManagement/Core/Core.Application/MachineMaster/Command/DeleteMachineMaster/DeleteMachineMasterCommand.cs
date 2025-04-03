using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MachineMaster.Command.DeleteMachineMaster
{
    public class DeleteMachineMasterCommand : IRequest<ApiResponseDTO<int>> 
    {
        public int Id { get; set; } 
    }
}