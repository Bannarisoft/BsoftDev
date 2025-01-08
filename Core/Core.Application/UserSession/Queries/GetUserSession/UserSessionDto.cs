using Core.Application.Common.Mappings;

namespace Core.Application.UserSession.Queries.GetUserSession
{
    public class UserSessionDto : IMapFrom<Core.Domain.Entities.UserSession>
    {
     public int UserId { get; set; }          
     public string UserName { get; set; }
     public string SessionId { get; set; }     // Unique session identifier
     public string Token { get; set; }         // Authentication token
     public DateTime CreatedAt { get; set; }   // Session creation time
     public byte IsActive { get; set; }        // Indicates if the session is active
     public string Browser { get; set; }       // Browser information (User-Agent)
     public string CreatedIP { get; set; }
     public string Status { get; set; }
    }
}