using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Commands.Purchase;
using Contracts.Dtos.Purchase;
using Core.Application.PurchaseIndents.Command.CreatePurchaseIndent;
using Core.Application.PurchaseIndents.Command.DeletePurchaseIndent;
using Core.Application.PurchaseIndents.Command.UpdatePurchaseIndent;
using Core.Application.PurchaseIndents.Queries.GetAllPurchaseIndent;
using Core.Application.PurchaseIndents.Queries.GetPendingIndent;
using Core.Application.PurchaseIndents.Queries.GetPendingIndentById;
using Core.Application.PurchaseIndents.Queries.GetPurchaseIndentById;
using Core.Domain.Entities;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class PurchaseIndentProfile : Profile
    {
    public PurchaseIndentProfile()
    {
      CreateMap<CreatePurchaseIndentCommand, IndentHeader>()
       .ForMember(dest => dest.Id, opt => opt.Ignore())
           .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
           .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted))
           .ForMember(dest => dest.IndentDetails, opt => opt.MapFrom(src => src.IndentDetails));

      CreateMap<PurchaseIndents.Command.CreatePurchaseIndent.IndentDetailDto, IndentDetail>();

      CreateMap<DeletePurchaseIndentCommand, IndentHeader>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));

      CreateMap<UpdatePurchaseIndentCommand, IndentHeader>()
        .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == 1 ? Status.Active : Status.Inactive))
        .ForMember(dest => dest.IndentDetails, opt => opt.MapFrom(src => src.IndentDetails));

      CreateMap<IndentDetailUpdateDto, IndentDetail>();

      CreateMap<IndentHeader, PurchaseIndents.Queries.GetAllPurchaseIndent.IndentDto>()
      .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == Status.Active ? 1 : 0))
      .ForMember(dest => dest.IndentType, opt => opt.MapFrom(src => src.IndentType.Code));

      CreateMap<IndentHeader, IndentByIdDto>();
      CreateMap<IndentDetail, IndentDetailByIdDto>();

      CreateMap<IndentHeader, UpdatePurchaseIndentCommand>()
        .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == Status.Active ? 1 : 0))
        .ForMember(dest => dest.IndentDetails, opt => opt.MapFrom(src => src.IndentDetails));

      CreateMap<IndentDetail, IndentDetailUpdateDto>();

      CreateMap<IndentHeader, IndentReverseMapDto>()
           .ForMember(dest => dest.Header, opt => opt.MapFrom(src => src))
           .ForMember(dest => dest.Lines, opt => opt.MapFrom(src => src.IndentDetails));

      CreateMap<IndentHeader, CreateIndentHeaderDto>();
      CreateMap<IndentDetail, CreateIndentDetailDto>();

      CreateMap<IndentHeader, PendingIndentDto>()
      .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == Status.Active ? 1 : 0))
      .ForMember(dest => dest.IndentType, opt => opt.MapFrom(src => src.IndentType.Code));

      CreateMap<IndentHeader, PendingIndentByIdDto>();
      CreateMap<IndentDetail, PendingIndentDetailByIdDto>()
      .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Code));

      CreateMap<UpdateApprovedQtyDto, IndentDetail>()
      .ForMember(dest => dest.ApprovedQuantity, opt => opt.MapFrom(src => src.ApprovedQuantity))
      .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IndentDetailId))
      .ForMember(dest => dest.Status, opt => opt.Ignore());

      CreateMap<UpdateIndentDetailCommand, IndentHeader>()
      .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IndentId))
      .ForMember(dest => dest.IndentDetails, opt => opt.MapFrom(src => src.ApprovedQty))
      .ForMember(dest => dest.Status, opt => opt.Ignore());
        }
        
    }
}