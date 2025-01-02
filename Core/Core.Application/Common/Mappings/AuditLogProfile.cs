using AutoMapper;
using Core.Application.AuditLog.Commands.CreateAuditLog;
using Core.Domain.Entities;

public class AuditLogMappingProfile : Profile
{
    public AuditLogMappingProfile()
    {
        CreateMap<CreateAuditLogCommand, AuditLogs>();
    }
}
