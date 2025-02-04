using Core.Domain.Entities;
using Core.Application.Common.Mappings;

namespace Core.Application.Users.Queries.GetUserAutoComplete
{
    public class UserAutoCompleteDtoo : IMapFrom<User>
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}