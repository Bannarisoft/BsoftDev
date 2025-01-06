using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces
{
    public interface IJwtTokenHelper
    {
        string GenerateToken(string username, IEnumerable<string> roles);
    }
}