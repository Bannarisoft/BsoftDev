using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundService.Application.Dto;
using BackgroundService.Application.Workflow.ApprovalRequests.Commands.ApproveApprovalRequest;
using BackgroundService.Application.Workflow.ApprovalRequests.Queries.GetAllApprovalRequest;
using BackgroundService.Domain.Entities.Workflow;
using Contracts.Dtos.Purchase;

namespace BackgroundService.Application.Workflow.Common.Mappings
{
    public class ApprovalRequestProfile : Profile
    {
        public ApprovalRequestProfile()
        {
            CreateMap<ApprovalRequest, ApprovalRequestDto>()
            // .ForMember(dest => dest.TargetTypeId, opt => opt.MapFrom(src => src.ApprovalStepDetail.TargetTypeId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Code));
            // .ForMember(dest => dest.ModuleTypeName, opt => opt.MapFrom(src => src.WorkflowType.ModuleTypeName));

            CreateMap<ApprovalRequest, ApprovalRequestHeaderDto>()
          .ForMember(dest => dest.CurrentStatus, opt => opt.MapFrom(src => src.Status.Code));

            CreateMap<ApprovalRequestLine, ApprovalRequestLineDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Code));

            CreateMap<ApproveApprovalRequestCommand, ApprovalRequest>()
            .ForMember(dest => dest.ApprovalRequestLines, opt => opt.MapFrom(src => src.ApprovalRequestLine))
            .ForMember(dest => dest.ApprovalDocuments, opt => opt.MapFrom(src => src.ApprovalDocument));

            CreateMap<ApprovalDocumentDto, ApprovalDocument>();

            CreateMap<ApproveApprovalRequestLineDto, ApprovalRequestLine>();
            CreateMap<ApproveApprovalRequestLineDto, UpdateApprovedQtyDto>()
            .ForMember(dest => dest.ApprovedQuantity, opt => opt.MapFrom(src => src.ApprovedQuantity))
            .ForMember(dest => dest.IndentDetailId, opt => opt.MapFrom(src => src.ModuleLineTransactionId));

            CreateMap<ApproveApprovalRequestLineDto, ApproveLineStatusDto>()
            .ForMember(dest => dest.ApprovalRequestLineId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ModuleLineTransactionId, opt => opt.MapFrom(src => src.ModuleLineTransactionId))
            .ForMember(dest => dest.IsApproved, opt => opt.MapFrom(src => src.IsApproved));
            
        }
    }
}