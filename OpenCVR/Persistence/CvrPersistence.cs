using System;
using System.Collections.Generic;
using System.Globalization;
using OpenCVR.Model;
#if (__MonoCS__)
using SQLiteCommand = Mono.Data.Sqlite.SqliteCommand;
using SQLiteConnection = Mono.Data.Sqlite.SqliteConnection;
using SQLiteDataReader = Mono.Data.Sqlite.SqliteDataReader;
#else
using System.Data.SQLite;
#endif

namespace OpenCVR.Persistence
{
    class CvrPersistence : ICvrPersistence
    {
        private readonly SQLiteConnection connection;

        public CvrPersistence(SQLiteConnection connection)
        {
            this.connection = connection;
            connection.Open();
        }

        public void UpgradeSchemaIfRequired()
        {
            using (var transaction = connection.BeginTransaction())
            {
                if (GetUserVersion() == 0)
                {
                    ExecuteNonQuery("CREATE TABLE Company" +
                                    "(" +
                        "Vat INT PRIMARY KEY NOT NULL," +
                            "StartDate int, " +
                            "EndDate int, " +
                            "UpdatedDate int, " +
                            "OptedOutForUnsolicictedAdvertising int, " +
                            "NameValidFrom int" +
                        ");");

                    ExecuteNonQuery("CREATE TABLE KeyValue" +
                                    "(" +
                        "Key text PRIMARY KEY NOT NULL," +
                            "Value text" +
                        ");");
                    SetUserVersion(1);
                }
                transaction.Commit();
            }
        }

        private void SetUserVersion(long userVersion)
        {
            ExecuteNonQuery("PRAGMA user_version = " + userVersion);
        }

        private void ExecuteNonQuery(string commandText, Dictionary<string, object> parameters = null)
        {
            PersistenceUtil.CreateCommand(connection, commandText, parameters).ExecuteNonQuery();
        }

        private SQLiteDataReader ExecuteQuery(string commandText, Dictionary<string, object> parameters = null)
        {
            return PersistenceUtil.CreateCommand(connection, commandText, parameters).ExecuteReader();
        }

        private long GetUserVersion()
        {
            var command = new SQLiteCommand
            {
                Connection = connection,
                CommandText = "PRAGMA user_version"
            };
            return (long)command.ExecuteScalar();
        }

        public Company FindWithVat(int vatNumber)
        {
            using (var r = ExecuteQuery("SELECT * FROM Company WHERE Vat = @vat", new Dictionary<string, object>
            {
                {"@vat", vatNumber }
            }))
            {
                if (r.HasRows && r.Read())
                {
                    return new Company
                    {
                        VatNumber = (int) r["Vat"],
                        StartDate = PersistenceUtil.OptionalMillisecondsSinceEpochToDateTime(r.GetInt64(r.GetOrdinal("StartDate"))),
                        EndDate = PersistenceUtil.OptionalMillisecondsSinceEpochToDateTime(r.GetInt64(r.GetOrdinal("EndDate"))),
                        UpdatedDate = PersistenceUtil.OptionalMillisecondsSinceEpochToDateTime(r.GetInt64(r.GetOrdinal("UpdatedDate"))),
                        OptedOutForUnsolicictedAdvertising = 1 == (int)r["OptedOutForUnsolicictedAdvertising"],
                        NameValidFrom = PersistenceUtil.OptionalMillisecondsSinceEpochToDateTime(r.GetInt64(r.GetOrdinal("nameValidFrom")))
                    };
                }
                throw new Exception();
            }
        }

        public DateTime GetLastProcessedEmailReceivedTime()
        {
            using (var r = ExecuteQuery("SELECT Value FROM KeyValue WHERE Key = @key", new Dictionary<string, object>
            {
                {"@key", "lastProcessedEmailReceivedTime"}
            }))
            {
                if (r.HasRows && r.Read())
                {
                    return DateTime.ParseExact((string) r["Value"], "O", CultureInfo.InvariantCulture);
                }
                return DateTime.MinValue;
            }
        }

        public void SetLastProcessedEmailReceivedTime(DateTime updateTime)
        {
            ExecuteNonQuery("INSERT OR REPLACE INTO KeyValue (Key, Value) VALUES (@key, @value)", new Dictionary<string, object>
            {
                {"@key", "lastProcessedEmailReceivedTime" },
                {"@value", updateTime.ToString("O", CultureInfo.InvariantCulture) },
            });
        }

        public IPersistenceTransaction StartTransaction()
        {
            return new PersistenceTransaction(connection.BeginTransaction());
        }

        public void InsertOrReplaceCompany(Company c)
        {
            ExecuteNonQuery("INSERT OR REPLACE INTO Company " +
                                   "(Vat, StartDate, EndDate, UpdatedDate, OptedOutForUnsolicictedAdvertising, NameValidFrom) VALUES " +
                                   "(@vat,@startDate,@endDate,@updateDate,@OptedOutForUnsolicictedAdvertising,@nameValidFrom)", new Dictionary<string, object>
                    {
                        {"@vat", c.VatNumber },
                        {"@startDate",  PersistenceUtil.OptionalDateTimeMillisecondsSinceEpoch(c.StartDate)},
                        {"@endDate",  PersistenceUtil.OptionalDateTimeMillisecondsSinceEpoch(c.EndDate)},
                        {"@updateDate",  PersistenceUtil.OptionalDateTimeMillisecondsSinceEpoch(c.UpdatedDate)},
                        {"@OptedOutForUnsolicictedAdvertising",  c.OptedOutForUnsolicictedAdvertising ? 1 : 0},
                        {"@nameValidFrom",  PersistenceUtil.OptionalDateTimeMillisecondsSinceEpoch(c.NameValidFrom)},
                    });
        }

        public void DeleteCompany(int vatNumber)
        {
            ExecuteNonQuery("DELETE FROM Company WHERE Vat = @vat", new Dictionary<string, object>
                    {
                        {"@vat", vatNumber }
                    });
        }
    }
}