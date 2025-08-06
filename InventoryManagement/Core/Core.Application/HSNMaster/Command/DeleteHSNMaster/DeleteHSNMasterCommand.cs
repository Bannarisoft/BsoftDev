using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.HSNMaster.Command.DeleteHSNMaster
{
    public class DeleteHSNMasterCommand: IRequest<ApiResponseDTO<bool>>
    {
        public int Id { get; set; }
        
    }
}