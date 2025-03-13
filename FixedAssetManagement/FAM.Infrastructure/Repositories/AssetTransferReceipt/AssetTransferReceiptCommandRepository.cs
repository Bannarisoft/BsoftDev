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

        public async Task<int> CreateAsync(AssetTransferReceiptHdr assetTransferReceiptHdr)
        {
              var entry =_applicationDbContext.Entry(assetTransferReceiptHdr);
              await _applicationDbContext.AssetTransferReceiptHdr.AddAsync(assetTransferReceiptHdr);
              await _applicationDbContext.SaveChangesAsync();

            // Update AckStatus in AssetTransferIssueHdr where AssetTransferId matches, AckStatus = 0, and Status = Approved
            var assetTransferIssueHdr = await _applicationDbContext.AssetTransferIssueHdr
                            .FirstOrDefaultAsync(x => x.Id == assetTransferReceiptHdr.AssetTransferId 
                              && x.AckStatus == 0 
                              && x.Status == "Approved");
            if (assetTransferIssueHdr != null)
            {
                assetTransferIssueHdr.AckStatus = 1;
                _applicationDbContext.AssetTransferIssueHdr.Update(assetTransferIssueHdr);
                await _applicationDbContext.SaveChangesAsync();
            }

              return assetTransferReceiptHdr.Id;
        }
    }
}