using MediatR;
using Core.Application.Common.Interfaces;
using Core.Application.Common;
using Core.Domain.Events;

namespace Core.Application.EventHandlers
{
    public class DomainEventHandler : INotificationHandler<AuditLogsDomainEvent>
    {
        private readonly IMongoDbContext _mongoDbContext;
        private readonly IIPAddressService _ipAddressService;
        public DomainEventHandler(IMongoDbContext mongoDbContext, IIPAddressService ipAddressService)
        {
            _mongoDbContext = mongoDbContext;
             _ipAddressService = ipAddressService;
        }

        public async Task Handle(AuditLogsDomainEvent notification, CancellationToken cancellationToken)
        {          
            var userIdString = _ipAddressService.GetUserId();        
            var auditLog = new
            {
                Module = notification.Module,   
                Action = notification.ActionDetail,
                Details = notification.Details,             
                MachineName = Environment.MachineName,
                OS = _ipAddressService.GetUserOS(),
                IPAddress = _ipAddressService.GetSystemIPAddress(),
                Browser = _ipAddressService.GetUserAgent(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy =userIdString,
                CreatedByName=_ipAddressService.GetUserName(),
            };

            await _mongoDbContext.GetCollection<dynamic>("AuditLogs").InsertOneAsync(auditLog, cancellationToken: cancellationToken);
        }
    }
}
