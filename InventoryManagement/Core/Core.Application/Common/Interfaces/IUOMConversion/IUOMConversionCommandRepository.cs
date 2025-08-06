using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IUOMConversion
{
    public interface IUOMConversionCommandRepository
    {

        Task<Core.Domain.Entities.UOMConversion> CreateAsync(Core.Domain.Entities.UOMConversion uOMConversion);     

        Task<Core.Domain.Entities.UOMConversion?> UpdateAsync(int id, Core.Domain.Entities.UOMConversion uOMConversion);
         
        Task<bool> DeleteAsync(int id,Core.Domain.Entities.UOMConversion uOMConversion);  

    }
}