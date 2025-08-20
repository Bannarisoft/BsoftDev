using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.PartyMaster.Command.CreatePartyMaster;
using Core.Application.PartyMaster.Command.DeletePartyMaster;
using Core.Application.PartyMaster.Command.UpdatePartyMaster;
using Core.Domain.Entities;
using static Core.Application.PartyMaster.Command.CreatePartyMaster.CreatePartyMasterDto;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class PartyMasterProfile : Profile
    {
        public PartyMasterProfile()
        {
            //Create DTO
            CreateMap<CreatePartyMasterDto, Core.Domain.Entities.PartyMaster>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PartyAddressTypes, opt => opt.MapFrom(src => src.PartyAddresses))
            .ForMember(dest => dest.PartyBankTypes, opt => opt.MapFrom(src => src.PartyBanks))
            .ForMember(dest => dest.PartyContactTypes, opt => opt.MapFrom(src => src.PartyContacts))
            .ForMember(dest => dest.PartyDocumentTypes, opt =>
                {
                    opt.PreCondition(src => src.PartyDocuments != null && src.PartyDocuments.Any());
                    opt.MapFrom(src => src.PartyDocuments);
                })
                        .ForMember(dest => dest.IsMsmeCompliant, opt => opt.MapFrom(src => src.IsMsmeCompliant == 1 ? true : false))
            .ForMember(dest => dest.IsTDSApplicable, opt => opt.MapFrom(src => src.IsTDSApplicable == 1 ? true : false))
            .ForMember(dest => dest.IsTCSApplicable, opt => opt.MapFrom(src => src.IsTCSApplicable == 1 ? true : false))
            .ForMember(dest => dest.IsGstReverseCharge, opt => opt.MapFrom(src => src.IsGstReverseCharge == 1 ? true : false))
            .ForMember(dest => dest.Is206AB206CCAApplicable, opt => opt.MapFrom(src => src.Is206AB206CCAApplicable == 1 ? true : false))
            .ForMember(dest => dest.IsInternalSupplier, opt => opt.MapFrom(src => src.IsInternalSupplier == 1 ? true : false))
            .ForMember(dest => dest.IsInternalCustomer, opt => opt.MapFrom(src => src.IsInternalCustomer == 1 ? true : false))
            .ForMember(dest => dest.IsStopPayment, opt => opt.MapFrom(src => src.IsStopPayment == 1 ? true : false))
            .ForMember(dest => dest.PartyStatus, opt => opt.MapFrom(src => "Pending"))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));
            CreateMap<PartyContactDto, Core.Domain.Entities.PartyContact>();
            CreateMap<PartyAddressDto, Core.Domain.Entities.PartyAddress>();
            CreateMap<PartyBankDto, Core.Domain.Entities.PartyBank>();
            CreateMap<PartyTypeDto, Core.Domain.Entities.PartyType>();
            CreateMap<PartyDocumentDto, Core.Domain.Entities.PartyDocument>();

            //Update DTO
            CreateMap<UpdatePartyMasterDto, Core.Domain.Entities.PartyMaster>()
            .ForMember(dest => dest.PartyTypes, opt => opt.MapFrom(src => src.PartyTypesUpdate))
            .ForMember(dest => dest.PartyContactTypes, opt => opt.MapFrom(src => src.PartyContactsUpdate))
            .ForMember(dest => dest.PartyAddressTypes, opt => opt.MapFrom(src => src.PartyAddressesUpdate))
            .ForMember(dest => dest.PartyBankTypes, opt => opt.MapFrom(src => src.PartyBanksUpdate))
             .ForMember(dest => dest.PartyDocumentTypes, opt =>
                {
                    opt.PreCondition(src => src.PartyDocumentsUpdate != null && src.PartyDocumentsUpdate.Any());
                    opt.MapFrom(src => src.PartyDocumentsUpdate);
                })
            //.ForMember(dest => dest.PartyDocumentTypes, opt => opt.MapFrom(src => src.PartyDocumentsUpdate))
            .ForMember(dest => dest.IsTDSApplicable, opt => opt.MapFrom(src => src.IsTDSApplicable == 1 ? true : false))
            .ForMember(dest => dest.IsTCSApplicable, opt => opt.MapFrom(src => src.IsTCSApplicable == 1 ? true : false))
            .ForMember(dest => dest.IsGstReverseCharge, opt => opt.MapFrom(src => src.IsGstReverseCharge == 1 ? true : false))
            .ForMember(dest => dest.Is206AB206CCAApplicable, opt => opt.MapFrom(src => src.Is206AB206CCAApplicable == 1 ? true : false))
            .ForMember(dest => dest.IsInternalSupplier, opt => opt.MapFrom(src => src.IsInternalSupplier == 1 ? true : false))
            .ForMember(dest => dest.IsInternalCustomer, opt => opt.MapFrom(src => src.IsInternalCustomer == 1 ? true : false))
            .ForMember(dest => dest.IsStopPayment, opt => opt.MapFrom(src => src.IsStopPayment == 1 ? true : false))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == 1 ? Status.Active : Status.Inactive));
            CreateMap<UpdatePartyMasterDto.UpdatePartyTypeDto, PartyType>();
            CreateMap<UpdatePartyMasterDto.UpdatePartyContactDto, PartyContact>();
            CreateMap<UpdatePartyMasterDto.UpdatePartyAddressDto, PartyAddress>();
            CreateMap<UpdatePartyMasterDto.UpdatePartyBankDto, PartyBank>();
            CreateMap<UpdatePartyMasterDto.UpdatePartyDocumentDto, PartyDocument>();

            //Delete 

            CreateMap <DeletePartyMasterCommand, Core.Domain.Entities.PartyMaster>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted)); 



        }
    }
}