using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using IdentityModel;
using IdentityModel.Client;
using System.IdentityModel.Tokens.Jwt; // For handling JWT tokens
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using BSOFT.Domain.Entities;
using BSOFT.Domain.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BSOFT.Application.UserLogin.Commands.UserLogin
{
    public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, LoginResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public UserLoginCommandHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

       public async Task<LoginResponse> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null || !VerifyPassword(user.PasswordHash, request.Password))
            {
                return new LoginResponse
                {
                    Message = "Invalid credentials"
                };
            }

            var token = _jwtTokenGenerator.GenerateToken(user);
            return new LoginResponse
            {
                Token = token,
                Message = "Login successful"
            };
        }

        private bool VerifyPassword(string hashedPassword, string password)
        {
            // Replace this with actual password hashing comparison.
            return hashedPassword == password;
        }
    }
}
