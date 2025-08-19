using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IPartyMaster;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Icao;
using PartyManagement.Infrastructure.Data;

namespace PartyManagement.Infrastructure.Repositories.PartyMaster
{
    public class PartyMasterCommandRepository : IPartyMasterCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public PartyMasterCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<int> CreateAsync(Core.Domain.Entities.PartyMaster partyMaster)
        {
            // Add main PartyMaster
            await _applicationDbContext.PartyMaster.AddAsync(partyMaster);

            // EF will automatically save non-null child collections
            await _applicationDbContext.SaveChangesAsync();

            return partyMaster.Id; ;
        }

        public async Task<bool> DeleteAsync(int Id, Core.Domain.Entities.PartyMaster partyMaster)
        {
             // Fetch the PartyMaster to delete from the database
            var partymasterToDelete = await _applicationDbContext.PartyMaster.FirstOrDefaultAsync(u => u.Id == Id);

            // If the PartyMaster does not exist
            if (partymasterToDelete is null)
            {
                return false; //indicate failure
            }

            // Update the IsActive status to indicate deletion (or soft delete)
            partymasterToDelete.IsDeleted = partyMaster.IsDeleted;

            // Save changes to the database 
            return await _applicationDbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteFileDetailsDocumentAsync(int id, int partyId, string fileName)
        {
            var entity = await _applicationDbContext.PartyDocument
                .FirstOrDefaultAsync(x => x.Id == id && x.PartyId == partyId && x.FileName == fileName);

            if (entity == null)
                return false;

            _applicationDbContext.PartyDocument.Remove(entity);
            await _applicationDbContext.SaveChangesAsync();
            return true;
        }


        public async Task<string> GetNextPartyCodeAsync()
        {
            var lastCode = await _applicationDbContext.PartyMaster
                .OrderByDescending(p => p.PartyCode)
                .Select(p => p.PartyCode)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(lastCode))
                return "P0001";

            int lastNumber = int.Parse(lastCode.Substring(1));
            return $"P{(lastNumber + 1).ToString("D4")}";
        }

        public async Task<List<int>> GetPartyDocumentIdsAsync(int partyId)
        {
            return await _applicationDbContext.PartyDocument
                .Where(d => d.PartyId == partyId)
                .Select(d => d.DocumentId)
                .ToListAsync();
        }


        public async Task<bool> UpdateAsync(int Id, Core.Domain.Entities.PartyMaster partyMaster)
        {
            var existingParty = await _applicationDbContext.PartyMaster
            .Include(p => p.PartyTypes)
            .Include(p => p.PartyContactTypes)
            .Include(p => p.PartyAddressTypes)
            .Include(p => p.PartyBankTypes)
            .Include(p => p.PartyDocumentTypes)
            .FirstOrDefaultAsync(p => p.Id == Id);

            if (existingParty == null)
                return false;

            // Update the existing PartyMaster properties

            existingParty.PartyName = partyMaster.PartyName;
            existingParty.PartyZoneId = partyMaster.PartyZoneId;
            existingParty.RegistrationTypeId = partyMaster.RegistrationTypeId;
            existingParty.GSTNumber = partyMaster.GSTNumber;
            existingParty.GSTStateCode = partyMaster.GSTStateCode;
            existingParty.PAN = partyMaster.PAN;
            existingParty.Website = partyMaster.Website;
            existingParty.TAN = partyMaster.TAN;
            existingParty.TDSCategoryId = partyMaster.TDSCategoryId;
            existingParty.MSMETypeId = partyMaster.MSMETypeId;
            existingParty.MSMENO = partyMaster.MSMENO;
            existingParty.MSMEValidUpto = partyMaster.MSMEValidUpto;
            existingParty.IsMsmeCompliant = partyMaster.IsMsmeCompliant;
            existingParty.IsTDSApplicable = partyMaster.IsTDSApplicable;
            existingParty.IsTCSApplicable = partyMaster.IsTCSApplicable;
            existingParty.IsGstReverseCharge = partyMaster.IsGstReverseCharge;
            existingParty.Is206AB206CCAApplicable = partyMaster.Is206AB206CCAApplicable;
            existingParty.PayementModeId = partyMaster.PayementModeId;
            existingParty.FavourOf = partyMaster.FavourOf;
            existingParty.PreferredCurrencyPurchase = partyMaster.PreferredCurrencyPurchase;
            existingParty.CreditLimit = partyMaster.CreditLimit;
            existingParty.SellingPriceListId = partyMaster.SellingPriceListId;
            existingParty.CustomerTypeId = partyMaster.CustomerTypeId;
            existingParty.IsInternalCustomer = partyMaster.IsInternalCustomer;
            existingParty.IsInternalSupplier = partyMaster.IsInternalSupplier;
            existingParty.IsStopPayment = partyMaster.IsStopPayment;
            existingParty.IsActive = partyMaster.IsActive;


            // PartyTypes - Update if exists, else Insert
            
            if (partyMaster.PartyTypes != null)
            {
                foreach (var incoming in partyMaster.PartyTypes)
                {
                    if (incoming.Id > 0 && incoming.PartyId > 0)
                    {
                        var existingChild = existingParty.PartyTypes
                            .FirstOrDefault(pt => pt.Id == incoming.Id && pt.PartyId == Id);

                        if (existingChild != null)
                        {
                            existingChild.PartyGroupId = incoming.PartyGroupId;
                            existingChild.PartyTypeId = incoming.PartyTypeId;
                        }
                    }
                    else // New record (Id == 0)
                    {
                        existingParty.PartyTypes.Add(new PartyType
                        {
                            PartyId = existingParty.Id,
                            PartyTypeId = incoming.PartyTypeId,
                            PartyGroupId = incoming.PartyGroupId
                        });
                    }
                }
            }

        

           // Partycontacts - Update if exists, else Insert
            if (partyMaster.PartyContactTypes != null)
            {
                foreach (var incoming in partyMaster.PartyContactTypes)
                {
                    if (incoming.Id > 0 && incoming.PartyId > 0)
                    {
                        var existingChild = existingParty.PartyContactTypes
                            .FirstOrDefault(pt => pt.Id == incoming.Id && pt.PartyId == Id);

                        if (existingChild != null)
                        {
                            existingChild.FirstName = incoming.FirstName;
                            existingChild.LastName = incoming.LastName;
                            existingChild.GenderId = incoming.GenderId;
                            existingChild.Designation = incoming.Designation;
                            existingChild.EmailID = incoming.EmailID;
                            existingChild.MobileNo = incoming.MobileNo;
                            existingChild.Phone = incoming.Phone;
                            existingChild.PreferredChannelId = incoming.PreferredChannelId;
                            existingChild.ContactTypeId = incoming.ContactTypeId;
                            existingChild.ContactBy = incoming.ContactBy;
                        }
                    }
                    else // New record (Id == 0)
                    {
                        existingParty.PartyContactTypes.Add(new PartyContact
                        {
                            PartyId = existingParty.Id,
                            FirstName = incoming.FirstName,
                            LastName = incoming.LastName,
                            GenderId = incoming.GenderId,
                            Designation = incoming.Designation,
                            EmailID = incoming.EmailID,
                            MobileNo = incoming.MobileNo,
                            Phone = incoming.Phone,
                            PreferredChannelId = incoming.PreferredChannelId,
                            ContactTypeId = incoming.ContactTypeId,
                            ContactBy = incoming.ContactBy
                        });
                    }
                }
            }

           // PartyAddresses - Update if exists, else Insert
        if (partyMaster.PartyAddressTypes != null)
        {
            foreach (var incoming in partyMaster.PartyAddressTypes)
            {
                    if (incoming.Id > 0 && incoming.PartyId > 0)
                    {
                        var existingChild = existingParty.PartyAddressTypes
                            .FirstOrDefault(pt => pt.Id == incoming.Id && pt.PartyId == Id);

                        if (existingChild != null)
                        {
                            existingChild.AddressType = incoming.AddressType;
                            existingChild.AddressLine1 = incoming.AddressLine1;
                            existingChild.AddressLine2 = incoming.AddressLine2;
                            existingChild.City = incoming.City;
                            existingChild.State = incoming.State;
                            existingChild.PostalCode = incoming.PostalCode;
                            existingChild.Country = incoming.Country;

                        }
                    }
                    else // New record (Id == 0)
                    {
                        existingParty.PartyAddressTypes.Add(new PartyAddress
                        {
                            PartyId = existingParty.Id,
                            AddressType = incoming.AddressType,
                            AddressLine1 = incoming.AddressLine1,
                            AddressLine2 = incoming.AddressLine2,
                            City = incoming.City,
                            State = incoming.State,
                            PostalCode = incoming.PostalCode,
                            Country = incoming.Country
                        });
                    }
            }
        }

          // PartyBanks - Update if exists, else Insert
        if (partyMaster.PartyBankTypes != null)
        {
            foreach (var incoming in partyMaster.PartyBankTypes)
            {
                    if (incoming.Id > 0 && incoming.PartyId > 0)
                    {
                        var existingChild = existingParty.PartyBankTypes
                            .FirstOrDefault(pt => pt.Id == incoming.Id && pt.PartyId == Id);

                        if (existingChild != null)
                        {
                            existingChild.BankName = incoming.BankName;
                            existingChild.BankAccountNumber = incoming.BankAccountNumber;
                            existingChild.BankBranch = incoming.BankBranch;
                            existingChild.IFSCCode = incoming.IFSCCode;
                            existingChild.SWIFTCode = incoming.SWIFTCode;
                            existingChild.AccountTypeId = incoming.AccountTypeId;
                            existingChild.IsDefaultAccount = incoming.IsDefaultAccount;
                            existingChild.IsPrimaryAccount = incoming.IsPrimaryAccount;

                        }
                    }
                    else // New record (Id == 0)
                    {
                        existingParty.PartyBankTypes.Add(new PartyBank
                        {
                            PartyId = existingParty.Id,
                            BankName = incoming.BankName,
                            BankAccountNumber = incoming.BankAccountNumber,
                            BankBranch = incoming.BankBranch,
                            IFSCCode = incoming.IFSCCode,
                            SWIFTCode = incoming.SWIFTCode,
                            AccountTypeId = incoming.AccountTypeId,
                            IsDefaultAccount = incoming.IsDefaultAccount,
                            IsPrimaryAccount = incoming.IsPrimaryAccount
                        });
                    }
            }
        }

            // PartyDocuments - Update if exists, else Insert
            if (partyMaster.PartyDocumentTypes != null && partyMaster.PartyDocumentTypes.Any())
            {
                partyMaster.PartyDocumentTypes = partyMaster.PartyDocumentTypes
                                                .Where(d => !(d.Id == 0 &&
                                                            (d.DocumentId == 0 ||
                                                            string.IsNullOrWhiteSpace(d.FileName) ||
                                                            d.FileName == "string")))
                                                .ToList();
                // ✅ If no valid documents left, set null
                if (partyMaster.PartyDocumentTypes == null || partyMaster.PartyDocumentTypes.Count == 0)
                {
                    partyMaster.PartyDocumentTypes = null;
                }

                else
                {
                    foreach (var incoming in partyMaster.PartyDocumentTypes)
                    {

                        if (incoming.Id > 0 && incoming.PartyId > 0)
                        {
                            var existingChild = existingParty.PartyDocumentTypes
                                ?.FirstOrDefault(pt => pt.Id == incoming.Id && pt.PartyId == Id);

                            if (existingChild != null)
                            {
                                existingChild.DocumentId = incoming.DocumentId;
                                existingChild.FileName = incoming.FileName;
                            }
                        }
                        else
                        {
                            if (incoming.Id == 0 && incoming.PartyId > 0 && incoming.DocumentId > 0 && !string.IsNullOrWhiteSpace(incoming.FileName))
                            {
                                existingParty.PartyDocumentTypes.Add(new PartyDocument
                                {
                                    PartyId = existingParty.Id,
                                    DocumentId = incoming.DocumentId,
                                    FileName = incoming.FileName,
                                    UploadedDate = DateTimeOffset.Now
                                });
                            }
                        }
                    }
                }
                
        }

             _applicationDbContext.PartyMaster.Update(existingParty);

            var result = await _applicationDbContext.SaveChangesAsync();

            // success even if 0 changes (no modification detected)
            return result >= 0;
    }
    }
}