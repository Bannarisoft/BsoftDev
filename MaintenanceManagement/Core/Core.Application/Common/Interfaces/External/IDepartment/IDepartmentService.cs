using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Models.Maintenance;

namespace Core.Application.Common.Interfaces.External.IDepartment
{
    public interface IDepartmentService
    {
        Task<List<DepartmentDto>> GetAllDepartmentAsync();
    }
}