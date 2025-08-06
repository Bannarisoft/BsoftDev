using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.ILogService;
using Core.Domain.Entities;
using PurchaseManagement.Infrastructure.Data;

namespace PurchaseManagement.Infrastructure.Repositories.LogServices
{
    public class LogServiceCommandRepository : ILogServiceCommand
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IIPAddressService _ipAddressService;
        public LogServiceCommandRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> CreateAsync(IndentLog indentLog)
        {
             _dbContext.Entry(indentLog);
            await _dbContext.IndentLog.AddAsync(indentLog);
            await _dbContext.SaveChangesAsync();

            return indentLog.Id > 0;
        }
    }
}