using BackgroundService.Application.Notification.Common.Interfaces;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationLevelHierarchy;
using BackgroundService.Infrastructure.Data.Notification;
using BackgroundService.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace BackgroundService.Infrastructure.Repositories.Notification.NotificationLevelHierarchy
{
    public class NotificationLevelHierarchyCommandRepository : BaseQueryRepository, INotificationLevelHierarchyCommandRepository
    {
        private readonly NotificationDbContext _applicationDbContext;

        public NotificationLevelHierarchyCommandRepository(NotificationDbContext applicationDbContext, IIPAddressService ipAddressService)
        : base(ipAddressService)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<int> CreateAsync(Domain.Entities.Notification.NotificationLevelHierarchy NotificationLevelHierarchy)
        {
            await _applicationDbContext.NotificationLevelHierarchy.AddAsync(NotificationLevelHierarchy);
            await _applicationDbContext.SaveChangesAsync();
            return NotificationLevelHierarchy.Id;
        }

        public async Task<int> DeleteAsync(int Id, Domain.Entities.Notification.NotificationLevelHierarchy NotificationLevelHierarchy)
        {
            var NotificationLevelHierarchyToDelete = await _applicationDbContext.NotificationLevelHierarchy.FirstOrDefaultAsync(u => u.Id == Id);
            if (NotificationLevelHierarchyToDelete is null)
            {
                return -1;
            }
            NotificationLevelHierarchyToDelete.IsDeleted = NotificationLevelHierarchy.IsDeleted;
            await _applicationDbContext.SaveChangesAsync();
            return 1;
        }
        public async Task<int> UpdateAsync(int Id, Domain.Entities.Notification.NotificationLevelHierarchy NotificationLevelHierarchy)
        {
            var existingNotificationLevelHierarchy = await _applicationDbContext.NotificationLevelHierarchy.FirstOrDefaultAsync(u => u.Id == Id);
            if (existingNotificationLevelHierarchy is null)
            {
                return -1;
            }
            existingNotificationLevelHierarchy.NotificationConfigId = NotificationLevelHierarchy.NotificationConfigId;
            existingNotificationLevelHierarchy.TargetTypeId = NotificationLevelHierarchy.TargetTypeId;
            existingNotificationLevelHierarchy.TargetId = NotificationLevelHierarchy.TargetId;
            existingNotificationLevelHierarchy.Description = NotificationLevelHierarchy.Description;
            existingNotificationLevelHierarchy.ApprovalModeId = NotificationLevelHierarchy.ApprovalModeId;
            existingNotificationLevelHierarchy.IsActive = NotificationLevelHierarchy.IsActive;

            _applicationDbContext.NotificationLevelHierarchy.Update(existingNotificationLevelHierarchy);
            await _applicationDbContext.SaveChangesAsync();
            return 1;
        }
        public async Task<bool> IsNameDuplicateAsync(int notificationConfigId, int targetTypeId, int targetId, int excludeId)
        {
            return await _applicationDbContext.NotificationLevelHierarchy
                .AnyAsync(cc => cc.NotificationConfigId == notificationConfigId && cc.TargetTypeId == targetTypeId && cc.TargetId == targetId && cc.IsDeleted == 0 && cc.Id != excludeId);
        }       
        public async Task<bool> ExistsByCodeAsync(int notificationConfigId, int targetTypeId, int targetId)
        {
            return await _applicationDbContext.NotificationLevelHierarchy
                .AnyAsync(cc => cc.NotificationConfigId == notificationConfigId && cc.TargetTypeId == targetTypeId && cc.TargetId == targetId && cc.IsDeleted == 0);
        }
    }
}