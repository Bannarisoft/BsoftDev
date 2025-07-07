using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.MachineSpecification.Command
{
    public class MachineSpecificationDto
    {
        public int Id { get; set; }
        public int SpecificationId { get; set; }
        public string? SpecificationName { get; set; }
        public int MachineId { get; set; }
        public Status IsActive { get; set; }
    }
}