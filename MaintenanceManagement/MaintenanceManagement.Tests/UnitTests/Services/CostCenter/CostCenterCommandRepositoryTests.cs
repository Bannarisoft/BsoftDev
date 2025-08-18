using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;

using Core.Domain.Entities;
using MaintenanceManagement.Infrastructure.Data;
using MaintenanceManagement.Infrastructure.Repositories.CostCenter;
using Core.Application.Common.Interfaces; // IIPAddressService, ITimeZoneService
using IsDeleteEnum = Core.Domain.Common.BaseEntity.IsDelete;
using StatusEnum   = Core.Domain.Common.BaseEntity.Status;

namespace MaintenanceManagement.Tests.UnitTests.Services.CostCenter
{
    [TestClass]
    public class CostCenterCommandRepositoryTests
    {
        private static ApplicationDbContext CreateDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            // Mocks that ALWAYS return non-null strings for any string member
            var ipSvcMock = new Mock<IIPAddressService>(MockBehavior.Loose);
            ipSvcMock.SetReturnsDefault<string>("127.0.0.1");

            var tzSvcMock = new Mock<ITimeZoneService>(MockBehavior.Loose);
            tzSvcMock.SetReturnsDefault<string>("Asia/Kolkata"); // harmless default if any string is requested

            return new ApplicationDbContext(options, ipSvcMock.Object, tzSvcMock.Object);
        }

        private static StatusEnum PickStatusValue()
        {
            foreach (var name in Enum.GetNames(typeof(StatusEnum)))
                if (string.Equals(name, "Active", StringComparison.OrdinalIgnoreCase))
                    return (StatusEnum)Enum.Parse(typeof(StatusEnum), name, true);
            var values = (StatusEnum[])Enum.GetValues(typeof(StatusEnum));
            return values.Length > 0 ? values[0] : default;
        }

        private static IsDeleteEnum PickDeletedValue()
        {
            var values = (IsDeleteEnum[])Enum.GetValues(typeof(IsDeleteEnum));
            return values.Length > 0 ? values[^1] : default;
        }

        private static CostCenter MakeCostCenter(
            string code = "CC-001",
            string name = "Spinning",
            int unitId = 70,
            int deptId = 10)
        {
            return new CostCenter
            {
                CostCenterCode    = code,
                CostCenterName    = name,
                UnitId            = unitId,
                DepartmentId      = deptId,

                // required fields
                ResponsiblePerson = "Unit Test",
                CreatedByName     = "UnitTest",
                CreatedIP         = "127.0.0.1",

                EffectiveDate     = DateTimeOffset.UtcNow,
                IsActive          = PickStatusValue(),
                Remarks           = "seed"
            };
        }

        [TestMethod]
        public async Task CreateAsync_Persists_And_Returns_Id()
        {
            using var db = CreateDb();
            var repo = new CostCenterCommandRepository(db);

            var entity = MakeCostCenter();

            var newId = await repo.CreateAsync(entity);

            Assert.IsTrue(newId > 0);
            var saved = await db.CostCenter.FirstOrDefaultAsync(c => c.Id == newId);
            Assert.IsNotNull(saved);
            Assert.AreEqual("CC-001", saved!.CostCenterCode);
            Assert.AreEqual("Spinning", saved.CostCenterName);
            Assert.IsFalse(string.IsNullOrWhiteSpace(saved.CreatedByName));
            Assert.AreEqual("127.0.0.1", saved.CreatedIP);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenFound_Updates_And_Returns_1()
        {
            using var db = CreateDb();
            var repo = new CostCenterCommandRepository(db);

            var existing = MakeCostCenter();
            db.CostCenter.Add(existing);
            await db.SaveChangesAsync();

            var update = MakeCostCenter(
                code: existing.CostCenterCode,
                name: "Spinning Updated",
                unitId: 71,
                deptId: 11
            );
            update.ResponsiblePerson = "Updated User";
            update.Remarks = "updated";

            var result = await repo.UpdateAsync(existing.Id, update);

            Assert.AreEqual(1, result);

            var reloaded = await db.CostCenter.FirstAsync(c => c.Id == existing.Id);
            Assert.AreEqual("Spinning Updated", reloaded.CostCenterName);
            Assert.AreEqual(71, reloaded.UnitId);
            Assert.AreEqual(11, reloaded.DepartmentId);
            Assert.AreEqual("Updated User", reloaded.ResponsiblePerson);
            Assert.IsFalse(string.IsNullOrWhiteSpace(reloaded.CreatedByName));
            Assert.IsFalse(string.IsNullOrWhiteSpace(reloaded.CreatedIP));
        }

        [TestMethod]
        public async Task UpdateAsync_WhenMissing_Returns_Minus1()
        {
            using var db = CreateDb();
            var repo = new CostCenterCommandRepository(db);

            var update = MakeCostCenter(name: "DoesNotMatter");
            var result = await repo.UpdateAsync(9999, update);

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenFound_Sets_IsDeleted_And_Returns_1()
        {
            using var db = CreateDb();
            var repo = new CostCenterCommandRepository(db);

            var existing = MakeCostCenter();
            db.CostCenter.Add(existing);
            await db.SaveChangesAsync();

            var toDelete = new CostCenter { IsDeleted = PickDeletedValue() };

            var result = await repo.DeleteAsync(existing.Id, toDelete);

            Assert.AreEqual(1, result);

            var reloaded = await db.CostCenter.FirstAsync(c => c.Id == existing.Id);
            Assert.AreEqual(toDelete.IsDeleted, reloaded.IsDeleted);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenMissing_Returns_Minus1()
        {
            using var db = CreateDb();
            var repo = new CostCenterCommandRepository(db);

            var toDelete = new CostCenter { IsDeleted = PickDeletedValue() };

            var result = await repo.DeleteAsync(9999, toDelete);

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public async Task ExistsByCodeAsync_Returns_True_If_Exists_False_Otherwise()
        {
            using var db = CreateDb();
            var repo = new CostCenterCommandRepository(db);

            db.CostCenter.Add(MakeCostCenter(code: "CC-ABC"));
            await db.SaveChangesAsync();

            Assert.IsTrue(await repo.ExistsByCodeAsync("CC-ABC"));
            Assert.IsFalse(await repo.ExistsByCodeAsync("CC-NOPE"));
            Assert.IsFalse(await repo.ExistsByCodeAsync(null));
            Assert.IsFalse(await repo.ExistsByCodeAsync(string.Empty));
            Assert.IsFalse(await repo.ExistsByCodeAsync("   "));
        }

        [TestMethod]
        public async Task IsNameDuplicateAsync_Excludes_Id_And_Detects_Duplicates()
        {
            using var db = CreateDb();
            var repo = new CostCenterCommandRepository(db);

            var a = MakeCostCenter(code: "CC-A", name: "Alpha");
            var b = MakeCostCenter(code: "CC-B", name: "Beta");
            db.CostCenter.AddRange(a, b);
            await db.SaveChangesAsync();

            var notDuplicate = await repo.IsNameDuplicateAsync("Alpha", a.Id);
            Assert.IsFalse(notDuplicate);

            var duplicate = await repo.IsNameDuplicateAsync("Alpha", b.Id);
            Assert.IsTrue(duplicate);

            var fresh = await repo.IsNameDuplicateAsync("Gamma", 0);
            Assert.IsFalse(fresh);
        }
    }
}
