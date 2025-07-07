using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.MachineGroup.Command.UpdateMachineGroup
{
    public class UpdateMachineGroupCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string? GroupName { get; set; }  
        public int Manufacturer  { get; set;}
        public int UnitId { get; set; }
        public int DepartmentId { get; set; }
        public byte IsActive { get; set; }
        
    }
}