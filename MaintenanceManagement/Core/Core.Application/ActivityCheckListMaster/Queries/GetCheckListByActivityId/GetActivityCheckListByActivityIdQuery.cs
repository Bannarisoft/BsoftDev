using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.ActivityCheckListMaster.Queries.GetCheckListByActivityId
{
    public class GetActivityCheckListByActivityIdQuery : IRequest<List<GetActivityCheckListByActivityIdDto>>
    {
        public List<int>? Ids { get; set; }
        public int? WorkOrderId { get; set; }  
    }
}