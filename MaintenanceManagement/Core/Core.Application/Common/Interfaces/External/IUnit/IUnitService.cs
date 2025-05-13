using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Dtos.Maintenance;

namespace Core.Application.Common.Interfaces.External.IUnit
{
    public interface IUnitService
    {
        Task<List<UnitDto>> GetUnitAutoCompleteAsync();
    }
}