using System;
using System.Data.Common;
using NUnit.Framework;
using OpenCVR.Model;
using OpenCVR.Persistence;

namespace OpenCVR.Test.Unit.Persistence
{
    [TestFixture]
    public class CvrPersistenceTests
    {
        private DbConnection connection;
        private ICvrPersistence persistence;

        [SetUp]
        public void Setup()
        {
            connection = PersistenceUtil.CreateConnection("Data Source=:memory:;Version=3;New=true");
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
            var command = PersistenceUtil.CreateCommand(connection, "PRAGMA user_version");
            return (int) (long) command.ExecuteScalar();
        }

        [Test]
        public void TestCanInsertNewCompany()
        {
            persistence.UpgradeSchemaIfRequired();
            var c = CreateCompany();

            persistence.InsertOrReplaceCompany(c);

            Assert.AreEqual(c, persistence.FindWithVat(c.VatNumber));
        }

        [Test]
        public void TestCanInsertNewCompanyWithNulls()
        {
            persistence.UpgradeSchemaIfRequired();
            var c = CreateCompany();
            c.NameValidFrom = null;

            persistence.InsertOrReplaceCompany(c);

            Assert.AreEqual(c, persistence.FindWithVat(c.VatNumber));
        }

        [Test]
        public void TestCanUpdateExistingCompany()
        {
            persistence.UpgradeSchemaIfRequired();
            var c = CreateCompany();
            persistence.InsertOrReplaceCompany(c);
            c.OptedOutForUnsolicictedAdvertising = false;

            persistence.InsertOrReplaceCompany(c);

            Assert.AreEqual(c, persistence.FindWithVat(c.VatNumber));
        }

        [Test]
        public void TestCanDeleteExistingCompany()
        {
            persistence.UpgradeSchemaIfRequired();
            var c = CreateCompany();
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

        private static Company CreateCompany()
        {
            var c = new Company
            {
                VatNumber = 1234567890,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                UpdatedDate = DateTime.Today,
                OptedOutForUnsolicictedAdvertising = true,
                NameValidFrom = DateTime.Today,
                Name = "Foobar baz"
            };
            return c;
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

        [Test]
        public void TestCanSearchByFullVatNumber()
        {
            persistence.UpgradeSchemaIfRequired();
            var c = CreateCompany();
            persistence.InsertOrReplaceCompany(c);

            var returnedCompany = persistence.Search(c.VatNumber.ToString());

            Assert.AreEqual(c, returnedCompany);
        }

        [Test]
        public void TestCanSearchByFullName()
        {
            persistence.UpgradeSchemaIfRequired();
            var c = CreateCompany();
            persistence.InsertOrReplaceCompany(c);

            var returnedCompany = persistence.Search(c.Name);

            Assert.AreEqual(c, returnedCompany);
        }

        [Test]
        public void TestCanSearchByPrefixOfName()
        {
            persistence.UpgradeSchemaIfRequired();
            var c = CreateCompany();
            persistence.InsertOrReplaceCompany(c);
            if(c.Name.Length < 10)
                throw new Exception("This test requires a longer length name");

            var returnedCompany = persistence.Search(c.Name.Substring(0, 5));

            Assert.AreEqual(c, returnedCompany);
        }

        [Test]
        public void TestSqlWildcardsInSearchStringIsEscaped()
        {
            persistence.UpgradeSchemaIfRequired();
            var c = CreateCompany();
            persistence.InsertOrReplaceCompany(c);
            if (c.Name.Length < 10)
                throw new Exception("This test requires a longer length name");
            if(c.Name[6] == '%')
                throw new Exception("Must not be %");

            var returnedCompany = persistence.Search(c.Name.Substring(0, 5) + "%");

            Assert.IsNull(returnedCompany);
        }

        [Test]
        public void TestCanFindCompanyWithBackslashInName()
        {
            persistence.UpgradeSchemaIfRequired();
            var c = CreateCompany();
            c.Name = @"\foo";
            persistence.InsertOrReplaceCompany(c);

            var returnedCompany = persistence.Search(@"\");

            Assert.AreEqual(c, returnedCompany);
        }

        [Test]
        public void TestCanSearchByPrefixOfVat()
        {
            persistence.UpgradeSchemaIfRequired();
            var c = CreateCompany();
            persistence.InsertOrReplaceCompany(c);
            if (c.VatNumber.ToString().Length < 10)
                throw new Exception("This test requires a longer vat number");

            var returnedCompany = persistence.Search(c.VatNumber.ToString().Substring(0, 4));

            Assert.AreEqual(c, returnedCompany);
        }

        [Ignore]
        [Test]
        public void TestCanSearchByProductionUnitNumber()
        {
            persistence.UpgradeSchemaIfRequired();
            var c = CreateCompany();
            persistence.InsertOrReplaceCompany(c);

            var returnedCompany = persistence.Search(c.ProductionUnits[0].ToString());

            Assert.AreEqual(c, returnedCompany);
        }
    }
}
