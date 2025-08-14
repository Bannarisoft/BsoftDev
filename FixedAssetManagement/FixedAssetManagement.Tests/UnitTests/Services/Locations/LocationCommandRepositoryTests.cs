using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FAM.Infrastructure.Data;                    // ApplicationDbContext
using FAM.Infrastructure.Repositories.Locations; // LocationCommandRepository
using Core.Domain.Entities;                      // Location
using Core.Domain.Common;                        // BaseEntity (Status, IsDelete)
using Core.Application.Common.Interfaces;        // IIPAddressService, ITimeZoneService

namespace FixedAssetManagement.Tests.UnitTests.Services.Locations
{
    [TestClass]
    public class LocationCommandRepositoryTests
    {
        private static ApplicationDbContext CreateInMemoryContext(string? dbName = null)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            // IMPORTANT: Make the audit services return non-null defaults for ANY string/DateTime members.
            var ipMock = new Mock<IIPAddressService>(MockBehavior.Loose);
            ipMock.SetReturnsDefault("127.0.0.1");                // any string on IIPAddressService -> "127.0.0.1"

            var tzMock = new Mock<ITimeZoneService>(MockBehavior.Loose);
            tzMock.SetReturnsDefault(DateTime.UtcNow);            // any DateTime on ITimeZoneService -> now
            tzMock.SetReturnsDefault("tester");                   // any string on ITimeZoneService -> "tester"

            // Build the real context with these mocks
            return new ApplicationDbContext(options, ipMock.Object, tzMock.Object);
        }

        // Helper: set audit fields before SaveChanges (in case your SaveChanges doesn't override them)
        private static void SeedAudit(Location loc, string user = "seed-user", string ip = "127.0.0.1")
        {
            loc.CreatedByName = user;
            loc.CreatedIP     = ip;
            loc.CreatedDate   = DateTime.UtcNow;
        }

        [TestMethod]
        public async Task CreateAsync_SetsNextSortOrder_AndPersists()
        {
            using var ctx = CreateInMemoryContext();
            var repo = new LocationCommandRepository(ctx);

            var existing = new Location
            {
                Code = "L-001",
                LocationName = "A",
                SortOrder = 0,
                DepartmentId = 1,
                UnitId = 1
            };
            SeedAudit(existing);
            ctx.Locations.Add(existing);
            await ctx.SaveChangesAsync();

            var toCreate = new Location
            {
                Code = "L-002",
                LocationName = "B",
                DepartmentId = 1,
                UnitId = 1
            };
            // even though SaveChanges likely overwrites, keep these non-null
            SeedAudit(toCreate);

            var created = await repo.CreateAsync(toCreate);

            Assert.IsTrue(created.Id > 0);
            Assert.AreEqual(1, created.SortOrder);                 // max(0) + 1
            Assert.AreEqual(2, await ctx.Locations.CountAsync());
        }

        [TestMethod]
        public async Task CheckForDuplicatesAsync_FindsNameAndSortOrder()
        {
            using var ctx = CreateInMemoryContext();
            var repo = new LocationCommandRepository(ctx);

            var a = new Location { Code = "L-001", LocationName = "Main",  SortOrder = 5, DepartmentId = 1, UnitId = 1 };
            var b = new Location { Code = "L-002", LocationName = "Spare", SortOrder = 7, DepartmentId = 1, UnitId = 1 };
            SeedAudit(a); SeedAudit(b);
            ctx.Locations.AddRange(a, b);
            await ctx.SaveChangesAsync();

            var (nameDup, sortDup) = await repo.CheckForDuplicatesAsync("Main", 7, excludeId: 0);

            Assert.IsTrue(nameDup);
            Assert.IsTrue(sortDup);
        }

        [TestMethod]
        public async Task UpdateAsync_UpdatesExisting_AndReturnsTrue()
        {
            using var ctx = CreateInMemoryContext();
            var repo = new LocationCommandRepository(ctx);

            var loc = new Location
            {
                Code = "L-001",
                LocationName = "Before",
                SortOrder = 1,
                DepartmentId = 1,
                UnitId = 1,
                IsActive = BaseEntity.Status.Active
            };
            SeedAudit(loc);
            ctx.Locations.Add(loc);
            await ctx.SaveChangesAsync();

            var toUpdate = new Location
            {
                Id = loc.Id,
                Code = "L-001X",
                LocationName = "After",
                SortOrder = 3,
                DepartmentId = 2,
                UnitId = 3,
                IsActive = BaseEntity.Status.Inactive
            };

            var ok = await repo.UpdateAsync(toUpdate);

            Assert.IsTrue(ok);

            var reloaded = await ctx.Locations.FirstAsync(l => l.Id == loc.Id);
            Assert.AreEqual("L-001X", reloaded.Code);
            Assert.AreEqual("After",  reloaded.LocationName);
            Assert.AreEqual(3,        reloaded.SortOrder);
            Assert.AreEqual(2,        reloaded.DepartmentId);
            Assert.AreEqual(3,        reloaded.UnitId);
            Assert.AreEqual(BaseEntity.Status.Inactive, reloaded.IsActive);
        }

        [TestMethod]
        public async Task DeleteAsync_SetsIsDeleted_AndSaves()
        {
            using var ctx = CreateInMemoryContext();
            var repo = new LocationCommandRepository(ctx);

            var notDeleted = Enum.TryParse<BaseEntity.IsDelete>("NotDeleted", true, out var nd) ? nd : (BaseEntity.IsDelete)0;

            var loc = new Location
            {
                Code = "L-001",
                LocationName = "ToDelete",
                SortOrder = 0,
                DepartmentId = 1,
                UnitId = 1,
                IsDeleted = notDeleted
            };
            SeedAudit(loc);
            ctx.Locations.Add(loc);
            await ctx.SaveChangesAsync();

            var toDelete = new Location { Id = loc.Id, IsDeleted = BaseEntity.IsDelete.Deleted };

            var affected = await repo.DeleteAsync(loc.Id, toDelete);

            Assert.AreEqual(1, affected);
            var reloaded = await ctx.Locations.FirstAsync(l => l.Id == loc.Id);
            Assert.AreEqual(BaseEntity.IsDelete.Deleted, reloaded.IsDeleted);
        }
    }
}
