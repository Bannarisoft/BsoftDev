using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Interfaces.External.IInvetoryManagement;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.Interfaces.IWarehouseMaster;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Infrastructure.Data;

namespace Core.Application.WarehouseMaster.Services
{
    public class WarehouseCodeGenerator : IWarehouseCodeGenerator
    {
        private readonly IUnitGrpcClient _unitGrpcClient;
        private readonly IMiscMasterGrpcClient _miscMasterGrpcClient;
        private readonly ApplicationDbContext _dbContext;
        public WarehouseCodeGenerator(IUnitGrpcClient unitGrpcClient, IMiscMasterGrpcClient miscMasterGrpcClient, ApplicationDbContext dbContext)
        {
            _unitGrpcClient = unitGrpcClient;
            _miscMasterGrpcClient = miscMasterGrpcClient;
            _dbContext = dbContext;
        }
       public async Task<string> GenerateAsync(int unitId, int warehouseTypeId)
        {
            // Get unit short name from gRPC
            var units = await _unitGrpcClient.GetAllUnitAsync();
            var unitShortName = units.FirstOrDefault(u => u.UnitId == unitId)?.ShortName ?? "00";

            // Get misc code from gRPC
            var miscMasters = await _miscMasterGrpcClient.GetMiscMasterByIdAsync("WarehouseType");
            var miscCode = miscMasters.FirstOrDefault(x => x.Id == warehouseTypeId)?.Code ?? "XX";

            // Find the last warehouse code in DB
            var lastCode = await _dbContext.WarehouseMasters
                .Where(w => w.UnitId == unitId && w.WarehouseTypeId == warehouseTypeId)
                .OrderByDescending(w => w.Id)
                .Select(w => w.WarehouseCode)
                .FirstOrDefaultAsync();

            // Calculate next number
            int newNumber = 1;
            if (!string.IsNullOrEmpty(lastCode) && lastCode.Length > 10)
            {
                var numberPart = lastCode[^4..]; // last 4 characters
                if (int.TryParse(numberPart, out int lastNumber))
                    newNumber = lastNumber + 1;
            }

            // Format: WH-<UnitShortName>-<MiscCode>-<0001>
            return $"WH-{unitShortName}-{miscCode}-{newNumber:D4}";
        }

        // public async Task<string> GenerateAsync(int unitId, int warehouseTypeId)
        // {
        //     var units = await _unitGrpcClient.GetAllUnitAsync();
        //     var unitLookup = units.ToDictionary(u => u.UnitId, u => u.ShortName);

        //     var miscMasters = await _miscMasterGrpcClient.GetMiscMasterByIdAsync("WarehouseType");
        //     var dict = miscMasters.ToDictionary(x => x.Id, x => x.Description);

        //     var lastCode = await _dbContext.WarehouseMasters
        //     .Where(w => w.UnitId == unitId && w.WarehouseTypeId == warehouseTypeId)
        //     .OrderByDescending(w => w.Id)
        //     .Select(w => w.WarehouseCode)
        //     .FirstOrDefaultAsync();

        //     int newNumber = 1;
        //     if (!string.IsNullOrEmpty(lastCode) && lastCode.Length > 10)
        //     {
        //         var numberPart = lastCode.Substring(lastCode.Length - 4);
        //         if (int.TryParse(numberPart, out int lastNumber))
        //             newNumber = lastNumber + 1;
        //     }

        //     return $"WH-{unitShortName}-{miscCode}-{newNumber:D4}";
            

        // }
    }
}