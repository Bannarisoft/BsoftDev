using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IPartyMaster;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using PartyManagement.Infrastructure.Data;

namespace PartyManagement.Infrastructure.Repositories.PartyMaster
{
    public class PartyMasterCommandRepository : IPartyMasterCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IIPAddressService _ipAddressService;
        public PartyMasterCommandRepository(ApplicationDbContext applicationDbContext, IIPAddressService ipAddressService)
        {
            _applicationDbContext = applicationDbContext;
            _ipAddressService = ipAddressService;
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

                      // ✅ Track changes for PartyMaster 
           await TrackChanges(existingParty, partyMaster, existingParty.Id, "PartyMaster");

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
            existingParty.GSTRegistrationDate = partyMaster.GSTRegistrationDate;
            existingParty.MSMERegistrationDate = partyMaster.MSMERegistrationDate;
            existingParty.CIN = partyMaster.CIN;
            existingParty.IECode = partyMaster.IECode;
            existingParty.IsActive = partyMaster.IsActive;

  


            // PartyTypes - Update if exists, else Insert

            if (partyMaster.PartyTypes != null)
            {
                foreach (var incoming in partyMaster.PartyTypes)
                {
                    if (incoming.Id > 0 && incoming.PartyId > 0)
                    {
                        var existingChildpartytypes = existingParty.PartyTypes
                            .FirstOrDefault(pt => pt.Id == incoming.Id && pt.PartyId == Id);

                        if (existingChildpartytypes != null)
                        {
                                   // Update Log - PartyType
                            await TrackChanges(existingChildpartytypes, incoming, existingParty.Id, "PartyType");
                            existingChildpartytypes.PartyGroupId = incoming.PartyGroupId;
                            existingChildpartytypes.PartyTypeId = incoming.PartyTypeId;

                     
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

                        // Insert Log - PartyType
                        await LogChange(existingParty.Id, "PartyType", "PartyTypeId-PartyGroupId", "", incoming.PartyTypeId + "," + incoming.PartyGroupId, "Insert");
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
                        var existingChildpartyciontact = existingParty.PartyContactTypes
                            .FirstOrDefault(pt => pt.Id == incoming.Id && pt.PartyId == Id);

                        if (existingChildpartyciontact != null)
                        {
                            // Update Log - PartyType
                            await TrackChanges(existingChildpartyciontact, incoming, existingParty.Id, "PartyContact");
                            existingChildpartyciontact.FirstName = incoming.FirstName;
                            existingChildpartyciontact.LastName = incoming.LastName;
                            existingChildpartyciontact.GenderId = incoming.GenderId;
                            existingChildpartyciontact.Designation = incoming.Designation;
                            existingChildpartyciontact.EmailID = incoming.EmailID;
                            existingChildpartyciontact.MobileNo = incoming.MobileNo;
                            existingChildpartyciontact.Phone = incoming.Phone;
                            existingChildpartyciontact.PreferredChannelId = incoming.PreferredChannelId;
                            existingChildpartyciontact.ContactTypeId = incoming.ContactTypeId;
                            existingChildpartyciontact.ContactBy = incoming.ContactBy;
                            
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

                        // Insert Log - PartyContact
                        await LogChange(existingParty.Id, "PartyContact", "ContactBy", "", incoming.ContactBy ?? string.Empty, "Insert");
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
                        var existingChildpartyaddress = existingParty.PartyAddressTypes
                            .FirstOrDefault(pt => pt.Id == incoming.Id && pt.PartyId == Id);

                        if (existingChildpartyaddress != null)
                        {
                            // Update Log - PartyAddress
                            await TrackChanges(existingChildpartyaddress, incoming, existingParty.Id, "PartyAddress");
                            existingChildpartyaddress.AddressType = incoming.AddressType;
                            existingChildpartyaddress.AddressLine1 = incoming.AddressLine1;
                            existingChildpartyaddress.AddressLine2 = incoming.AddressLine2;
                            existingChildpartyaddress.CityId = incoming.CityId;
                            existingChildpartyaddress.StateId = incoming.StateId;
                            existingChildpartyaddress.PostalCode = incoming.PostalCode;
                            existingChildpartyaddress.CountryId = incoming.CountryId;
                            

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
                            CityId = incoming.CityId,
                            StateId = incoming.StateId,
                            PostalCode = incoming.PostalCode,
                            CountryId = incoming.CountryId
                        });

                        // Insert Log - PartyAddress
                        await LogChange(existingParty.Id, "PartyAddress", "AddressType", "", incoming.AddressType ?? string.Empty, "Insert");
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
                        var existingChildpartybank = existingParty.PartyBankTypes
                            .FirstOrDefault(pt => pt.Id == incoming.Id && pt.PartyId == Id);

                        if (existingChildpartybank != null)
                        {
                              // Update Log - PartyBank
                            await TrackChanges(existingChildpartybank, incoming, existingParty.Id, "PartyBank");
                            existingChildpartybank.BankName = incoming.BankName;
                            existingChildpartybank.BankAccountNumber = incoming.BankAccountNumber;
                            existingChildpartybank.BankBranch = incoming.BankBranch;
                            existingChildpartybank.IFSCCode = incoming.IFSCCode;
                            existingChildpartybank.SWIFTCode = incoming.SWIFTCode;
                            existingChildpartybank.AccountTypeId = incoming.AccountTypeId;
                            existingChildpartybank.IsDefaultAccount = incoming.IsDefaultAccount;
                            existingChildpartybank.IsPrimaryAccount = incoming.IsPrimaryAccount;
                          

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

                        // Insert Log - PartyBank
                        await LogChange(existingParty.Id, "PartyBank", "BankName", "", incoming.BankName ?? string.Empty, "Insert");
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
                            var existingChildpartydocument = existingParty.PartyDocumentTypes
                                ?.FirstOrDefault(pt => pt.Id == incoming.Id && pt.PartyId == Id);

                            if (existingChildpartydocument != null)
                            {
                                existingChildpartydocument.DocumentId = incoming.DocumentId;
                                existingChildpartydocument.FileName = incoming.FileName;
                                // Update Log - PartyDocument
                                //await TrackChanges(existingChild, incoming, existingParty.Id, "PartyDocument");
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
                                // Insert Log - PartyDocument
                                await LogChange(existingParty.Id, "PartyDocument", "FileName", "", incoming.FileName, "Insert");
                            }
                        }
                    }
                }

            }

          //  _applicationDbContext.PartyMaster.Update(existingParty);

            var result = await _applicationDbContext.SaveChangesAsync();

            // success even if 0 changes (no modification detected)
            return result >= 0;
        }

        public async Task<bool> LogChange(int partyId, string tableName, string columnName, string oldValue, string newValue, string actionType)
        {
            var log = new PartyActivityLog
            {
                PartyId = partyId,
                TableName = tableName,
                ColumnName = columnName,
                OldValue = oldValue ?? "",
                NewValue = newValue ?? "",
                ActionType = actionType,
                ChangedBy = _ipAddressService.GetUserId(),
                ChangedByName = _ipAddressService.GetUserName(),
                ChangedIp = _ipAddressService.GetSystemIPAddress(),
                ChangedOn = DateTimeOffset.UtcNow
            };
            await _applicationDbContext.PartyActivityLog.AddAsync(log);
            
            return true;

        }
        //Track changes for PartyMaster root & child entity (all fields handled automatically)
        private async Task TrackChanges<T>(T existingEntity, T newEntity, int partyId, string tableName)
        {
            var entityType = typeof(T);
            var properties = entityType.GetProperties();

            // Fields we don't want to track (audit/system)
            var ignoreProps = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "CreatedBy", "CreatedByName", "CreatedDate", "CreatedIP",
                "ModifiedBy", "ModifiedByName", "ModifiedDate", "ModifiedIP"
            };

            foreach (var prop in properties)
            {
                //  Skip navigation properties (collections / complex entities)
                if ((typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType) 
                    && prop.PropertyType != typeof(string))
                    || (prop.PropertyType.IsClass && prop.PropertyType != typeof(string)))
                    continue;

                // Skip system/audit fields
                if (ignoreProps.Contains(prop.Name))
                    continue;

                var existingValue = prop.GetValue(existingEntity)?.ToString() ?? "";
                var newValue = prop.GetValue(newEntity)?.ToString() ?? "";

                if (existingValue != newValue)
                {
                    await LogChange(partyId, tableName, prop.Name, existingValue, newValue, "Update");
                }
            }
        }




    }
}