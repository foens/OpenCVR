using System;
using NUnit.Framework;
using OpenCVR.Model;
using OpenCVR.Persistence;
using OpenCVR.Update.Parse;
#if (linux)
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
        public void TestUnixTime()
        {
            DateTime original = DateTime.Today;
            var converted = CvrPersistence.UnixTimeStampToDateTime(CvrPersistence.DateTimeToUnixTimestamp(original));
            Assert.AreEqual(original, converted);
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
                NameValidFrom = DateTime.Today,
                OptedOutForUnsolicictedAdvertising = true,
                UpdatedDate = DateTime.Today
            };

            persistence.InsertCompanies(new[]{ c });

            Assert.AreEqual(c, persistence.FindWithVat(c.VatNumber));
        }
    }
}
