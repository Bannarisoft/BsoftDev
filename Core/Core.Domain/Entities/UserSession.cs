using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class UserSession
    {
     public int Id { get; set; }               // Primary Key
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