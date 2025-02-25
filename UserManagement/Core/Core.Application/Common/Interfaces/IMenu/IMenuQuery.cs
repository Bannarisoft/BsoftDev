using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IMenu
{
    public interface IMenuQuery
    {
        Task<List<Domain.Entities.Menu>> GetParentMenus(List<int> moduleId);
        Task<List<Domain.Entities.Menu>> GetChildMenus(List<int> ParentId);
    }
}