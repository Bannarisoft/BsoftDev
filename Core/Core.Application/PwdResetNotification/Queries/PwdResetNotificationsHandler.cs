using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUserPasswordNotifications;

namespace Core.Application.PwdResetNotification.Queries
{
    public class PwdResetNotificationsHandler
    {
        private readonly IUserPwdNotificationsQueryRepository _IUserPwdNotificationsQueryRepository;
        public PwdResetNotificationsHandler(IUserPwdNotificationsQueryRepository IUserPwdNotificationsQueryRepository)
        {
            _IUserPwdNotificationsQueryRepository = IUserPwdNotificationsQueryRepository;
        }

        public async Task Handle(PwdResetNotifications request, CancellationToken cancellationToken)
        {
            var username = request.Username;
            if(username == null)
            {
                throw new ArgumentNullException(nameof(username));
            }

            // Get password last change date from PasswordLastChange table
            var lastPasswordChangeDate = await _IUserPwdNotificationsQueryRepository.GetLastPasswordChangeDate(username.Trim());
            //Check if password has expired
            if (lastPasswordChangeDate == null)
            {
               // handle case where no password change date is found
            }
            else
            {
            var (pwdExpiryDays, pwdExpiryAlertDays) = await _IUserPwdNotificationsQueryRepository.GetPasswordExpiryDays();
            var passwordAge = (DateTime.Now - lastPasswordChangeDate).Value.Days;
            if (passwordAge >= pwdExpiryDays)
            {
            // handle case where password has expired
            
            }
            else if (passwordAge >= pwdExpiryDays - pwdExpiryAlertDays)
            {
            int daysLeft = pwdExpiryDays - passwordAge;
            // handle case where password is near expiry
            }
            else
            {
            // handle case where password is not near expiry
            }   
    }
        }
    }
}