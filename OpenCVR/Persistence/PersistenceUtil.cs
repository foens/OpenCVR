using System;
using System.Collections.Generic;
#if (__MonoCS__)
using SQLiteCommand = Mono.Data.Sqlite.SqliteCommand;
using SQLiteConnection = Mono.Data.Sqlite.SqliteConnection;
#else
using System.Data.SQLite;
#endif

namespace OpenCVR.Persistence
{
    internal class PersistenceUtil
    {
        public static DateTime? OptionalUnixTimeStampToDateTime(object unixTimeStamp)
        {
            if (unixTimeStamp == null)
                return null;
            return UnixTimeStampToDateTime((int)unixTimeStamp);
        }

        public static object OptionalDateTimeToUnixTimeStamp(DateTime? d)
        {
            if (d.HasValue)
                return DateTimeToUnixTimestamp(d.Value);
            return null;
        }

        public static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dtDateTime.AddSeconds(unixTimeStamp);
        }

        public static int DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (int)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
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
    }
}
