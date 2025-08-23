using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IPurchaseIndent
{
    public interface IPurchaseIndentQuery
    {
        Task<(List<IndentHeader>, int)> GetAllPurchaseIndentAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<bool> NotFoundAsync(int id);
        Task<IndentHeader> GetByIdAsync(int id);
        Task<string> GeneratePurchaseIndentNumberAsync(int unitId);
        
        
    }
}