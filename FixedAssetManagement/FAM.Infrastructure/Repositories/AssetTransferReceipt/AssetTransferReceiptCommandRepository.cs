using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IAssetTransferReceipt;
using Core.Domain.Entities.AssetMaster;
using FAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FAM.Infrastructure.Repositories.AssetTransferReceipt
{
    public class AssetTransferReceiptCommandRepository : IAssetTransferReceiptCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public AssetTransferReceiptCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<int> CreateAsync(AssetTransferReceiptHdr assetTransferReceiptHdr,AssetTransferIssueHdr assetTransferIssueHdr,List<Core.Domain.Entities.AssetMaster.AssetLocation> assetLocation)
        {
            var existingRecord = await _applicationDbContext.AssetTransferIssueHdr
                .FirstOrDefaultAsync(x => x.Id == assetTransferIssueHdr.Id && x.AckStatus == 0 && x.Status == "Approved");

                // Insert into AssetTransferReceiptHdr table
                var entry =_applicationDbContext.Entry(assetTransferReceiptHdr);
                await _applicationDbContext.AssetTransferReceiptHdr.AddAsync(assetTransferReceiptHdr);

                 if (existingRecord != null)
                 {
                     existingRecord.AckStatus = assetTransferIssueHdr.AckStatus;
                     _applicationDbContext.AssetTransferIssueHdr.Update(existingRecord);
                 }

                // Retrieve all AssetLocations that need to be updated based on the provided list
                var assetIds = assetLocation.Select(a => a.AssetId).ToList();
                var assetLocationsToUpdate = await _applicationDbContext.AssetLocations
                    .Where(x => assetIds.Contains(x.AssetId))
                    .ToListAsync();

                if (assetLocationsToUpdate.Any()) // Ensure there are records to update
                {
                    foreach (var assetLocationToUpdate in assetLocationsToUpdate)
                    {
                        var newLocation = assetLocation.FirstOrDefault(a => a.AssetId == assetLocationToUpdate.AssetId);
                        if (newLocation != null)
                        {
                            assetLocationToUpdate.LocationId = newLocation.LocationId;
                            assetLocationToUpdate.SubLocationId = newLocation.SubLocationId;
                            assetLocationToUpdate.UserID = newLocation.UserID;
                            assetLocationToUpdate.UnitId = newLocation.UnitId;
                            assetLocationToUpdate.CustodianId = newLocation.CustodianId;
                            assetLocationToUpdate.DepartmentId = newLocation.DepartmentId;
                        }
                    }

                    _applicationDbContext.AssetLocations.UpdateRange(assetLocationsToUpdate);
                }

                  //  Update FixedAsset.AssetMaster's ToUnitId based on AssetId
                    var assetMasterRecords = await _applicationDbContext.AssetMasterGenerals
                     .Where(x => assetIds.Contains(x.Id))
                    .ToListAsync();
                    
                    foreach (var assetMaster in assetMasterRecords)
                    {
                        var matchingAsset = assetLocation.FirstOrDefault(a => a.AssetId == assetMaster.Id);
                        if (matchingAsset != null)
                        {
                            assetMaster.UnitId = matchingAsset.UnitId; // Update ToUnitId
                        }
                    }

                    _applicationDbContext.AssetMasterGenerals.UpdateRange(assetMasterRecords);
                    await _applicationDbContext.SaveChangesAsync();
                    return assetTransferReceiptHdr.Id;
                    
        }

    }
}