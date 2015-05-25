using System;
using System.Collections.Generic;
#if (__MonoCS__)
using SQLiteCommand = Mono.Data.Sqlite.SqliteCommand;
using SQLiteConnection = Mono.Data.Sqlite.SqliteConnection;
using SQLiteDataReader = Mono.Data.Sqlite.SqliteDataReader;
#else
using System.Data.SQLite;
#endif

namespace OpenCVR.Persistence
{
    internal class PersistenceUtil
    {
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

        public static SQLiteCommand CreateCommand(SQLiteConnection connection, string commandText, Dictionary<string, object> parameters)
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
            return command;
        }

        public static string GetNullableString(SQLiteDataReader reader, string key)
        {
            int index = reader.GetOrdinal(key);
            if (reader.IsDBNull(index))
                return null;
            return reader.GetString(index);
        }
    }
}
