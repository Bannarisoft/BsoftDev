using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Domain.Enums
{
    public class LocationEnum
    {
        public enum CountryStatus
        {
            Inactive = 0,
            Active = 1
        }

        public enum StateStatus
        {
            Inactive = 0,
            Active = 1
        }

        public enum CityStatus
        {
            Inactive = 0,
            Active = 1
        }

        public enum CountryDelete
        {
            NotDeleted = 0,
            Deleted = 1
        }

        public enum StateDelete
        {
            NotDeleted = 0,
            Deleted = 1
        }

        public enum CityDelete
        {
            NotDeleted = 0,
            Deleted = 1
        }
    }
}