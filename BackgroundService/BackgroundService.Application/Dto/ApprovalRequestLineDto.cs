using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundService.Application.Dto
{
    public class ApprovalRequestLineDto
    {
        public int ModuleLineTransactionId { get; set; }
        public string Status { get; set; }
    }
}