using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MachineMaster.Queries.GetMachineDepartmentbyId
{
    public class GetMachineDepartmentbyIdQuery : IRequest<ApiResponseDTO<MachineDepartmentGroupDto>>
    {
        public int MachineGroupId { get; set; }
    }
}