using System;
using System.Collections.Generic;
using System.Data.Common;
using Mono.Data.Sqlite;
using System.Data.SQLite;

namespace OpenCVR.Persistence
{
    internal class PersistenceUtil
    {
        public static DbConnection CreateConnection(string connectionString)
        {
            if(IsRunningOnMono())
                return new SqliteConnection(connectionString);
            return new SQLiteConnection(connectionString);
        }

        public static DateTime? OptionalMillisecondsSinceEpochToDateTime(object unixTimeStamp)
        {
            if (unixTimeStamp == null)
                return null;
            return MillisecondsSinceEpochToDateTime((long)unixTimeStamp);
        }

        public static DateTime MillisecondsSinceEpochToDateTime(long unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dtDateTime.AddMilliseconds(unixTimeStamp);
        }

        public static object OptionalDateTimeMillisecondsSinceEpoch(DateTime? d)
        {
            if (d.HasValue)
                return DateTimeToMillisecondsSinceEpoch(d.Value);
            return null;
        }
        
        public static long DateTimeToMillisecondsSinceEpoch(DateTime dateTime)
        {
            return (long)(dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public static DbCommand CreateCommand(DbConnection connection, string commandText, Dictionary<string, object> parameters = null)
        {
            var command = CreateCommandInternal();
            command.Connection = connection;
            command.CommandText = commandText;

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(CreateParameter(parameter.Key, parameter.Value));
                }
            }
            return command;
        }

        private static DbParameter CreateParameter(string key, object value)
        {
            if(IsRunningOnMono())
                return new SqliteParameter(key, value);
            return new SQLiteParameter(key, value);
        }

        private static DbCommand CreateCommandInternal()
        {
            if (IsRunningOnMono())
                return new SqliteCommand();
            return new SQLiteCommand();
        }

        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        public static string GetNullableString(DbDataReader reader, string key)
        {
            int index = reader.GetOrdinal(key);
            if (reader.IsDBNull(index))
                return null;
            return reader.GetString(index);
        }
    }
}
