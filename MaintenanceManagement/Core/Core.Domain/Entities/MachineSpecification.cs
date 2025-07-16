using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class MachineSpecification : BaseEntity
    {
        public int SpecificationId { get; set; }
        public MiscMaster SpecificationIdMachineSpec { get; set; } = null!;
        public string? SpecificationValue { get; set; }
        public int MachineId { get; set; }
        public MachineMaster? MachineMaster { get; set; }
        
        
    }
}