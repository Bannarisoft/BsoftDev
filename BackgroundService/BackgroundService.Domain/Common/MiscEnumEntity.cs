using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundService.Domain.Common
{
    public static class MiscEnumEntity
    {
        public static class GetStatusPending
        {
            public const string Status = "Pending";
        }
         public static class GetApprovalStatus
        {
            public const string Status = "ApprovalStatus";
        }
    }
}