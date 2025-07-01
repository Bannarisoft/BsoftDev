using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.MachineGroup.Queries.GetMachineGroup;
using MediatR;

namespace Core.Application.MachineGroup.Command.CreateMachineGroup
{
    public class CreateMachineGroupCommand : IRequest<ApiResponseDTO<MachineGroupDto>>
    {
        public string? GroupName { get; set; }
        public int Manufacturer { get; set; } 
        public int UnitId { get; set; }
        public int DepartmentId { get; set; }
    }
}