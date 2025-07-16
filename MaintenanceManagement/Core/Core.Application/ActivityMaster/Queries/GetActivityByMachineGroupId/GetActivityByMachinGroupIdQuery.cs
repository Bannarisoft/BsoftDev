using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.ActivityMaster.Queries.GetActivityByMachinGroupId
{
    public class GetActivityByMachinGroupIdQuery :IRequest<List<GetActivityByMachineGroupDto>>
    {

     public int MachineGroupId { get; set; }
        
    }
}