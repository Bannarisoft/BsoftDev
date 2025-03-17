namespace Core.Application.Common.Interfaces.IUserGroup
{
    using UserGroup = Core.Domain.Entities.UserGroup;
    public interface IUserGroupQueryRepository
    {        
        Task<List< UserGroup>> GetUserGroups (string searchPattern);
    }
}