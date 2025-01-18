using System.Security.Cryptography;
using System.Text;
using Core.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging; // This is where the ILogger interface is defined
using Core.Application.Common.Interfaces.IUser;
using Core.Domain.Events;
using Serilog;
using Core.Application.Common.HttpResponse;

namespace Core.Application.UserLogin.Commands.UserLogin
{
    public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, ApiResponseDTO<LoginResponse>>
    {
        private readonly IUserCommandRepository _userRepository;
        private readonly IUserQueryRepository _userQueryRepository;
        private readonly IJwtTokenHelper  _jwtTokenHelper;
        private readonly ILogger<UserLoginCommandHandler> _logger;
        private readonly IMediator _mediator; 


        public UserLoginCommandHandler(IUserCommandRepository userRepository,  IJwtTokenHelper jwtTokenHelper, IUserQueryRepository userQueryRepository, IMediator mediator,ILogger<UserLoginCommandHandler> logger)
        {
            _userRepository = userRepository;
            _userQueryRepository = userQueryRepository;
            _jwtTokenHelper = jwtTokenHelper;
             _mediator = mediator; 
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
        }

       public async Task<ApiResponseDTO<LoginResponse>> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling user login request for Username: {Username}", request.Username);
                        // Validate request input
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogWarning("Invalid login attempt with missing credentials.");
                return new ApiResponseDTO<LoginResponse>
                {
                    IsSuccess = false,
                    Message = "Username and password are required."
                };
            }
            // var user = await _userQueryRepository.GetByUsernameAsync(request.Username);
            // Validate request input
            // if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            // {
            //     _logger.LogWarning("Invalid login attempt with missing credentials.");
            //     return new LoginResponse
            //     {
            //         IsAuthenticated = false,
            //         Message = "Username and password are required."
            //     };
            // }
            // Fetch user details
            var user = await _userQueryRepository.GetByUsernameAsync(request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid login attempt for Username: {Username}", request.Username);
                return new ApiResponseDTO<LoginResponse>
                {
                    IsSuccess = false,
                    Message = "Invalid username or password."
                };
            }

            // if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            // {
            //     _logger.LogWarning("Invalid login attempt for Username: {Username}", request.Username);
            //     return new LoginResponse
            //     {
            //         IsAuthenticated = false,
            //         IsFirstTimeUser = false,
            //         Message = "Invalid username or password."
            //     };
            // }

            _logger.LogInformation("User {Username} found. Retrieving roles...", request.Username);
             // Get user roles
            // var roles = await _userQueryRepository.GetUserRolesAsync(user.UserId);
            // Fetch user roles
            var roles = await _userQueryRepository.GetUserRolesAsync(user.UserId);
            if (roles == null || roles.Count == 0)
            {
                _logger.LogWarning("No roles found for user {UserId}.", user.UserId);
                return new ApiResponseDTO<LoginResponse>
                {
                    IsSuccess = false,
                    Message = "User does not have any assigned roles."
                };
            }
            // var roles = await _userQueryRepository.GetUserRolesAsync(user.UserId);
            // if (roles == null || roles.Count == 0)
            // {
            //     _logger.LogWarning("No roles found for user {UserId}.", user.UserId);
            //     return new LoginResponse
            //     {
            //         IsAuthenticated = false,
            //         Message = "User does not have any assigned roles."
            //     };
            // }

            _logger.LogInformation("Roles retrieved for user {UserId}: {Roles}", user.UserId, string.Join(", ", roles));

            // Generate JWT token
            var token = _jwtTokenHelper.GenerateToken(user.UserName, roles);
            _logger.LogInformation("JWT token generated for Username: {Username}", user.UserName);
            
            //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Login",
                    actionCode: user.UserName,
                    actionName: "User logged in",
                    details: $"User '{user.UserName}' logged in successfully with roles: {token}, Roles: {roles}",
                    module:"UserLogin"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            
            //Log login event via Serilog
            Log.Information("User {UserName} logged in successfully at {Time}. Roles: {Roles}", user.UserName, DateTime.UtcNow, string.Join(", ", roles));
            return new ApiResponseDTO<LoginResponse>
            {
                IsSuccess = true,
                Message = "Login Successful.",
                Data = new LoginResponse
                {
                    Token = token,
                    UserName = user.UserName,
                    UserRole = roles,
                    IsAuthenticated = true,
                    IsFirstTimeUser = user.IsFirstTimeUser,
                    Message = "Login Successful."
                }
            };
            // return new LoginResponse
            // {
            //     Token = token,
            //     UserName = user.UserName,
            //     UserRole = roles,
            //     IsAuthenticated = true,
            //     IsFirstTimeUser = user.IsFirstTimeUser,
            //     Message = "Login Successful."
            // };

        }

        // private bool VerifyPassword(string password, string storedHash)
        // {
        //     using var sha256 = SHA256.Create();
        //     var hashedPassword = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
        //     return hashedPassword == storedHash;
        // }
    }
}
