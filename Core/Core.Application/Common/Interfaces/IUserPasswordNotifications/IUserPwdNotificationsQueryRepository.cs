using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IUserPasswordNotifications
{
    public interface IUserPwdNotificationsQueryRepository
    {
         Task<DateTime?> GetLastPasswordChangeDate (string username);
         Task<(int PwdExpiryDays, int PwdExpiryAlertDays)> GetPasswordExpiryDays();

    }
}