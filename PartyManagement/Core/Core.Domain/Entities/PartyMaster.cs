using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class PartyMaster : BaseEntity
    {
        public int CompanyId { get; set; }
        public string? PartyCode { get; set; }
        public string? PartyName { get; set; }
        public int? PartyZoneId { get; set; }
        public MiscMaster? ZoneType { get; set; } = null!;
        public int? RegistrationTypeId { get; set; }
        public MiscMaster RegistrationType { get; set; } = null!;
        public string? GSTNumber { get; set; }
        public int? GSTStateCode { get; set; }
        public string? PAN { get; set; }
        public string? Website { get; set; }
        public string? TAN { get; set; }
        public int? TDSCategoryId { get; set; }
        public int? MSMETypeId { get; set; }
        public MiscMaster? MSMETypeMisc { get; set; } = null!;
        public string? MSMENO { get; set; }
        public DateTimeOffset? MSMEValidUpto { get; set; }
        public bool IsMsmeCompliant { get; set; }
        public bool IsTDSApplicable { get; set; }
        public bool IsTCSApplicable { get; set; }
        public bool IsGstReverseCharge { get; set; }
        public bool Is206AB206CCAApplicable { get; set; }
        public int? PayementModeId { get; set; }
        public MiscMaster? PaymentModeTypeMisc { get; set; } = null!;
        public string? FavourOf { get; set; }
        public int? PreferredCurrencyPurchase { get; set; }
        public int? CreditDays { get; set; }
        public int? DueDateTypeId { get; set; }
        public MiscMaster? DueDateTypeMisc { get; set; } = null!;
        public int? LeadTime { get; set; }
        public int? PreferredCurrencySale { get; set; }
        public decimal? CreditLimit { get; set; }
        public int? SellingPriceListId { get; set; }
        public int? CustomerTypeId { get; set; }
        public bool IsInternalSupplier { get; set; }
        public bool IsInternalCustomer { get; set; }
        public bool IsStopPayment { get; set; }
        public MiscMaster? CustomerTypeMisc { get; set; } = null!;
        public ICollection<PartyContact>? PartyContactTypes { get; set; }
        public ICollection<PartyAddress>? PartyAddressTypes { get; set; }
        public ICollection<PartyType>? PartyTypes { get; set; }
        public ICollection<PartyDocument>? PartyDocumentTypes { get; set; }
        public ICollection<PartyBank>? PartyBankTypes { get; set; }
        
    }
}