using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dapper;

using Core.Domain.Entities;                       // Location
using FAM.Infrastructure.Repositories.Locations;  // LocationQueryRepository

namespace FixedAssetManagement.Tests.UnitTests.Repositories.Locations
{
    [TestClass]
    public class LocationQueryRepository_IntegrationTests
    {
        private static async Task ExecuteAsync(string connStr, string sql)
        {
            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();
            await conn.ExecuteAsync(sql);
        }

        private sealed class TestDb : IAsyncDisposable
        {
            public string Master => new SqlConnectionStringBuilder
            {
                DataSource = @"(localdb)\MSSQLLocalDB",
                InitialCatalog = "master",
                IntegratedSecurity = true,
                TrustServerCertificate = true
            }.ConnectionString;

            public string DbName    { get; } = "UT_FixedAsset_" + Guid.NewGuid().ToString("N");
            public string BannariDb { get; } = "Bannari"; // repo uses three-part names against this DB

            public string DbConnString => new SqlConnectionStringBuilder
            {
                DataSource = @"(localdb)\MSSQLLocalDB",
                InitialCatalog = DbName,
                IntegratedSecurity = true,
                TrustServerCertificate = true,
                MultipleActiveResultSets = true
            }.ConnectionString;

            public async Task InitializeAsync()
            {
                // Create main db
                await ExecuteAsync(Master, $"CREATE DATABASE [{DbName}]");

                // Create FixedAsset schema + Location table (ALL columns the repo selects)
                await ExecuteAsync(DbConnString, @"
                    IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'FixedAsset')
                        EXEC('CREATE SCHEMA FixedAsset');

                    IF OBJECT_ID('FixedAsset.Location') IS NOT NULL DROP TABLE FixedAsset.Location;

                    CREATE TABLE FixedAsset.Location
                    (
                        Id             INT IDENTITY(1,1) PRIMARY KEY,
                        Code           NVARCHAR(50)    NOT NULL,
                        LocationName   NVARCHAR(255)   NOT NULL,
                        Description    NVARCHAR(255)   NULL,
                        SortOrder      INT             NULL,
                        UnitId         INT             NULL,
                        DepartmentId   INT             NULL,
                        IsActive       INT             NULL,
                        IsDeleted      INT             NOT NULL DEFAULT 0,

                        CreatedBy      INT             NULL,
                        CreatedDate    DATETIME2       NULL,
                        CreatedByName  NVARCHAR(100)   NULL,
                        CreatedIP      NVARCHAR(64)    NULL,

                        ModifiedBy     INT             NULL,
                        ModifiedDate   DATETIME2       NULL,
                        ModifiedByName NVARCHAR(100)   NULL,
                        ModifiedIP     NVARCHAR(64)    NULL
                    );
                ");

                // Ensure Bannari DB with AppData.Department/Unit exists for the JOINs
                await ExecuteAsync(Master, $@"
                    IF DB_ID('{BannariDb}') IS NULL
                        CREATE DATABASE [{BannariDb}];
                ");

                var bannariConn = new SqlConnectionStringBuilder(DbConnString)
                {
                    InitialCatalog = BannariDb
                }.ConnectionString;

                await ExecuteAsync(bannariConn, @"
                    IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'AppData')
                        EXEC('CREATE SCHEMA AppData');

                    IF OBJECT_ID('AppData.Department') IS NOT NULL DROP TABLE AppData.Department;
                    IF OBJECT_ID('AppData.Unit')       IS NOT NULL DROP TABLE AppData.Unit;

                    CREATE TABLE AppData.Department ( Id INT PRIMARY KEY, DepartmentName NVARCHAR(100) NULL );
                    CREATE TABLE AppData.Unit       ( Id INT PRIMARY KEY, UnitName       NVARCHAR(100) NULL );

                    INSERT INTO AppData.Department(Id, DepartmentName) VALUES (10, 'Dept-A'), (20, 'Dept-B');
                    INSERT INTO AppData.Unit(Id, UnitName)             VALUES (2, 'Unit-2'), (3, 'Unit-3');
                ");
            }

            public async ValueTask DisposeAsync()
            {
                try { await ExecuteAsync(Master, $@"ALTER DATABASE [{DbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{DbName}];"); } catch { }
                // If Bannari is a shared dev DB, comment out the next line.
                try { await ExecuteAsync(Master, $@"ALTER DATABASE [{BannariDb}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{BannariDb}];"); } catch { }
            }
        }

        private static IDbConnection OpenConn(string cs)
        {
            var c = new SqlConnection(cs);
            c.Open();
            return c;
        }

        [TestMethod]
        public async Task GetByIdAsync_Returns_Location_WhenFound()
        {
            await using var db = new TestDb();
            await db.InitializeAsync();

            await ExecuteAsync(db.DbConnString, @"
                INSERT INTO FixedAsset.Location(Code, LocationName, DepartmentId, UnitId, IsDeleted)
                VALUES ('LOC-042', 'Main Store', 10, 3, 0);
            ");

            using var conn = OpenConn(db.DbConnString);
            var repo = new LocationQueryRepository(conn);

            var row = await repo.GetByIdAsync(1);

            Assert.IsNotNull(row);
            Assert.AreEqual(1, row.Id);
            Assert.AreEqual("LOC-042", row.Code);
            Assert.AreEqual("Main Store", row.LocationName);
            Assert.AreEqual(10, row.DepartmentId);
            Assert.AreEqual(3, row.UnitId);
        }

        [TestMethod]
        public async Task GetByIdAsync_Returns_Null_WhenMissing()
        {
            await using var db = new TestDb();
            await db.InitializeAsync();

            using var conn = OpenConn(db.DbConnString);
            var repo = new LocationQueryRepository(conn);

            var row = await repo.GetByIdAsync(999);
            Assert.IsNull(row);
        }

        [TestMethod]
        public async Task GetByLocationNameAsync_Returns_Row_WhenFound()
        {
            await using var db = new TestDb();
            await db.InitializeAsync();

            await ExecuteAsync(db.DbConnString, @"
                INSERT INTO FixedAsset.Location(Code, LocationName, DepartmentId, UnitId, IsDeleted)
                VALUES ('LOC-007', 'Spare Store', 20, 2, 0);
            ");

            using var conn = OpenConn(db.DbConnString);
            var repo = new LocationQueryRepository(conn);

            var row = await repo.GetByLocationNameAsync("Spare Store", DepartmentId: 20, UnitId: 2);

            Assert.IsNotNull(row);
            Assert.AreEqual("LOC-007", row!.Code);
            Assert.AreEqual(20, row.DepartmentId);
            Assert.AreEqual(2, row.UnitId);
        }

        [TestMethod]
        public async Task GetLocation_Returns_Filtered_List()
        {
            await using var db = new TestDb();
            await db.InitializeAsync();

            await ExecuteAsync(db.DbConnString, @"
                INSERT INTO FixedAsset.Location(Code, LocationName, IsDeleted) VALUES
                ('L-1','Main Store', 0),
                ('L-2','Spare Store',0),
                ('L-3','Other',      0);
            ");

            using var conn = OpenConn(db.DbConnString);
            var repo = new LocationQueryRepository(conn);

            var rows = await repo.GetLocation("Store");

            Assert.IsNotNull(rows);
            Assert.AreEqual(2, rows.Count);
            Assert.AreEqual("Main Store", rows[0].LocationName);
            Assert.AreEqual("Spare Store", rows[1].LocationName);
        }
    }
}
