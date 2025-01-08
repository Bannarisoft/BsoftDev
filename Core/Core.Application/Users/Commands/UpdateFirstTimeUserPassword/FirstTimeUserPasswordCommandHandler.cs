using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IUser;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Users.Commands.UpdateFirstTimeUserPassword
{
    public class FirstTimeUserPasswordCommandHandler : IRequestHandler<FirstTimeUserPasswordCommand, string>
    {
        private readonly IMapper _imapper;
        private readonly IChangePassword _ichangePassword;
        private readonly IUserQueryRepository _userQueryRepository;
        public FirstTimeUserPasswordCommandHandler(IMapper imapper, IChangePassword ichangePassword, IUserQueryRepository userRepository)
        {
            _imapper = imapper;
            _ichangePassword = ichangePassword;
            _userQueryRepository = userRepository;
            
        }
        public async Task<string> Handle(FirstTimeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            //  try
            //   {
                
                var user = await _userQueryRepository.GetByUsernameAsync(request.UserName);
                
                 if ( !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                 {
                       
                      var passwordLog = _imapper.Map<PasswordLog>(request);
                      
                      passwordLog.PasswordHash = await _ichangePassword.PasswordEncode(request.Password);
                      
                      var changedPasswordLog = await _ichangePassword.ChangePassword(request.UserId,passwordLog);

                      if (changedPasswordLog != null)
                      {
                          return "Password changed successfully.";
                      }

                     return "Password change failed."; 
                 }
                 
                 return "Your input password should not match the default password. Please try a different password.";    
            //    }
            // catch (Exception ex)
            // {

            //     return $"Password change failed: {ex.Message}";
            // }
        }
    }
}