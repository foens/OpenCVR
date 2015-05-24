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
            var transaction = connection.BeginTransaction();
            try
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
                transaction = null;
            }
            finally
            {
                transaction?.Rollback();
            }
        }

        private void SetUserVersion(long userVersion)
        {
            ExecuteNonQuery("PRAGMA user_version = " + userVersion);
        }

        private void ExecuteNonQuery(string commandText, Dictionary<string, object> parameters = null)
        {
            var command = new SQLiteCommand
            {
                Connection = connection,
                CommandText = commandText
            };
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }
            }
            command.ExecuteNonQuery();
        }

        private SQLiteDataReader ExecuteQuery(string commandText, Dictionary<string, object> parameters = null)
        {
            var command = new SQLiteCommand
            {
                Connection = connection,
                CommandText = commandText
            };
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }
            }
            return command.ExecuteReader();
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

        public void InsertCompanies(IEnumerable<Company> companies)
        {
            var transaction = connection.BeginTransaction();
            try
            {
                foreach (var company in companies)
                {
                    ExecuteNonQuery("INSERT INTO Company " +
                                   "(Vat, StartDate, EndDate, UpdatedDate, OptedOutForUnsolicictedAdvertising, NameValidFrom) VALUES " +
                                   "(@vat,@startDate,@endDate,@updateDate,@OptedOutForUnsolicictedAdvertising,@nameValidFrom)", new Dictionary<string, object>
                    {
                        {"@vat", company.VatNumber },
                        {"@startDate",  OptionalDateTimeToUnixTimeStamp(company.StartDate)},
                        {"@endDate",  OptionalDateTimeToUnixTimeStamp(company.EndDate)},
                        {"@updateDate",  OptionalDateTimeToUnixTimeStamp(company.UpdatedDate)},
                        {"@OptedOutForUnsolicictedAdvertising",  company.OptedOutForUnsolicictedAdvertising ? 1 : 0},
                        {"@nameValidFrom",  OptionalDateTimeToUnixTimeStamp(company.NameValidFrom)},
                    });
                }
                transaction.Commit();
                transaction = null;
            }
            finally
            {
                transaction?.Rollback();
            }
        }

        private static object OptionalDateTimeToUnixTimeStamp(DateTime? d)
        {
            if (d.HasValue)
                return DateTimeToUnixTimestamp(d.Value);
            return null;
        }

        internal static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dtDateTime.AddSeconds(unixTimeStamp);
        }

        internal static int DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (int) (dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public Company FindWithVat(int vatNumber)
        {
            var c = new SQLiteCommand
            {
                Connection = connection,
                CommandText = "SELECT * FROM Company WHERE Vat = @vat"
            };
            c.Parameters.AddWithValue("@vat", vatNumber);
            using (var r = c.ExecuteReader())
            {
                if (r.HasRows && r.Read())
                {
                    return new Company
                    {
                        VatNumber = (int) r["Vat"],
                        StartDate = OptionalUnixTimeStampToDateTime(r["StartDate"]),
                        EndDate = OptionalUnixTimeStampToDateTime(r["EndDate"]),
                        UpdatedDate = OptionalUnixTimeStampToDateTime(r["UpdatedDate"]),
                        OptedOutForUnsolicictedAdvertising = 1 == (int)r["OptedOutForUnsolicictedAdvertising"],
                        NameValidFrom = OptionalUnixTimeStampToDateTime(r["nameValidFrom"])
                    };
                }
                throw new Exception();
            }
        }

        private DateTime? OptionalUnixTimeStampToDateTime(object unixTimeStamp)
        {
            if (unixTimeStamp == null)
                return null;
            return UnixTimeStampToDateTime((int)unixTimeStamp);
        }

        public DateTime GetLastUpdateTime()
        {
            using (var r = ExecuteQuery("SELECT Value FROM KeyValue WHERE Key = @key", new Dictionary<string, object>
            {
                {"@key", "lastUpdateTime"}
            }))
            {
                if (r.HasRows && r.Read())
                {
                    return DateTime.ParseExact((string) r["Value"], "O", CultureInfo.InvariantCulture);
                }
                return DateTime.MinValue;
            }
        }

        public void SetLastUpdateTime(DateTime updateTime)
        {
            ExecuteNonQuery("INSERT OR REPLACE INTO KeyValue (Key, Value) VALUES (@key, @value)", new Dictionary<string, object>
            {
                {"@key", "lastUpdateTime" },
                {"@value", updateTime.ToString("O", CultureInfo.InvariantCulture) },
            });
        }
    }
}