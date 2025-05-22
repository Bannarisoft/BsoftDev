using Core.Application.Common.HttpResponse;
using Core.Application.Users.Commands.ChangeUserPassword;
using Core.Application.Users.Commands.CreateUser;
using Core.Application.Users.Commands.DeleteUser;
using Core.Application.Users.Commands.ForgotUserPassword;
using Core.Application.Users.Commands.ResetUserPassword;
using Core.Application.Users.Commands.UpdateFirstTimeUserPassword;
using Core.Application.Users.Commands.UpdateUser;
using Core.Application.Users.Queries.GetUserById;
using Core.Application.Users.Queries.GetUsers;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserManagement.API.Controllers;

namespace UserManagement.Tests.UnitTests.Controllers
{
    [TestClass]
    public class UserControllerExtendedTests
    {
        private Mock<ISender> _mediatorMock = null!;
        private Mock<ILogger<UserController>> _loggerMock = null!;

        [TestInitialize]
        public void Setup()
        {
            _mediatorMock = new Mock<ISender>();
            _loggerMock = new Mock<ILogger<UserController>>();
        }

        private UserController SetupController(
            IValidator<CreateUserCommand>? createValidator = null,
            IValidator<UpdateUserCommand>? updateValidator = null,
            IValidator<FirstTimeUserPasswordCommand>? firstTimeValidator = null,
            IValidator<ChangeUserPasswordCommand>? changeValidator = null,
            IValidator<ForgotUserPasswordCommand>? forgotValidator = null,
            IValidator<ResetUserPasswordCommand>? resetValidator = null)
        {
            return new UserController(
                _mediatorMock.Object,
                createValidator ?? Mock.Of<IValidator<CreateUserCommand>>(),
                updateValidator ?? Mock.Of<IValidator<UpdateUserCommand>>(),
                firstTimeValidator ?? Mock.Of<IValidator<FirstTimeUserPasswordCommand>>(),
                changeValidator ?? Mock.Of<IValidator<ChangeUserPasswordCommand>>(),
                _loggerMock.Object,
                forgotValidator ?? Mock.Of<IValidator<ForgotUserPasswordCommand>>(),
                resetValidator ?? Mock.Of<IValidator<ResetUserPasswordCommand>>()
            );
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            var updateCommand = new UpdateUserCommand { UserId = 99, UserName = "updatedUser" };

            var validator = new Mock<IValidator<UpdateUserCommand>>();
            validator.Setup(v => v.ValidateAsync(updateCommand, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new ValidationResult());

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetUserByIdQuery>(q => q.UserId == 99), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserByIdDTO?)null);

            var controller = SetupController(updateValidator: validator.Object);

            var result = await controller.UpdateAsync(updateCommand);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnUser_WhenFound()
        {
            int userId = 1;
            var expectedUser = new UserByIdDTO { UserId = userId, UserName = "foundUser" };

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetUserByIdQuery>(q => q.UserId == userId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedUser);

            var controller = SetupController();

            var result = await controller.GetByIdAsync(userId);

            result.Should().BeOfType<OkObjectResult>();
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldReturnBadRequest_WhenIdInvalid()
        {
            var controller = SetupController();

            var result = await controller.DeleteAsync(0);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldReturnOk_WhenUserDeleted()
        {
            int userId = 1;

            _mediatorMock
                .Setup(m => m.Send(It.Is<DeleteUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ApiResponseDTO<bool> { IsSuccess = true });

            var controller = SetupController();

            var result = await controller.DeleteAsync(userId);

            result.Should().BeOfType<OkObjectResult>();
        }

        [TestMethod]
        public async Task ChangePassword_ShouldReturnOk_WhenSuccessful()
        {
            var command = new ChangeUserPasswordCommand { UserName = "testUser", NewPassword = "Pass@123" };

            var validator = new Mock<IValidator<ChangeUserPasswordCommand>>();
            validator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new ValidationResult());

            _mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ApiResponseDTO<string> { IsSuccess = true, Message = "Password changed" });

            var controller = SetupController(changeValidator: validator.Object);

            var result = await controller.ChangePassword(command);

            result.Should().BeOfType<OkObjectResult>();
        }

        [TestMethod]
        public async Task FirstTimeUserChangePassword_ShouldReturnOk_WhenSuccessful()
        {
            var command = new FirstTimeUserPasswordCommand
            {
                UserName = "firstuser",
                Password = "First@123"
            };

            var validator = new Mock<IValidator<FirstTimeUserPasswordCommand>>();
            validator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new ValidationResult());

            _mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ApiResponseDTO<string> { IsSuccess = true });

            var controller = SetupController(firstTimeValidator: validator.Object);

            var result = await controller.FirstTimeUserChangePassword(command);

            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
