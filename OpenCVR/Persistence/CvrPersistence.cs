using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using OpenCVR.Model;

namespace OpenCVR.Persistence
{
    class CvrPersistence : ICvrPersistence
    {
        private readonly DbConnection connection;

        public CvrPersistence(DbConnection connection)
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
                            "VatText text, " +
                            "StartDate int, " +
                            "EndDate int, " +
                            "UpdatedDate int, " +
                            "OptedOutForUnsolicictedAdvertising int, " +
                            "NameValidFrom int, " +
                            "Name text" +
                        ");");
                    ExecuteNonQuery("CREATE TABLE KeyValue" +
                                    "(" +
                        "Key text PRIMARY KEY NOT NULL," +
                            "Value text" +
                        ");");
                    ExecuteNonQuery("CREATE INDEX NameLikeIndex ON Company (Name COLLATE NOCASE);");
                    ExecuteNonQuery("CREATE INDEX VatLikeIndex ON Company (VatText COLLATE NOCASE);");
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

        private DbDataReader ExecuteQuery(string commandText, Dictionary<string, object> parameters = null)
        {
            return PersistenceUtil.CreateCommand(connection, commandText, parameters).ExecuteReader();
        }

        private long GetUserVersion()
        {
            var command = PersistenceUtil.CreateCommand(connection, "PRAGMA user_version");
            return (long)command.ExecuteScalar();
        }

        public Company FindWithVat(int vatNumber)
        {
            using (var r = ExecuteQuery("SELECT * FROM Company WHERE Vat = @vat", new Dictionary<string, object>
            {
                {"@vat", vatNumber }
            }))
            {
                if (r.Read())
                {
                    return ReaderToCompany(r);
                }
                throw new Exception();
            }
        }

        private static Company ReaderToCompany(DbDataReader r)
        {
            return new Company
            {
                VatNumber = (int) r["Vat"],
                StartDate = PersistenceUtil.OptionalMillisecondsSinceEpochToDateTime(GetLongOrNull(r, "StartDate")),
                EndDate = PersistenceUtil.OptionalMillisecondsSinceEpochToDateTime(GetLongOrNull(r, "EndDate")),
                UpdatedDate = PersistenceUtil.OptionalMillisecondsSinceEpochToDateTime(GetLongOrNull(r, "UpdatedDate")),
                OptedOutForUnsolicictedAdvertising = 1 == (int) r["OptedOutForUnsolicictedAdvertising"],
                NameValidFrom =
                    PersistenceUtil.OptionalMillisecondsSinceEpochToDateTime(GetLongOrNull(r, "nameValidFrom")),
                Name = PersistenceUtil.GetNullableString(r, "Name")
            };
        }

        private static long? GetLongOrNull(DbDataReader r, string key)
        {
            var index = r.GetOrdinal(key);
            if (r.IsDBNull(index))
                return null;
            return r.GetInt64(index);
        }

        public DateTime GetLastProcessedEmailReceivedTime()
        {
            using (var r = ExecuteQuery("SELECT Value FROM KeyValue WHERE Key = @key", new Dictionary<string, object>
            {
                {"@key", "lastProcessedEmailReceivedTime"}
            }))
            {
                if (r.Read())
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
                                   "(Vat, VatText, StartDate, EndDate, UpdatedDate, OptedOutForUnsolicictedAdvertising, NameValidFrom, Name) VALUES " +
                                   "(@vat,@vatText,@startDate,@endDate,@updateDate,@OptedOutForUnsolicictedAdvertising,@nameValidFrom,@name)", new Dictionary<string, object>
                    {
                        {"@vat", c.VatNumber },
                        {"@vatText",  c.VatNumber.ToString()},
                        {"@startDate",  PersistenceUtil.OptionalDateTimeMillisecondsSinceEpoch(c.StartDate)},
                        {"@endDate",  PersistenceUtil.OptionalDateTimeMillisecondsSinceEpoch(c.EndDate)},
                        {"@updateDate",  PersistenceUtil.OptionalDateTimeMillisecondsSinceEpoch(c.UpdatedDate)},
                        {"@OptedOutForUnsolicictedAdvertising",  c.OptedOutForUnsolicictedAdvertising ? 1 : 0},
                        {"@nameValidFrom",  PersistenceUtil.OptionalDateTimeMillisecondsSinceEpoch(c.NameValidFrom)},
                        {"@name",  c.Name}
                    });
        }

        public void DeleteCompany(int vatNumber)
        {
            ExecuteNonQuery("DELETE FROM Company WHERE Vat = @vat", new Dictionary<string, object>
                    {
                        {"@vat", vatNumber }
                    });
        }

        public Company Search(string search)
        {
            using (var r = ExecuteQuery("SELECT * FROM Company WHERE VatText LIKE @vat OR Name LIKE @name LIMIT 1", new Dictionary<string, object>
            {
                {"@vat", search + "%"},
                {"@name", search + "%"},
            }))
            {
                if (r.Read())
                {
                    return ReaderToCompany(r);
                }
                return null;
            }
        }
    }
}