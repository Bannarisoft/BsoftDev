using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRule;
using BackgroundService.Domain.Entities.Workflow;
using BackgroundService.Infrastructure.Data.Notification;
using Microsoft.EntityFrameworkCore;

namespace BackgroundService.Infrastructure.Repositories.Workflow.ApprovalRules
{
    public class ApprovalRuleCommandRepository : IApprovalRuleCommand
    {
        private readonly NotificationDbContext _notificationDbContext;
        public ApprovalRuleCommandRepository(NotificationDbContext notificationDbContext)
        {
            _notificationDbContext = notificationDbContext;
        }
        public async Task<int> CreateAsync(ApprovalRule approvalRule)
        {
             _notificationDbContext.Entry(approvalRule);
            await _notificationDbContext.ApprovalRule.AddAsync(approvalRule);
            await _notificationDbContext.SaveChangesAsync();

            return approvalRule.Id;
        }

        public async Task<bool> DeleteAsync(int id, ApprovalRule approvalRule)
        {
            var ApprovalRuleDelete = await _notificationDbContext.ApprovalRule.FirstOrDefaultAsync(u => u.Id == id);
            if (ApprovalRuleDelete != null)
            {
                ApprovalRuleDelete.IsDeleted = approvalRule.IsDeleted;
                return await _notificationDbContext.SaveChangesAsync() >0;
            }
            return false; 
        }

        public async Task<bool> UpdateAsync(ApprovalRule approvalRule)
        {
             var existingApprovalRule = await _notificationDbContext.ApprovalRule
            .AsNoTracking().FirstOrDefaultAsync(u => u.Id == approvalRule.Id);
            
            if (existingApprovalRule != null)
            {
                // existingApprovalRule.ConditionKey = approvalRule.ConditionKey;
                // existingApprovalRule.Operator = approvalRule.Operator;
                // existingApprovalRule.Value = approvalRule.Value;
                existingApprovalRule.IsActive = approvalRule.IsActive;
                _notificationDbContext.ApprovalRule.Update(existingApprovalRule);

                return await _notificationDbContext.SaveChangesAsync() >0;
            }
            
            return false; 
        }
    }
}