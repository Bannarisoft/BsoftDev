using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.ILogService
{
    public interface ILogServiceCommand
    {
        Task<bool> CreateAsync(IndentLog indentLog);  
    }
}