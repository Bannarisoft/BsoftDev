  using Core.Application.Common.Interfaces.IUserSession;
 using Google.Protobuf.WellKnownTypes;
 using Grpc.Core;
 using GrpcServices.UserManagement;
 
 
 namespace UserManagement.API.GrpcServices
 {
     public class SessionGrpcService : SessionService.SessionServiceBase
     {
          private readonly IUserSessionRepository _userSessionRepository;

         public SessionGrpcService(IUserSessionRepository userSessionRepository)
         {
             _userSessionRepository = userSessionRepository;
         }

         public override async Task<SessionResponse> GetSessionByJwtId(SessionRequest request, ServerCallContext context)
         {
             var session = await _userSessionRepository.GetSessionByJwtIdAsync(request.JwtId);

             if (session is null)
                 throw new RpcException(new Status(StatusCode.NotFound, "Session not found"));

             return new SessionResponse
             {
                 Id = session.Id,
                 UserId = session.UserId,
                 JwtId = session.JwtId,
                 BrowserInfo = session.BrowserInfo,
                 CreatedAt = Timestamp.FromDateTimeOffset(session.CreatedAt),
                 ExpiresAt = Timestamp.FromDateTimeOffset(session.ExpiresAt),
                 IsActive = session.IsActive,
                 LastActivity = Timestamp.FromDateTimeOffset(session.LastActivity)
             };
         }

         public override async Task<Empty> UpdateSession(UpdateSessionRequest request, ServerCallContext context)
         {
             var session = await _userSessionRepository.GetSessionByJwtIdAsync(request.JwtId)
                 ?? throw new RpcException(new Status(StatusCode.NotFound, "Session not found"));

             session.LastActivity = request.LastActivity.ToDateTimeOffset().UtcDateTime;
             await _userSessionRepository.UpdateSessionAsync(session);

             return new Empty();
         } 
     }
 }
