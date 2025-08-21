using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts.Dtos.Workflow
{
    public class ApprovalRequestLineStatusDto
    {
        public int Id { get; set; }
        public int ModuleLineTransactionId { get; set; }
        public string Status { get; set; }
        public string ApproverBinding { get; set; }
        public string ApproverValue { get; set; }
    }
}