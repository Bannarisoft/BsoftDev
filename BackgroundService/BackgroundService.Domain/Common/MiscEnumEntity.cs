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
        public static class GetStatusApproved
        {
            public const string Status = "Approved";
        }
        public static class GetStatusRejected
        {
            public const string Status = "Rejected";
        }
    }
}