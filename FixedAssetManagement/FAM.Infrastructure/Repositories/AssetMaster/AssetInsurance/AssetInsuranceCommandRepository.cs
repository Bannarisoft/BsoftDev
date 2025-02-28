using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetInsurance;
using FAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace FAM.Infrastructure.Repositories.AssetMaster.AssetInsurance
{
    public class AssetInsuranceCommandRepository  : IAssetInsuranceCommandRepository
    {
          private readonly ApplicationDbContext _dbContext;

            public AssetInsuranceCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext;
        }

        public async  Task<Core.Domain.Entities.AssetMaster.AssetInsurance> CreateAsync(Core.Domain.Entities.AssetMaster.AssetInsurance assetInsurance)
        {                           
             await _dbContext.AssetInsurance.AddAsync(assetInsurance);
                await _dbContext.SaveChangesAsync();
                return assetInsurance;
        }  
        
           public async Task<bool> UpdateAsync(int id,Core.Domain.Entities.AssetMaster.AssetInsurance assetInsurance)
        {
            var existingassetInsurance =await _dbContext.AssetInsurance.FirstOrDefaultAsync(m =>m.Id == assetInsurance.Id);
         
            if (existingassetInsurance != null)
            {
                existingassetInsurance.PolicyNo = assetInsurance.PolicyNo;
                existingassetInsurance.StartDate = assetInsurance.StartDate;   
                existingassetInsurance.Insuranceperiod = assetInsurance.Insuranceperiod;            
                existingassetInsurance.EndDate = assetInsurance.EndDate;
                existingassetInsurance.PolicyAmount = assetInsurance.PolicyAmount;
                existingassetInsurance.VendorCode = assetInsurance.VendorCode;               
                existingassetInsurance.RenewalStatus = assetInsurance.RenewalStatus;
                existingassetInsurance.RenewedDate = assetInsurance.RenewedDate;               
                existingassetInsurance.InsuranceStatus = assetInsurance.InsuranceStatus;

                _dbContext.AssetInsurance.Update(existingassetInsurance);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            return false;
        }

        
    }
}