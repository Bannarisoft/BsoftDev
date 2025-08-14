using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using Core.Domain.Events;                                       // AuditLogsDomainEvent
using Core.Domain.Entities;                                     // Location (domain entity)
using Core.Application.Common.Interfaces.ILocation;             // ILocationQueryRepository
using Core.Application.Location.Queries.GetLocationAutoComplete; // Handler + Query
using Core.Application.Location.Queries.GetLocations;           // LocationAutoCompleteDto

namespace FixedAssetManagement.Tests.UnitTests.Handlers.Queries.Locations
{
    [TestClass]
    public class GetLocationAutoCompleteQueryHandlerTests
    {
        [TestMethod]
        public async Task Handle_Found_MapsAndPublishes_ReturnsDtos()
        {
            // Arrange
            var repo     = new Mock<ILocationQueryRepository>(MockBehavior.Strict);
            var mapper   = new Mock<IMapper>(MockBehavior.Strict);
            var mediator = new Mock<IMediator>(MockBehavior.Strict);

            var search = "main";
            var domainList = new List<Location>
            {
                new Location { Id = 1, Code = "LOC-001", LocationName = "Main Store" },
                new Location { Id = 2, Code = "LOC-002", LocationName = "Main Office" }
            };

            repo.Setup(r => r.GetLocation(search))
                .ReturnsAsync(domainList);

            var mapped = new List<LocationAutoCompleteDto>
            {
                new LocationAutoCompleteDto { Id = 1, LocationName = "Main Store" },
                new LocationAutoCompleteDto { Id = 2, LocationName = "Main Office" }
            };

            mapper.Setup(m => m.Map<List<LocationAutoCompleteDto>>(domainList))
                  .Returns(mapped);

            mediator.Setup(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var sut = new GetLocationAutoCompleteQueryHandler(repo.Object, mediator.Object, mapper.Object);

            // Act
            var result = await sut.Handle(new GetLocationAutoCompleteQuery { SearchPattern = search }, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Main Store", result[0].LocationName);
            Assert.AreEqual("Main Office", result[1].LocationName);

            repo.Verify(r => r.GetLocation(search), Times.Once);
            mapper.Verify(m => m.Map<List<LocationAutoCompleteDto>>(domainList), Times.Once);
            mediator.Verify(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()), Times.Once);

            repo.VerifyNoOtherCalls();
            mapper.VerifyNoOtherCalls();
            mediator.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Handle_NoResults_ThrowsValidationException_DoesNotPublish()
        {
            // Arrange
            var repo     = new Mock<ILocationQueryRepository>(MockBehavior.Strict);
            var mapper   = new Mock<IMapper>(MockBehavior.Strict);
            var mediator = new Mock<IMediator>(MockBehavior.Strict);

            var search = "zzz";

            // Return empty list to trigger the "no results" branch
            repo.Setup(r => r.GetLocation(search))
                .ReturnsAsync(new List<Location>());

            var sut = new GetLocationAutoCompleteQueryHandler(repo.Object, mediator.Object, mapper.Object);

            // Act + Assert
            var ex = await Assert.ThrowsExceptionAsync<ValidationException>(() =>
                sut.Handle(new GetLocationAutoCompleteQuery { SearchPattern = search }, CancellationToken.None));

            StringAssert.Contains(ex.Message.ToLowerInvariant(), "no location");

            repo.Verify(r => r.GetLocation(search), Times.Once);

            // No mapping or publish should occur
            mapper.VerifyNoOtherCalls();
            mediator.VerifyNoOtherCalls();
            repo.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Handle_NullResults_ThrowsValidationException_DoesNotPublish()
        {
            // Arrange
            var repo     = new Mock<ILocationQueryRepository>(MockBehavior.Strict);
            var mapper   = new Mock<IMapper>(MockBehavior.Strict);
            var mediator = new Mock<IMediator>(MockBehavior.Strict);

            var search = "none";

            // Return null to trigger the other guard
            repo.Setup(r => r.GetLocation(search))
                .ReturnsAsync((List<Location>?)null);

            var sut = new GetLocationAutoCompleteQueryHandler(repo.Object, mediator.Object, mapper.Object);

            // Act + Assert
            var ex = await Assert.ThrowsExceptionAsync<ValidationException>(() =>
                sut.Handle(new GetLocationAutoCompleteQuery { SearchPattern = search }, CancellationToken.None));

            StringAssert.Contains(ex.Message.ToLowerInvariant(), "no location");

            repo.Verify(r => r.GetLocation(search), Times.Once);
            mapper.VerifyNoOtherCalls();
            mediator.VerifyNoOtherCalls();
            repo.VerifyNoOtherCalls();
        }
    }
}
