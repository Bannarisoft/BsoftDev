using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Location.Queries.GetLocations
{
    public class LocationAutoCompleteDto
    {
        public int Id { get; set; }
        public string? LocationName { get; set; }
    }
}