using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Core.Application.Common;
using Core.Application.AuditLog.Queries.GetAuditLog;

namespace Core.Application.AuditLog.Commands.CreateAuditLog
{
    public class CreateAuditLogCommandHandler : IRequestHandler<CreateAuditLogCommand, Result<AuditLogDto>>
    {
        private readonly IMapper _mapper;
        private readonly IAuditLogRepository _auditLogRepository;

        public CreateAuditLogCommandHandler(IMapper mapper, IAuditLogRepository auditLogRepository)
        {
            
            _mapper = mapper;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<Result<AuditLogDto>> Handle(CreateAuditLogCommand request, CancellationToken cancellationToken)
        {
            // Map the incoming request to the AuditLog entity
            var auditLogEntity = _mapper.Map<AuditLogs>(request);

            // Use the MongoDB repository to create the entity
            var createdAuditLog = await _auditLogRepository.CreateAsync(auditLogEntity);

            // Map the created entity to the DTO
            var auditLogDto = _mapper.Map<AuditLogDto>(createdAuditLog);

            // Return the result with success status
            return Result<AuditLogDto>.Success(auditLogDto);
        }
    }
}
