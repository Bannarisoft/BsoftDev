using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using System.Text;

namespace Core.Application.Common.Interfaces.IDivision
{
    public interface IDivisionCommandRepository
    {  
        Task<int> CreateAsync(Division division);     
        Task<bool> UpdateAsync(Division division);
        Task<int> DeleteAsync(int id,Division division);        
    }
}