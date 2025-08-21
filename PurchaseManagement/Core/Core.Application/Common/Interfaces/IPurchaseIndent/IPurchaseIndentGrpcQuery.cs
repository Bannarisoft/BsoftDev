using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IPurchaseIndent
{
    public interface IPurchaseIndentGrpcQuery
    {
        Task<IndentHeader> GetByIdGrpcAsync(int id);
    }
}