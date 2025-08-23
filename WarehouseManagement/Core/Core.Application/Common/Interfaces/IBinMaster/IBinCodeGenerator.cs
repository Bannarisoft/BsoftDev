using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IBinMaster
{
    public interface IBinCodeGenerator
    {
        Task<string> GenerateAsync(int warehouseId, int? rackId, CancellationToken ct = default);
         
        
    }
}