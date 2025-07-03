using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Users.Commands.RemoveVerificationCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Core.Application.Tests.Users.Commands
{
    [TestClass]
    public class RemoveVerficationCodeCommandHandlerTests
    {
        private Mock<IUserCommandRepository>? _mockUserCommandRepository;
        private RemoveVerficationCodeCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockUserCommandRepository = new Mock<IUserCommandRepository>();
            _handler = new RemoveVerficationCodeCommandHandler(_mockUserCommandRepository.Object);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnSuccess_WhenVerificationCodeRemoved()
        {
            // Arrange
            var command = new RemoveVerficationCodeCommand
            {
                UserName = "testuser"
            };

            _mockUserCommandRepository!.Setup(repo => repo.RemoveVerficationCode(command.UserName))
            .Returns(Task.FromResult(true));


            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Data);
            _mockUserCommandRepository.Verify(r => r.RemoveVerficationCode(command.UserName), Times.Once);
        }
    }
}
