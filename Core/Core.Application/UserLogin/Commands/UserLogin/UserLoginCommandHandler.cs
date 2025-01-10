using System.Security.Cryptography;
using System.Text;
using Core.Application.Common.Interfaces;
using MediatR;
using Core.Application.Common.Interfaces.IUser;

namespace Core.Application.UserLogin.Commands.UserLogin
{
    public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, LoginResponse>
    {
        private readonly IUserCommandRepository _userRepository;
        private readonly IUserQueryRepository _userQueryRepository;
                private readonly IJwtTokenHelper  _jwtTokenHelper;

        public UserLoginCommandHandler(IUserCommandRepository userRepository,  IJwtTokenHelper jwtTokenHelper, IUserQueryRepository userQueryRepository)
        {
            _userRepository = userRepository;
            _userQueryRepository = userQueryRepository;
            _jwtTokenHelper = jwtTokenHelper;
            
        }

       public async Task<LoginResponse> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            
            var user = await _userQueryRepository.GetByUsernameAsync(request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new LoginResponse
                {
                    IsAuthenticated = false,
                    IsFirstTimeUser = false,
                    Message = "Invalid username or password."
                };
                // throw new UnauthorizedAccessException("Invalid username or password.");
            }

             // Get user roles
            var roles = await _userQueryRepository.GetUserRolesAsync(user.UserId);

            // Generate JWT token
            var token = _jwtTokenHelper.GenerateToken(user.UserName, roles);

            return new LoginResponse
            {
                Token = token,
                UserName = user.UserName,
                UserRole = roles,
                IsAuthenticated = true,
                IsFirstTimeUser = user.IsFirstTimeUser,
                Message = "Login Successful."
            };

        }

        private bool VerifyPassword(string password, string storedHash)
        {
            using var sha256 = SHA256.Create();
            var hashedPassword = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
            return hashedPassword == storedHash;
        }
    }
}
