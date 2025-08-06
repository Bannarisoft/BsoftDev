using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IPurchaseIndent;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using PurchaseManagement.Infrastructure.Data;

namespace PurchaseManagement.Infrastructure.Repositories.PurchaseIndents
{
    public class PurchaseIndentCommandRepository : IPurchaseIndentCommand
    {
        private readonly ApplicationDbContext _dbContext;
        public PurchaseIndentCommandRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<int> CreateAsync(IndentHeader indentHeader)
        {
            _dbContext.Entry(indentHeader);
            await _dbContext.IndentHeader.AddAsync(indentHeader);
            await _dbContext.SaveChangesAsync();

            return indentHeader.Id;
        }

        public async Task<bool> DeleteAsync(int id, IndentHeader indentHeader)
        {
            var PurchaseIndentDelete = await _dbContext.IndentHeader.FirstOrDefaultAsync(u => u.Id == id);
            if (PurchaseIndentDelete != null)
            {
                PurchaseIndentDelete.IsDeleted = indentHeader.IsDeleted;
                return await _dbContext.SaveChangesAsync() >0;
            }
            return false; 
        }

        public async Task<bool> UpdateAsync(IndentHeader indentHeader)
        {
             var existingPurchaseIndent = await _dbContext.IndentHeader
              .Include(cf => cf.IndentDetails)
            .Include(cf => cf.IndentDepartmentMappings)
            .FirstOrDefaultAsync(u => u.Id == indentHeader.Id);
            
            if (existingPurchaseIndent != null)
            {
                 _dbContext.IndentDepartmentMapping.RemoveRange(existingPurchaseIndent.IndentDepartmentMappings);
                 
                 existingPurchaseIndent.IndentTypeId = indentHeader.IndentTypeId;
                 existingPurchaseIndent.UnitId = indentHeader.UnitId;
                 existingPurchaseIndent.Purpose = indentHeader.Purpose;

                 foreach (var updatedDetail in indentHeader.IndentDetails)
                {
                    var existingDetail = existingPurchaseIndent.IndentDetails
                        .FirstOrDefault(d => d.Id == updatedDetail.Id);

                    if (existingDetail != null)
                    {
                        existingDetail.ItemId = updatedDetail.ItemId;
                        existingDetail.QuantityRequired = updatedDetail.QuantityRequired;
                        existingDetail.RequiredDate = updatedDetail.RequiredDate;
                        existingDetail.TotalEstimatedCost = updatedDetail.TotalEstimatedCost;
                        existingDetail.PRConsumptionDays = updatedDetail.PRConsumptionDays;
                        existingDetail.Remark = updatedDetail.Remark;

                    }
                    else
                    {

                        existingPurchaseIndent.IndentDetails.Add(updatedDetail);
                    }
                }
                
                
                if (indentHeader.IndentDepartmentMappings?.Any() == true)
                   await _dbContext.IndentDepartmentMapping.AddRangeAsync(indentHeader.IndentDepartmentMappings);


                return await _dbContext.SaveChangesAsync() > 0;
            }
            
            return false;
        }
    }
}