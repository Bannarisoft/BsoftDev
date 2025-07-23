using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts.Dtos.Workflow
{
    public class ApprovalRequestDto
    {
        public string ModuleTypeName { get; set; }
        public int ModuleTransactionId { get; set; }
    }
}