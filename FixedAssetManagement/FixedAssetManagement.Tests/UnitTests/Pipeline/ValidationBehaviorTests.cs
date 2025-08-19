// File: FixedAssetManagement.Tests/UnitTests/Pipeline/ValidationBehaviorTests.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Uses your behavior from Application layer
using Core.Application.Common.Behaviors;

namespace FixedAssetManagement.Tests.UnitTests.Pipeline
{
    // Sample request types only for testing the behavior
    public record CreateWarehouseCommand(string Code, string Name) : IRequest<string>;
    public record NoRuleCommand(string Payload) : IRequest<int>;

    // Validators used by the tests
    public class CreateWarehouseSyncValidator : AbstractValidator<CreateWarehouseCommand>
    {
        public CreateWarehouseSyncValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        }
    }

    // Async validator to exercise ValidateAsync path
    public class CreateWarehouseAsyncValidator : AbstractValidator<CreateWarehouseCommand>
    {
        public CreateWarehouseAsyncValidator()
        {
            RuleFor(x => x.Code).MustAsync(async (_, ct) =>
            {
                await Task.Delay(1, ct); // simulate I/O
                return true;             // allow; just to exercise async
            });
        }
    }

    // Additional validator to produce another failure for aggregation test
    public class CreateWarehouseLengthValidator : AbstractValidator<CreateWarehouseCommand>
    {
        public CreateWarehouseLengthValidator()
        {
            RuleFor(x => x.Name).MinimumLength(3).WithMessage("Name too short.");
        }
    }

    [TestClass]
    public class ValidationBehaviorTests
    {
        [TestMethod]
        public async Task Valid_Request_Calls_Next_And_Returns_Result()
        {
            // Arrange
            var validators = new IValidator<CreateWarehouseCommand>[]
            {
                new CreateWarehouseSyncValidator(),
                new CreateWarehouseAsyncValidator()
            };

            var behavior = new ValidationBehavior<CreateWarehouseCommand, string>(validators);

            var nextCalled = 0;
            RequestHandlerDelegate<string> next = () =>
            {
                nextCalled++;
                return Task.FromResult("OK");
            };

            var request = new CreateWarehouseCommand("WH1", "Main Warehouse");

            // Act
            var result = await behavior.Handle(request, next, CancellationToken.None);

            // Assert
            Assert.AreEqual(1, nextCalled, "Next delegate should be called exactly once.");
            Assert.AreEqual("OK", result);
        }

        [TestMethod]
        public async Task Invalid_Request_Throws_ValidationException_And_Does_Not_Call_Next()
        {
            // Arrange
            var validators = new IValidator<CreateWarehouseCommand>[]
            {
                new CreateWarehouseSyncValidator(),
                new CreateWarehouseLengthValidator()
            };

            var behavior = new ValidationBehavior<CreateWarehouseCommand, string>(validators);

            var nextCalled = 0;
            RequestHandlerDelegate<string> next = () =>
            {
                nextCalled++;
                return Task.FromResult("SHOULD_NOT_RUN");
            };

            // Code and Name invalid (empty) ⇒ multiple failures
            var request = new CreateWarehouseCommand("", "");

            // Act + Assert
            var ex = await Assert.ThrowsExceptionAsync<ValidationException>(async () =>
                await behavior.Handle(request, next, CancellationToken.None));

            Assert.AreEqual(0, nextCalled, "Next delegate must NOT be called when validation fails.");
            Assert.IsTrue(ex.Errors.Any(), "ValidationException should contain failures.");
            Assert.IsTrue(ex.Errors.Any(f => f.PropertyName == nameof(CreateWarehouseCommand.Code)));
            Assert.IsTrue(ex.Errors.Any(f => f.PropertyName == nameof(CreateWarehouseCommand.Name)));
        }

        [TestMethod]
        public async Task Multiple_Validators_Aggregate_All_Failures()
        {
            // Arrange
            var validators = new IValidator<CreateWarehouseCommand>[]
            {
                new CreateWarehouseSyncValidator(),  // NotEmpty for Code/Name
                new CreateWarehouseLengthValidator() // Name min length 3
            };
            var behavior = new ValidationBehavior<CreateWarehouseCommand, string>(validators);

            var request = new CreateWarehouseCommand("WH1", ""); // Code ok, Name empty (2 rules will hit)

            // Act
            var ex = await Assert.ThrowsExceptionAsync<ValidationException>(async () =>
                await behavior.Handle(request, () => Task.FromResult("OK"), CancellationToken.None));

            // Assert: at least two distinct errors for Name
            var nameErrors = ex.Errors.Where(e => e.PropertyName == nameof(CreateWarehouseCommand.Name)).ToList();
            Assert.IsTrue(nameErrors.Count >= 2, "Expected aggregated failures for Name from multiple validators.");
        }

        [TestMethod]
        public async Task No_Validators_Passes_Request_Through()
        {
            // Arrange
            var behavior = new ValidationBehavior<NoRuleCommand, int>(Enumerable.Empty<IValidator<NoRuleCommand>>());

            var nextCalled = 0;
            RequestHandlerDelegate<int> next = () =>
            {
                nextCalled++;
                return Task.FromResult(42);
            };

            var request = new NoRuleCommand("anything");

            // Act
            var result = await behavior.Handle(request, next, CancellationToken.None);

            // Assert
            Assert.AreEqual(1, nextCalled);
            Assert.AreEqual(42, result);
        }

        [TestMethod]
        public async Task CancellationToken_Is_Propagated_To_Next()
        {
            // Arrange
            var validators = new IValidator<CreateWarehouseCommand>[] { new CreateWarehouseSyncValidator() };
            var behavior = new ValidationBehavior<CreateWarehouseCommand, string>(validators);

            using var cts = new CancellationTokenSource();
            var capturedToken = default(CancellationToken?);

            RequestHandlerDelegate<string> next = () =>
            {
                capturedToken = cts.Token;
                return Task.FromResult("OK");
            };

            var request = new CreateWarehouseCommand("WH1", "Main");

            // Act
            var result = await behavior.Handle(request, next, cts.Token);

            // Assert
            Assert.AreEqual("OK", result);
            Assert.IsTrue(capturedToken.HasValue, "Next should have been invoked.");
        }

        [TestMethod]
        public async Task Async_Validator_Path_Does_Not_Block_And_Allows_Valid_Request()
        {
            // Arrange
            var validators = new IValidator<CreateWarehouseCommand>[] { new CreateWarehouseAsyncValidator() };
            var behavior = new ValidationBehavior<CreateWarehouseCommand, string>(validators);

            var request = new CreateWarehouseCommand("WH2", "Secondary");

            // Act
            var result = await behavior.Handle(request, () => Task.FromResult("OK"), CancellationToken.None);

            // Assert
            Assert.AreEqual("OK", result);
        }
    }
}
