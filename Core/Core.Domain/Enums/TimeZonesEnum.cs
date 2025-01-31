using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Domain.Enums
{
    public class TimeZonesEnum
    {
         public enum TimeZonesStatus
        {
            Inactive = 0,
            Active  = 1
        }
        public enum TimeZonesDelete
        {
            NotDeleted = 0,
            Deleted = 1
        }
    }
}