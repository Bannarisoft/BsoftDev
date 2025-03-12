using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetTransferIssue.Command.CreateAssetTransferIssue;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetTransferIssue;
using Core.Domain.Entities.AssetMaster;
using FAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FAM.Infrastructure.Repositories.AssetMaster.AssetTransfer
{
    public class AssetTransferCommandRepository : IAssetTransferCommandRepository
    {

      private readonly ApplicationDbContext _applicationDbContext;

        public AssetTransferCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

      
            public async Task<int> CreateAssetTransferAsync(AssetTransferIssueHdr  assetTransferIssuHdr)
                {
                     var entry =_applicationDbContext.Entry(assetTransferIssuHdr);
                    await _applicationDbContext.AssetTransferIssueHdr.AddAsync(assetTransferIssuHdr);                                        
                    // 🔹 Save changes
                    await _applicationDbContext.SaveChangesAsync();                    
                    // 🔹 Return the generated ID
                    return assetTransferIssuHdr.Id;
                }
                public async Task<AssetTransferIssueDtl> CreateAssetTransferIssueAsync(AssetTransferIssueDtl  assetTransferIssueDtl)
                {
                    _applicationDbContext.AssetTransferIssueDtl.Add(assetTransferIssueDtl);
                    await _applicationDbContext.SaveChangesAsync();
                    return assetTransferIssueDtl;
                }


                public async Task<bool> UpdateAssetTransferAsync(AssetTransferIssueHdr assetTransferIssueHdr)
                {
                    // 🔹 Find the existing record
                    var existingRecord = await _applicationDbContext.AssetTransferIssueHdr   
                        .FirstOrDefaultAsync(h => h.Id == assetTransferIssueHdr.Id);

                   existingRecord.DocDate = assetTransferIssueHdr.DocDate;
                   existingRecord.TransferType = assetTransferIssueHdr.TransferType;
                   existingRecord.FromUnitId = assetTransferIssueHdr.FromUnitId;
                   existingRecord.ToUnitId = assetTransferIssueHdr.ToUnitId;
                   existingRecord.FromDepartmentId = assetTransferIssueHdr.FromDepartmentId;
                   existingRecord.ToDepartmentId = assetTransferIssueHdr.ToDepartmentId;
                   existingRecord.FromCustodianId = assetTransferIssueHdr.FromCustodianId;
                   existingRecord.ToCustodianId = assetTransferIssueHdr.ToCustodianId;
                   existingRecord.FromCustodianName = assetTransferIssueHdr.FromCustodianName;
                   existingRecord.ToCustodianName = assetTransferIssueHdr.ToCustodianName;
                   existingRecord.Status = assetTransferIssueHdr.Status;
                   existingRecord.ModifiedBy = assetTransferIssueHdr.ModifiedBy;
                   existingRecord.ModifiedDate = assetTransferIssueHdr.ModifiedDate;
                   existingRecord.ModifiedIP = assetTransferIssueHdr.ModifiedIP;
                   existingRecord.ModifiedByName = assetTransferIssueHdr.ModifiedByName;

                    _applicationDbContext.AssetTransferIssueHdr.Update(existingRecord);
                    
                    _applicationDbContext.AssetTransferIssueDtl.RemoveRange(_applicationDbContext.AssetTransferIssueDtl.Where(x => x.AssetTransferId == assetTransferIssueHdr.Id));
                    await _applicationDbContext.SaveChangesAsync();
                    
                   await _applicationDbContext.AssetTransferIssueDtl.AddRangeAsync(assetTransferIssueHdr.AssetTransferIssueDtl);
                   
                    return await _applicationDbContext.SaveChangesAsync()>0;

                    
                }
                private void UpdateAssetTransferIssueDetails(AssetTransferIssueHdr existingRecord, ICollection<AssetTransferIssueDtl> updatedDetails)
                {
                    if (updatedDetails == null)
                        return;

                    // 🔹 Get existing details from DB
                    var existingDetails = existingRecord.AssetTransferIssueDtl.ToList();

                    // 🔹 Identify and remove deleted details
                    foreach (var existingDetail in existingDetails)
                    {
                        if (!updatedDetails.Any(d => d.Id == existingDetail.Id))
                        {
                            _applicationDbContext.AssetTransferIssueDtl.Remove(existingDetail); // Remove detail
                        }
                    }

                    // 🔹 Identify and add new details
                    foreach (var newDetail in updatedDetails)
                    {
                        if (newDetail.Id == 0) // If ID is 0, it's a new entry
                        {
                            existingRecord.AssetTransferIssueDtl.Add(newDetail);
                        }
                    }
                }
    }   
}