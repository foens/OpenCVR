using System;
using NUnit.Framework;
using OpenCVR.Model;
using OpenCVR.Persistence;
#if (__MonoCS__)
using SQLiteCommand = Mono.Data.Sqlite.SqliteCommand;
using SQLiteConnection = Mono.Data.Sqlite.SqliteConnection;
#else
using System.Data.SQLite;
#endif

namespace OpenCVR.Test.Unit.Persistence
{
    [TestFixture]
    public class CvrPersistenceTests
    {
        private SQLiteConnection connection;
        private CvrPersistence persistence;

        [SetUp]
        public void Setup()
        {
            connection = new SQLiteConnection("Data Source=:memory:;Version=3;New=true");
            persistence = new CvrPersistence(connection);
        }

        [Test]
        public void TestDoesCreateInitialSchema()
        {
            Assert.AreEqual(0, GetUserVersion());
            persistence.UpgradeSchemaIfRequired();
            Assert.AreEqual(1, GetUserVersion());
        }

        private int GetUserVersion()
        {
            var command = new SQLiteCommand
            {
                Connection = connection,
                CommandText = "PRAGMA user_version"
            };
            return (int) (long) command.ExecuteScalar();
        }

        [Test]
        public void TestCanInsertNewCompany()
        {
            persistence.UpgradeSchemaIfRequired();
            var c = new Company
            {
                VatNumber = 123,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                UpdatedDate = DateTime.Today,
                OptedOutForUnsolicictedAdvertising = true,
                NameValidFrom = DateTime.Today,
                Name = "test company"
            };

            persistence.InsertOrReplaceCompany(c);

            Assert.AreEqual(c, persistence.FindWithVat(c.VatNumber));
        }

        [Test]
        public void TestCanUpdateExistingCompany()
        {
            persistence.UpgradeSchemaIfRequired();
            var c = new Company
            {
                VatNumber = 123,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                UpdatedDate = DateTime.Today,
                OptedOutForUnsolicictedAdvertising = true,
                NameValidFrom = DateTime.Today
            };
            persistence.InsertOrReplaceCompany(c);
            c.OptedOutForUnsolicictedAdvertising = false;

            persistence.InsertOrReplaceCompany(c);

            Assert.AreEqual(c, persistence.FindWithVat(c.VatNumber));
        }

        [Test]
        public void TestCanDeleteExistingCompany()
        {
            persistence.UpgradeSchemaIfRequired();
            var c = new Company
            {
                VatNumber = 123,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                UpdatedDate = DateTime.Today,
                OptedOutForUnsolicictedAdvertising = true,
                NameValidFrom = DateTime.Today
            };
            persistence.InsertOrReplaceCompany(c);

            persistence.DeleteCompany(c.VatNumber);

            try
            {
                persistence.FindWithVat(c.VatNumber);
                Assert.Fail("Expected above query to fail, since the company was deleted");
            }
            catch (Exception)
            {
                // Expected
            }
        }

        [Test]
        public void TestCanQueryTimeWithDefaultValueOf0()
        {
            persistence.UpgradeSchemaIfRequired();

            var lastUpdateTime = persistence.GetLastProcessedEmailReceivedTime();

            Assert.AreEqual(DateTime.MinValue, lastUpdateTime);
        }

        [Test]
        public void TestCanInsertTime()
        {
            persistence.UpgradeSchemaIfRequired();
            var insertedLastUpdateTme = DateTime.Now;

            persistence.SetLastProcessedEmailReceivedTime(insertedLastUpdateTme);
            var lastUpdateTime = persistence.GetLastProcessedEmailReceivedTime();

            Assert.AreEqual(insertedLastUpdateTme, lastUpdateTime);
        }

        [Test]
        public void TestCanUpdateTimeMultipleTimes()
        {
            persistence.UpgradeSchemaIfRequired();
            var firstInsertDate = DateTime.Now;
            var secondInsertDate = firstInsertDate.AddDays(12);

            persistence.SetLastProcessedEmailReceivedTime(firstInsertDate);
            persistence.SetLastProcessedEmailReceivedTime(secondInsertDate);
            var lastUpdateTime = persistence.GetLastProcessedEmailReceivedTime();

            Assert.AreEqual(secondInsertDate, lastUpdateTime);
        }

        [Test]
        public void TestStartTransactionReturnsAnInstance()
        {
            Assert.NotNull(persistence.StartTransaction());
        }
    }
}
