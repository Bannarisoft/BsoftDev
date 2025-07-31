using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundService.Application.Workflow.ApprovalStepDetails.Commands.CreateApprovalStepDetail;
using BackgroundService.Application.Workflow.ApprovalStepDetails.Commands.DeleteApprovalStepDetail;
using BackgroundService.Application.Workflow.ApprovalStepDetails.Commands.UpdateApprovalStepDetail;
using BackgroundService.Application.Workflow.ApprovalStepDetails.Queries.GetAllApprovalStepDetail;
using BackgroundService.Domain.Entities.Notification;
using BackgroundService.Domain.Entities.Workflow;
using static BackgroundService.Domain.Common.BaseEntity;

namespace BackgroundService.Application.Workflow.Common.Mappings
{
    public class ApprovalStepDetailProfile : Profile
    {
        public ApprovalStepDetailProfile()
        {
            CreateMap<ApprovalStepDetail, ApprovalStepDetailDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == Status.Active ? 1 : 0))
            .ForMember(dest => dest.WorkflowType, opt => opt.MapFrom(src => src.WorkflowType))
            .ForMember(dest => dest.ApprovalStep, opt => opt.MapFrom(src => src.ApprovalStep))
            .ForMember(dest => dest.ApprovalType, opt => opt.MapFrom(src => src.ApprovalType));

            CreateMap<WorkflowType, WorkflowTypeApprovalStepDto>();
            CreateMap<Domain.Entities.Notification.MiscMaster, ApprovalStepDto>()
            .ForMember(dest => dest.StepName, opt => opt.MapFrom(src => src.Code));
            CreateMap<Domain.Entities.Notification.MiscMaster, ApprovalTypeDto>()
            .ForMember(dest => dest.ApproverTypeName, opt => opt.MapFrom(src => src.Code));

            CreateMap<CreateApprovalStepDetailCommand, ApprovalStepDetail>()
                 .ForMember(dest => dest.Id, opt => opt.Ignore())
                 .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
                 .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted))
                 .ForMember(dest => dest.ApprovalStepUnitMappings, opt => opt.MapFrom(src => src.ApprovalStepUnitMappings))
                 .ForMember(dest => dest.RuleSkipApproverMappings, opt => opt.MapFrom(src => src.RuleSkipApproverMappings))
                 .ForMember(dest => dest.ApprovalStepDepartmentMappings, opt => opt.MapFrom(src => src.ApprovalStepDepartmentMappings));

            CreateMap<ApprovalStepUnitMappingDto, ApprovalStepUnitMapping>();
            CreateMap<RuleSkipApproverMappingDto, RuleSkipApproverMapping>();

            CreateMap<UpdateApprovalStepDetailCommand, ApprovalStepDetail>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == 1 ? Status.Active : Status.Inactive))
                .ForMember(dest => dest.ApprovalStepUnitMappings, opt => opt.MapFrom(src => src.ApprovalStepUnitMappings))
                 .ForMember(dest => dest.RuleSkipApproverMappings, opt => opt.MapFrom(src => src.RuleSkipApproverMappings))
                 .ForMember(dest => dest.ApprovalStepDepartmentMappings, opt => opt.MapFrom(src => src.ApprovalStepDepartmentMappings));


              CreateMap<DeleteApprovalStepDetailCommand, ApprovalStepDetail>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) 
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));   
        }
    }
}