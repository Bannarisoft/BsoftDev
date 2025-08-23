using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces.ILogService;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.Common.Interfaces.IPurchaseIndent;
using Core.Application.PurchaseIndents.Command.UpdatePurchaseIndent;
using Core.Domain.Common;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using PurchaseManagement.Infrastructure.Data;

namespace PurchaseManagement.Infrastructure.Repositories.PurchaseIndents
{
    public class PurchaseIndentCommandRepository : IPurchaseIndentCommand
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogServiceCommand _logServiceCommand;
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
        private readonly IMapper _imapper;
        public PurchaseIndentCommandRepository(ApplicationDbContext dbContext, ILogServiceCommand logServiceCommand,
        IMiscMasterQueryRepository miscMasterQueryRepository, IMapper imapper)
        {
            _dbContext = dbContext;
            _logServiceCommand = logServiceCommand;
            _miscMasterQueryRepository = miscMasterQueryRepository;
            _imapper = imapper;
        }
        public async Task<IndentHeader> CreateAsync(IndentHeader indentHeader)
        {
            _dbContext.Entry(indentHeader);
            await _dbContext.IndentHeader.AddAsync(indentHeader);
            await _dbContext.SaveChangesAsync();

            return indentHeader;
        }

        public async Task<bool> DeleteAsync(int id, IndentHeader indentHeader)
        {
            var PurchaseIndentDelete = await _dbContext.IndentHeader.FirstOrDefaultAsync(u => u.Id == id);
            if (PurchaseIndentDelete != null)
            {
                PurchaseIndentDelete.IsDeleted = indentHeader.IsDeleted;
                return await _dbContext.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> UpdateAsync(IndentHeader indentHeader, string request)
        {
            var existingPurchaseIndent = await _dbContext.IndentHeader
             .Include(cf => cf.IndentDetails)
           .FirstOrDefaultAsync(u => u.Id == indentHeader.Id);

            var Indent = _imapper.Map<UpdatePurchaseIndentCommand>(existingPurchaseIndent);

            var StatusMisc = await _miscMasterQueryRepository.GetMiscMasterByName(MiscEnumEntity.Status, MiscEnumEntity.Open);
            var IndentLog = new IndentLog
            {
                IndentHeaderId = indentHeader.Id,
                ActionType = "Updated",
                ActionRemarks = "Indent Updated",
                PreviousData = JsonSerializer.Serialize(Indent),
                NewData = request,
                StatusId = StatusMisc.Id
            };

            await _logServiceCommand.CreateAsync(IndentLog);

            if (existingPurchaseIndent != null)
            {


                existingPurchaseIndent.IndentTypeId = indentHeader.IndentTypeId;
                existingPurchaseIndent.IndentDate = indentHeader.IndentDate;
                existingPurchaseIndent.UnitId = indentHeader.UnitId;
                existingPurchaseIndent.DepartmentId = indentHeader.DepartmentId;
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


                return await _dbContext.SaveChangesAsync() > 0;
            }

            return false;
        }
        public async Task<List<IndentDetail>> UpdateIndentDetailAsync(List<IndentDetail> indentDetail)
        {
            if (indentDetail == null || indentDetail.Count == 0)
                    return new List<IndentDetail>();

                var ids = indentDetail.Select(d => d.Id).ToList();

                var existing = await _dbContext.IndentDetail
                    .Where(d => ids.Contains(d.Id))
                    .ToListAsync();

                
                foreach (var entity in existing)
                {
                    var incoming = indentDetail.FirstOrDefault(u => u.Id == entity.Id);
                    if (incoming == null) continue;

                    entity.ApprovedQuantity = incoming.ApprovedQuantity;
                    
                }

                await _dbContext.SaveChangesAsync();
                return existing;
                
        }
    }
}