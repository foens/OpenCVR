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
                            "NameValidFrom int, " +
                            "Name text" +
                        ");");
                    ExecuteNonQuery("CREATE TABLE KeyValue" +
                                    "(" +
                        "Key text PRIMARY KEY NOT NULL," +
                            "Value text" +
                        ");");
                    ExecuteNonQuery("CREATE INDEX NameLikeIndex ON Company (NAME COLLATE NOCASE);");
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
                    return ReaderToCompany(r);
                }
                throw new Exception();
            }
        }

        private static Company ReaderToCompany(SQLiteDataReader r)
        {
            return new Company
            {
                VatNumber = (int) r["Vat"],
                StartDate = PersistenceUtil.OptionalMillisecondsSinceEpochToDateTime(r.GetInt64(r.GetOrdinal("StartDate"))),
                EndDate = PersistenceUtil.OptionalMillisecondsSinceEpochToDateTime(r.GetInt64(r.GetOrdinal("EndDate"))),
                UpdatedDate = PersistenceUtil.OptionalMillisecondsSinceEpochToDateTime(r.GetInt64(r.GetOrdinal("UpdatedDate"))),
                OptedOutForUnsolicictedAdvertising = 1 == (int) r["OptedOutForUnsolicictedAdvertising"],
                NameValidFrom =
                    PersistenceUtil.OptionalMillisecondsSinceEpochToDateTime(r.GetInt64(r.GetOrdinal("nameValidFrom"))),
                Name = PersistenceUtil.GetNullableString(r, "Name")
            };
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
                                   "(Vat, StartDate, EndDate, UpdatedDate, OptedOutForUnsolicictedAdvertising, NameValidFrom, Name) VALUES " +
                                   "(@vat,@startDate,@endDate,@updateDate,@OptedOutForUnsolicictedAdvertising,@nameValidFrom,@name)", new Dictionary<string, object>
                    {
                        {"@vat", c.VatNumber },
                        {"@startDate",  PersistenceUtil.OptionalDateTimeMillisecondsSinceEpoch(c.StartDate)},
                        {"@endDate",  PersistenceUtil.OptionalDateTimeMillisecondsSinceEpoch(c.EndDate)},
                        {"@updateDate",  PersistenceUtil.OptionalDateTimeMillisecondsSinceEpoch(c.UpdatedDate)},
                        {"@OptedOutForUnsolicictedAdvertising",  c.OptedOutForUnsolicictedAdvertising ? 1 : 0},
                        {"@nameValidFrom",  PersistenceUtil.OptionalDateTimeMillisecondsSinceEpoch(c.NameValidFrom)},
                        {"@name",  c.Name},
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
            const char escapeCharacter = '\\';
            using (var r = ExecuteQuery("SELECT * FROM Company WHERE Vat LIKE @vat ESCAPE @escape OR Name LIKE @name ESCAPE @escape", new Dictionary<string, object>
            {
                {"@vat", EscapeLikeValue(search, escapeCharacter) + "%"},
                {"@name", EscapeLikeValue(search, escapeCharacter) + "%"},
                {"@escape", escapeCharacter.ToString() }
            }))
            {
                if (r.HasRows && r.Read())
                {
                    return ReaderToCompany(r);
                }
                return null;
            }
        }

        private static string EscapeLikeValue(string like, char escapeCharacter)
        {
            return like.Replace("" + escapeCharacter, "" + escapeCharacter + escapeCharacter)
                .Replace("%", escapeCharacter + "%")
                .Replace("_", escapeCharacter + "_")
                .Replace("[", escapeCharacter + "[")
                .Replace("]", escapeCharacter + "]");
        }
    }
}