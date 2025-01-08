using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using IdentityModel;
using IdentityModel.Client;
using System.IdentityModel.Tokens.Jwt; // For handling JWT tokens
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Common.Interfaces.IUserPasswordNotifications;
using System.Runtime.ConstrainedExecution;
using DnsClient.Protocol;
using Core.Application.UserSession.Command;
using Core.Application.UserSession.Queries.GetUserSession;
using Microsoft.AspNetCore.Http;

namespace Core.Application.UserLogin.Commands.UserLogin
{
    public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, LoginResponse>
    {
        private readonly IUserCommandRepository _userRepository;
        private readonly IUserQueryRepository _userQueryRepository;
        private readonly IUserPwdNotificationsQueryRepository _userPwdNotificationsQueryRepository;
        private readonly IRequestHandler<CreateUserSessionCommand, UserSessionDto> _createUserSessionCommandHandler;
        private readonly IIPAddressService _ipAddressService;
         private readonly IHttpContextAccessor _httpContextAccessor;

        
        // private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IJwtTokenHelper  _jwtTokenHelper;

        public UserLoginCommandHandler(IUserCommandRepository userRepository,  IJwtTokenHelper jwtTokenHelper, IUserQueryRepository userQueryRepository,IUserPwdNotificationsQueryRepository userPwdNotificationsQueryRepository,IRequestHandler<CreateUserSessionCommand, UserSessionDto> createUserSessionCommandHandler,IIPAddressService ipAddressService,IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _userQueryRepository = userQueryRepository;
            // _jwtTokenGenerator = jwtTokenGenerator;
            _jwtTokenHelper = jwtTokenHelper;
            _userPwdNotificationsQueryRepository=userPwdNotificationsQueryRepository;
            _createUserSessionCommandHandler = createUserSessionCommandHandler;
            _ipAddressService = ipAddressService;
            _httpContextAccessor = httpContextAccessor;
          
        }

       public async Task<LoginResponse> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userQueryRepository.GetByUsernameAsync(request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }
             // Get user roles
            var roles = await _userQueryRepository.GetUserRolesAsync(user.UserId);

            // Generate JWT token
            var token = _jwtTokenHelper.GenerateToken(user.UserName, roles);
            
            // Call the CreateUserSessionCommandHandler
            var createUserSessionCommand = new CreateUserSessionCommand
            {
                UserId = user.UserId,
                UserName =user.UserName,
                SessionId =  Guid.NewGuid().ToString(),
                Token = token,
                CreatedAt =DateTime.UtcNow,
                IsActive = 1,
                Browser = _ipAddressService.GetUserBrowserDetails(_httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString()),
                CreatedIP =_ipAddressService.GetSystemIPAddress(),
                Status = "I"
            };
            
            var userSessionDto = await _createUserSessionCommandHandler.Handle(createUserSessionCommand, cancellationToken);


            // Get password last change date from PasswordLastChange table
             var passwordLastChange = await _userPwdNotificationsQueryRepository.GetLastPasswordChangeDate(user.UserName.Trim());

           //Check if password has expired
            var (pwdExpiryDays, pwdExpiryAlertDays) = await _userPwdNotificationsQueryRepository.GetPasswordExpiryDays();

            var passwordAge = (DateTime.Now - passwordLastChange).Value.Days;

            if (passwordLastChange == null)
            {
                 return new LoginResponse
               { 
                    Token = token,
                    UserName = user.UserName,
                    UserRole = roles,
                    Message = "No Logs For Password Change Detected.",
                    

               };
                
            }

            else if (passwordAge >= pwdExpiryDays)
            {
            // Password has expired, prompt user to change password
                return new LoginResponse
               { 
                      Token = token,
                      UserName = user.UserName,
                      UserRole = roles,
                      Message = "Your password has expired. Please update your password to regain access."
               };
            }
            // Password is near expiry, send notification to user
            else if (passwordAge >= pwdExpiryDays - pwdExpiryAlertDays)
            {   
                int daysLeft = pwdExpiryDays - passwordAge;
                return new LoginResponse
                {

                Token = token,
                UserName = user.UserName,
                UserRole = roles,
                Message = $"Your password will expire in {daysLeft} days. Please update your password to avoid any disruptions."
                };
            }
            else
            {
                return new LoginResponse
            {
                Token = token,
                UserName = user.UserName,
                UserRole = roles,
                Message = "Login Successful."
                
            };


           
            }



        // var user = await _userRepository.GetByUsernameAsync(request.Username);

        // if (user == null || !VerifyPassword(user.PasswordHash, request.Password))
        // {
        //     throw new UnauthorizedAccessException("Invalid Username or Password");
        // }

        // if (user.UserRole == null || string.IsNullOrEmpty(user.UserRole.RoleName))
        // {
        //     throw new UnauthorizedAccessException("User role is not assigned.");
        // }

        // var token = _jwtTokenGenerator.GenerateToken(user);

        // return new LoginResponse
        // {
        //     UserName = user.UserName,
        //     UserRole = user.UserRole.RoleName,
        //     Token = token,
        //     Message = "Login successful",
        //     IsAuthenticated = true
        // };
        //     var user = await _userRepository.GetByUsernameAsync(request.Username);
        //     if (user == null || !VerifyPassword(user.PasswordHash, request.Password))
        //     {
        //         return new LoginResponse
        //         {
        //             Message = "Invalid credentials"
        //         };
        //     }

        //     var token = _jwtTokenGenerator.GenerateToken(user);
        //     return new LoginResponse
        //     {
        //         Token = token,
        //         Message = "Login successful"
        //     };
        }

        // private bool VerifyPassword(string password, string storedHash)
        // {
        //     using var sha256 = SHA256.Create();
        //     var hashedPassword = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
        //     return hashedPassword == storedHash;
        // }
    }
}
