using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Common.Interfaces.INotifications;
using Core.Application.Users.Commands.ForgotUserPassword;
using Core.Domain.Events;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MediatR;
using Contracts.Events.Notifications;
using Core.Application.Common.Interfaces;

namespace Core.Application.Tests.Users.Commands
{
    [TestClass]
    public class ForgotUserPasswordCommandHandlerTests
    {
        private Mock<IUserQueryRepository>? _mockUserQueryRepository;
        private Mock<IChangePassword>? _mockChangePasswordService;
        private Mock<INotificationsQueryRepository>? _mockNotificationsQueryRepository;
        private Mock<IMapper>? _mockMapper;
        private Mock<IMediator>? _mockMediator;
        private Mock<ISmsService>? _mockSmsService;
        private Mock<ITimeZoneService>? _mockTimeZoneService;
        private Mock<IEmailService>? _mockEmailService;
        private Mock<IBackgroundServiceClient>? _mockBackgroundServiceClient;
        private Mock<ILogger<ForgotUserPasswordCommandHandler>>? _mockLogger;

        private ForgotUserPasswordCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockUserQueryRepository = new Mock<IUserQueryRepository>();
            _mockChangePasswordService = new Mock<IChangePassword>();
            _mockNotificationsQueryRepository = new Mock<INotificationsQueryRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockMediator = new Mock<IMediator>();
            _mockSmsService = new Mock<ISmsService>();
            _mockTimeZoneService = new Mock<ITimeZoneService>();
            _mockEmailService = new Mock<IEmailService>();
            _mockBackgroundServiceClient = new Mock<IBackgroundServiceClient>();
            _mockLogger = new Mock<ILogger<ForgotUserPasswordCommandHandler>>();

            _handler = new ForgotUserPasswordCommandHandler(
                _mockUserQueryRepository.Object,
                _mockMapper.Object,
                _mockChangePasswordService.Object,
                _mockLogger.Object,
                _mockNotificationsQueryRepository.Object,
                _mockMediator.Object,
                _mockSmsService.Object,
                _mockTimeZoneService.Object,
                _mockEmailService.Object,
                _mockBackgroundServiceClient.Object);
        }

        [TestMethod]
        public async Task Handle_ShouldSendEmailSmsAndReturnSuccessResponse()
        {
            // Arrange
            var command = new ForgotUserPasswordCommand { UserName = "testuser" };

            var user = new Core.Domain.Entities.User
            {
                UserName = "testuser",
                EmailId = "testuser@gmail.com",
                Mobile = "1234567890"
            };

            _mockUserQueryRepository
                .Setup(r => r.GetByUsernameAsync(command.UserName))
                .ReturnsAsync(user);

            _mockChangePasswordService
                .Setup(c => c.GenerateVerificationCode(It.IsAny<int>()))
                .ReturnsAsync("ABC123");

            _mockNotificationsQueryRepository
                .Setup(n => n.GetResetCodeExpiryMinutes())
                .ReturnsAsync(15);

            _mockTimeZoneService
                .Setup(tz => tz.GetSystemTimeZone())
                .Returns("UTC");

            _mockTimeZoneService
                .Setup(tz => tz.GetCurrentTime("UTC"))
                .Returns(DateTime.UtcNow);

            _mockEmailService
                .Setup(e => e.SendEmailAsync(It.IsAny<SendEmailCommand>()))
                .ReturnsAsync(true);

            _mockSmsService
                .Setup(s => s.SendSmsAsync(It.IsAny<SendSmsCommand>()))
                .ReturnsAsync(true);

            _mockBackgroundServiceClient
                .Setup(b => b.ScheduleVerificationCodeCleanupAsync(command.UserName, 15))
                .Returns(Task.CompletedTask);

            _mockMediator
                .Setup(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var userDto = new Core.Application.Users.Queries.GetUsers.UserDto
            {
                UserName = user.UserName,
                EmailId = user.EmailId,
                Mobile = user.Mobile
            };

            _mockMapper
                .Setup(m => m.Map<Core.Application.Users.Queries.GetUsers.UserDto>(user))
                .Returns(userDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual("Verification code sent to your registered email address testuser@gmail.com and mobile number 1234567890.", result.Data.Message);
            Assert.AreEqual("ABC123", result.Data.VerificationCode);

            // Verify email sent
            _mockEmailService.Verify(e => e.SendEmailAsync(It.IsAny<SendEmailCommand>()), Times.Once);

            // Verify SMS sent
            _mockSmsService.Verify(s => s.SendSmsAsync(It.IsAny<SendSmsCommand>()), Times.Once);

            // Verify background cleanup scheduled
            _mockBackgroundServiceClient.Verify(b => b.ScheduleVerificationCodeCleanupAsync(command.UserName, 15), Times.Once);

            // Verify domain event published
            _mockMediator.Verify(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
