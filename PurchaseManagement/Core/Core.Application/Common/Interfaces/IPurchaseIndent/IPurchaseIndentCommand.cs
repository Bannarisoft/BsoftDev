using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IPurchaseIndent
{
    public interface IPurchaseIndentCommand
    {
         Task<int> CreateAsync(IndentHeader indentHeader);     
        Task<bool> UpdateAsync(IndentHeader indentHeader);
        Task<bool> DeleteAsync(int id,IndentHeader indentHeader); 
    }
}