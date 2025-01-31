using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Domain.Enums
{
    public class CurrencyEnum
    {
        public enum CurrencyStatus
        {
            Inactive = 0,
            Active  = 1
        }
        public enum CurrencyDelete
        {
            NotDeleted = 0,
            Deleted = 1
        }
    }
}