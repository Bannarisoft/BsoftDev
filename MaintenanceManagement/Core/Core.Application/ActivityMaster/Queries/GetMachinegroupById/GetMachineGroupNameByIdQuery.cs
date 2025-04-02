using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.ActivityMaster.Queries.GetMachineGroupById
{
    public class GetMachineGroupNameByIdQuery   :   IRequest<ApiResponseDTO<List<GetMachineGroupNameByIdDto>>>
    {
       public int   ActivityId { get; set; }    
        
    }
}