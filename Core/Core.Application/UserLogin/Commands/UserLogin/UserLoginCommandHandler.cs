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

namespace Core.Application.UserLogin.Commands.UserLogin
{
    public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, LoginResponse>
    {
        private readonly IUserRepository _userRepository;
        
        // private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IJwtTokenHelper  _jwtTokenHelper;

        public UserLoginCommandHandler(IUserRepository userRepository,  IJwtTokenHelper jwtTokenHelper)
        {
            _userRepository = userRepository;
            // _jwtTokenGenerator = jwtTokenGenerator;
            _jwtTokenHelper = jwtTokenHelper;
        }

       public async Task<LoginResponse> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            // Get user roles
            var roles = await _userRepository.GetUserRolesAsync(user.UserId);

            // Generate JWT token
            var token = _jwtTokenHelper.GenerateToken(user.UserName, roles);

            return new LoginResponse
            {
                Token = token,
                UserName = user.UserName,
                UserRole = roles
            };
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
