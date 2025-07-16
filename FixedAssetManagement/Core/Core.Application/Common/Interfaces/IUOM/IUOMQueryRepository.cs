using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.UOM.Queries.GetUOMTypeAutoComplete;

namespace Core.Application.Common.Interfaces.IUOM
{
    public interface IUOMQueryRepository
    {
        Task<(List<Core.Domain.Entities.UOM>,int)> GetAllUOMAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<Core.Domain.Entities.UOM> GetByIdAsync(int id);
        Task<List<Core.Domain.Entities.UOM>> GetUOM(string searchPattern);
        Task<List<UOMTypeAutoCompleteDto>> GetUOMType(string searchPattern);

        Task<Core.Domain.Entities.UOM?> GetByUOMNameAsync(string name,int? id = null);
        
    }
}