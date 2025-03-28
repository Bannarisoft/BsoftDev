using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.ActivityMaster.Queries.GetAllActivityMaster;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.ActivityMaster.Command.CreateActivityMaster
{
    public class CreateActivityMasterCommand : IRequest<ApiResponseDTO<GetAllActivityMasterDto>>
    {
        public string? ActivityName { get; set;}
        public string? Description { get; set; }
        public int DepartmentId { get; set; }       
        public int MachineGroupId { get; set; }        
        public int EstimatedDuration { get; set; }
        public int ActivityType { get; set; }
    }
}