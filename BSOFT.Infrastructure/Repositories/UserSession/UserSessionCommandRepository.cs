using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Infrastructure.Data;
using Core.Application.Common.Interfaces.IUserSession;

namespace BSOFT.Infrastructure.Repositories.UserSession
{
    public class UserSessionCommandRepository : IUserSessionCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        
        public UserSessionCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task<Core.Domain.Entities.UserSession> CreateAsync(Core.Domain.Entities.UserSession userSession)
        {
            await _applicationDbContext.UserSession.AddAsync(userSession);
            await _applicationDbContext.SaveChangesAsync();
            return userSession;
        }
    }
}