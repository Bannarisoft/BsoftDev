using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundService.Application.Workflow.ApprovalRequests.Queries.GetAllApprovalRequest;
using BackgroundService.Domain.Entities.Workflow;

namespace BackgroundService.Application.Workflow.Common.Mappings
{
    public class ApprovalRequestProfile : Profile
    {
        public ApprovalRequestProfile()
        {
            CreateMap<ApprovalRequest, ApprovalRequestDto>()
            .ForMember(dest => dest.TargetTypeId, opt => opt.MapFrom(src => src.ApprovalStepDetail.TargetTypeId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Code))
            .ForMember(dest => dest.ModuleTypeName, opt => opt.MapFrom(src => src.WorkflowType.ModuleTypeName));
        }
    }
}