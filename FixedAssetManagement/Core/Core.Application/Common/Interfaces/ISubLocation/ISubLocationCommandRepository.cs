using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.ISubLocation
{
    public interface ISubLocationCommandRepository
    {
        Task<Core.Domain.Entities.SubLocation> CreateAsync(Core.Domain.Entities.SubLocation sublocation);     
        Task<bool> UpdateAsync(Core.Domain.Entities.SubLocation sublocation);
        Task<bool> DeleteAsync(int id,Core.Domain.Entities.SubLocation sublocation); 
    }
}