using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;


namespace Core.Application.Common.Interfaces.IUserSession
{
    public interface IUserSessionCommandRepository
    {
        Task<Core.Domain.Entities.UserSession> CreateAsync(Core.Domain.Entities.UserSession userSession);
    }
}