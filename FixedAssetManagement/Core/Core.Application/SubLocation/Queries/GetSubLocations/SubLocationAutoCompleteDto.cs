using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Location.Queries.GetSubLocations
{
    public class SubLocationAutoCompleteDto
    {
        public int Id { get; set; }
        public string? SubLocationName { get; set; }
    }
}