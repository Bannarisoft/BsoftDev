using System.Data;
using BackgroundService.Application.Notification.Common.Interfaces;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationLevelHierarchy;
using BackgroundService.Application.Notification.NotificationLevelHierarchy.Queries.GetAllNotificationLevelHierarchy;
using BackgroundService.Infrastructure.Repositories.Common;
using Dapper;

namespace  BackgroundService.Infrastructure.Repositories.Notification.NotificationLevelHierarchy
{
    public class NotificationLevelHierarchyQueryRepository : BaseQueryRepository,INotificationLevelHierarchyQueryRepository
    {
        private readonly IDbConnection _dbConnection;        

        public NotificationLevelHierarchyQueryRepository(IDbConnection dbConnection, IIPAddressService ipAddressService)
        : base(ipAddressService) 
        {
            _dbConnection = dbConnection;            
        }

        public async Task<NotificationLevelHierarchyDto> GetByIdAsync(int Id)
        {
            var UnitId = _ipAddressService.GetUnitId();
            const string query = @" select NH.Id,NotificationConfigId,TargetTypeId,TargetId,UnitId,ApprovalModeId,NH.Description,NH.IsActive,NH.IsDeleted,NH.CreatedBy,NH.CreatedDate,
                NH.CreatedByName,NH.CreatedIP,NH.ModifiedBy,NH.ModifiedDate,NH.ModifiedByName,NH.ModifiedIP,
                NC.ModuleName,MM2.Code NotificationEventType,MM.Code TargetType,
                case when MM.Code='USER' then U.UserName else Case when MM.Code='ROLE' then R.RoleName else Case when MM.Code='DEPT' then D.DeptName else NG.GroupName end end end as TargetName,
                MM1.Code ApprovalMode
                from AppNotification.NotificationLevelHierarchy NH
                INNER JOIN AppNotification.NotificationConfig NC on NH.NotificationConfigId=NC.Id
                INNER JOIN AppData.MiscMaster MM2 on MM2.Id=NC.NotificationEventTypeId
                INNER JOIN AppData.MiscMaster MM on MM.Id=NH.TargetTypeId
                LEFT JOIN Bannari.AppSecurity.Users U on U.UserId=NH.TargetId and MM.Code='USER'
                LEFT JOIN Bannari.AppSecurity.UserRole R on R.Id=NH.TargetId  and MM.Code='ROLE'
                LEFT JOIN AppNotification.NotificationGroup NG on NG.Id=NH.TargetId
                LEFT JOIN Bannari.AppData.Department D on D.Id=NH.TargetId and MM.Code='DEPT'
                INNER JOIN AppData.MiscMaster MM1 on MM1.Id=NH.ApprovalModeId                
                where NC.UnitId=@UnitId and NH.IsDeleted=0 and NH.Id=@Id ";

            var NotificationLevelHierarchy = await _dbConnection.QueryFirstOrDefaultAsync<NotificationLevelHierarchyDto>(query, new { UnitId,Id });
            return NotificationLevelHierarchy;
        }

        public async Task<(IEnumerable<dynamic>, int)> GetAllNotificationLevelHierarchyAsync(int PageNumber, int PageSize, string? SearchTerm)
        {      
            var UnitId = _ipAddressService.GetUnitId();      
            var query = $$"""
            DECLARE @TotalCount INT;
            SELECT @TotalCount = COUNT(*) 
            from AppNotification.NotificationLevelHierarchy NH
            INNER JOIN AppNotification.NotificationConfig NC on NH.NotificationConfigId=NC.Id
            INNER JOIN AppData.MiscMaster MM2 on MM2.Id=NC.NotificationEventTypeId
            INNER JOIN AppData.MiscMaster MM on MM.Id=NH.TargetTypeId
            LEFT JOIN Bannari.AppSecurity.Users U on U.UserId=NH.TargetId and MM.Code='USER'
            LEFT JOIN Bannari.AppSecurity.UserRole R on R.Id=NH.TargetId  and MM.Code='ROLE'
            LEFT JOIN AppNotification.NotificationGroup NG on NG.Id=NH.TargetId
            LEFT JOIN Bannari.AppData.Department D on D.Id=NH.TargetId and MM.Code='DEPT'
            INNER JOIN AppData.MiscMaster MM1 on MM1.Id=NH.ApprovalModeId
            where NC.UnitId=@UnitId and NH.IsDeleted=0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (ModuleName LIKE @Search)")}};

            SELECT  NH.Id,NotificationConfigId,TargetTypeId,TargetId,NC.UnitId,ApprovalModeId,NH.Description,NH.IsActive,NH.IsDeleted,NH.CreatedBy,NH.CreatedDate,
            NH.CreatedByName,NH.CreatedIP,NH.ModifiedBy,NH.ModifiedDate,NH.ModifiedByName,NH.ModifiedIP,
            NC.ModuleName,MM2.Code NotificationEventType,MM.Code TargetType,
            case when MM.Code='USER' then U.UserName else Case when MM.Code='ROLE' then R.RoleName else Case when MM.Code='DEPT' then D.DeptName else NG.GroupName end end end as TargetName,
            MM1.Code ApprovalMode
            from AppNotification.NotificationLevelHierarchy NH
            INNER JOIN AppNotification.NotificationConfig NC on NH.NotificationConfigId=NC.Id
            INNER JOIN AppData.MiscMaster MM2 on MM2.Id=NC.NotificationEventTypeId
            INNER JOIN AppData.MiscMaster MM on MM.Id=NH.TargetTypeId
            LEFT JOIN Bannari.AppSecurity.Users U on U.UserId=NH.TargetId and MM.Code='USER'
            LEFT JOIN Bannari.AppSecurity.UserRole R on R.Id=NH.TargetId  and MM.Code='ROLE'
            LEFT JOIN AppNotification.NotificationGroup NG on NG.Id=NH.TargetId
            LEFT JOIN Bannari.AppData.Department D on D.Id=NH.TargetId and MM.Code='DEPT'
            INNER JOIN AppData.MiscMaster MM1 on MM1.Id=NH.ApprovalModeId            
   
            where NC.UnitId=@UnitId and NH.IsDeleted=0 
            {{ (string.IsNullOrEmpty(SearchTerm) ? "" : "AND (ModuleName LIKE @Search )")}}
            ORDER BY Id desc
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

            SELECT @TotalCount AS TotalCount;
            """;

            var parameters = new
            {
                UnitId,
                Search = $"%{SearchTerm}%",
                Offset = (PageNumber - 1) * PageSize,
                PageSize
            };

            var NotificationLevelHierarchy = await _dbConnection.QueryMultipleAsync(query, parameters);
            var NotificationLevelHierarchyList = (await NotificationLevelHierarchy.ReadAsync<NotificationLevelHierarchyDto>()).ToList();
            int totalCount = (await NotificationLevelHierarchy.ReadFirstAsync<int>());
            return (NotificationLevelHierarchyList, totalCount);
        }
        public async Task<bool> SoftDeleteValidation(int Id)
        {
             const string query = @"
                    SELECT 1 
                    FROM AppNotification.NotificationEventLog
                    WHERE NotificationLevelHierarchyId = @Id AND IsDeleted = 0";
            using var multi = await _dbConnection.QueryMultipleAsync(query, new { Id = Id });
            var notificationLevelHierarchyExists = await multi.ReadFirstOrDefaultAsync<int?>();
            return notificationLevelHierarchyExists.HasValue;
        }
        public async Task<bool> NotFoundAsync(int Id)
        {
            var query = "SELECT COUNT(1) FROM AppNotification.NotificationLevelHierarchy WHERE Id = @Id AND IsDeleted = 0";             
            var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = Id });
            return count > 0;
        }   
    }
}