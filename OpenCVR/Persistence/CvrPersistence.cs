using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
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
                            "Name text," +

                            "LocationAddress_ValidFrom int," +
                            "LocationAddress_StreetName text," +
                            "LocationAddress_RoadCode int," +
                            "LocationAddress_HouseNumberFrom int," +
                            "LocationAddress_HouseNumberTo int," +
                            "LocationAddress_LetterFrom text," +
                            "LocationAddress_LetterTo text," +
                            "LocationAddress_Floor text," +
                            "LocationAddress_SideDoor text," +
                            "LocationAddress_ZipCode int," +
                            "LocationAddress_PostalDistrict text," +
                            "LocationAddress_CityName text," +
                            "LocationAddress_MunicipalityCode int," +
                            "LocationAddress_MunicipalityText text," +
                            "LocationAddress_PostalBox int," +
                            "LocationAddress_CoName text," +
                            "LocationAddress_AddressFreeText text," +

                            "PostalAddress_ValidFrom int," +
                            "PostalAddress_StreetName text," +
                            "PostalAddress_RoadCode int," +
                            "PostalAddress_HouseNumberFrom int," +
                            "PostalAddress_HouseNumberTo int," +
                            "PostalAddress_LetterFrom text," +
                            "PostalAddress_LetterTo text," +
                            "PostalAddress_Floor text," +
                            "PostalAddress_SideDoor text," +
                            "PostalAddress_ZipCode int," +
                            "PostalAddress_PostalDistrict text," +
                            "PostalAddress_CityName text," +
                            "PostalAddress_MunicipalityCode int," +
                            "PostalAddress_MunicipalityText text," +
                            "PostalAddress_PostalBox int," +
                            "PostalAddress_CoName text," +
                            "PostalAddress_AddressFreeText text" +
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
                NameValidFrom = PersistenceUtil.OptionalMillisecondsSinceEpochToDateTime(GetLongOrNull(r, "nameValidFrom")),
                Name = PersistenceUtil.GetNullableString(r, "Name"),
                LocationAddress = ReadAddress("LocationAddress", r),
                PostalAddress = ReadAddress("PostalAddress", r),
            };
        }

        private static Address ReadAddress(string prefix, DbDataReader r)
        {
            prefix += "_";
            return new Address
            {
                ValidFrom = PersistenceUtil.OptionalMillisecondsSinceEpochToDateTime(GetLongOrNull(r, prefix + "ValidFrom")),
                StreetName = PersistenceUtil.GetNullableString(r, prefix + "StreetName"),
                RoadCode = PersistenceUtil.GetNullableInt(r, prefix + "RoadCode"),
                HouseNumberFrom = PersistenceUtil.GetNullableInt(r, prefix + "HouseNumberFrom"),
                HouseNumberTo = PersistenceUtil.GetNullableInt(r, prefix + "HouseNumberTo"),
                LetterFrom = PersistenceUtil.GetNullableString(r, prefix + "LetterFrom"),
                LetterTo = PersistenceUtil.GetNullableString(r, prefix + "LetterTo"),
                Floor = PersistenceUtil.GetNullableString(r, prefix + "Floor"),
                SideDoor = PersistenceUtil.GetNullableString(r, prefix + "SideDoor"),
                ZipCode = PersistenceUtil.GetNullableInt(r, prefix + "ZipCode"),
                PostalDistrict = PersistenceUtil.GetNullableString(r, prefix + "PostalDistrict"),
                CityName = PersistenceUtil.GetNullableString(r, prefix + "CityName"),
                MunicipalityCode = PersistenceUtil.GetNullableInt(r, prefix + "MunicipalityCode"),
                MunicipalityText = PersistenceUtil.GetNullableString(r, prefix + "MunicipalityText"),
                PostalBox = PersistenceUtil.GetNullableInt(r, prefix + "PostalBox"),
                CoName = PersistenceUtil.GetNullableString(r, prefix + "CoName"),
                AddressFreeText = PersistenceUtil.GetNullableString(r, prefix + "AddressFreeText"),
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

        public ICompanyUpdateTransaction BeginCompanyUpdateTransaction()
        {
            return new CompanyUpdateTransaction(connection.BeginTransaction(), connection);
        }

        public void InsertOrReplaceCompany(Company c)
        {
            ExecuteNonQuery("INSERT OR REPLACE INTO Company " +
                                   "(Vat, VatText, StartDate, EndDate, UpdatedDate, OptedOutForUnsolicictedAdvertising, NameValidFrom, Name, " + string.Join(",", AdressKeys("LocationAddress")) + "," + string.Join(",", AdressKeys("PostalAddress")) + ") VALUES " +
                                   "(@vat,@vatText,@startDate,@endDate,@updateDate,@OptedOutForUnsolicictedAdvertising,@nameValidFrom,@name,@" + string.Join(",@", AdressKeys("LocationAddress")) + ",@" + string.Join(",@", AdressKeys("PostalAddress")) + ")", new Dictionary<string, object>
                    {
                        {"@vat", c.VatNumber },
                        {"@vatText",  c.VatNumber.ToString()},
                        {"@startDate",  PersistenceUtil.OptionalDateTimeMillisecondsSinceEpoch(c.StartDate)},
                        {"@endDate",  PersistenceUtil.OptionalDateTimeMillisecondsSinceEpoch(c.EndDate)},
                        {"@updateDate",  PersistenceUtil.OptionalDateTimeMillisecondsSinceEpoch(c.UpdatedDate)},
                        {"@OptedOutForUnsolicictedAdvertising",  c.OptedOutForUnsolicictedAdvertising ? 1 : 0},
                        {"@nameValidFrom",  PersistenceUtil.OptionalDateTimeMillisecondsSinceEpoch(c.NameValidFrom)},
                        {"@name",  c.Name},

                        {"LocationAddress_ValidFrom",  PersistenceUtil.OptionalDateTimeMillisecondsSinceEpoch(c.LocationAddress.ValidFrom)},
                        {"LocationAddress_StreetName",  c.LocationAddress.StreetName},
                        {"LocationAddress_RoadCode",  c.LocationAddress.RoadCode},
                        {"LocationAddress_HouseNumberFrom",  c.LocationAddress.HouseNumberFrom},
                        {"LocationAddress_HouseNumberTo",  c.LocationAddress.HouseNumberTo},
                        {"LocationAddress_LetterFrom",  c.LocationAddress.LetterFrom},
                        {"LocationAddress_LetterTo",  c.LocationAddress.LetterTo},
                        {"LocationAddress_Floor",  c.LocationAddress.Floor},
                        {"LocationAddress_SideDoor",  c.LocationAddress.SideDoor},
                        {"LocationAddress_ZipCode",  c.LocationAddress.ZipCode},
                        {"LocationAddress_PostalDistrict",  c.LocationAddress.PostalDistrict},
                        {"LocationAddress_CityName",  c.LocationAddress.CityName},
                        {"LocationAddress_MunicipalityCode",  c.LocationAddress.MunicipalityCode},
                        {"LocationAddress_MunicipalityText",  c.LocationAddress.MunicipalityText},
                        {"LocationAddress_PostalBox",  c.LocationAddress.PostalBox},
                        {"LocationAddress_CoName",  c.LocationAddress.CoName},
                        {"LocationAddress_AddressFreeText",  c.LocationAddress.AddressFreeText},

                        {"PostalAddress_ValidFrom",  PersistenceUtil.OptionalDateTimeMillisecondsSinceEpoch(c.PostalAddress.ValidFrom)},
                        {"PostalAddress_StreetName",  c.PostalAddress.StreetName},
                        {"PostalAddress_RoadCode",  c.PostalAddress.RoadCode},
                        {"PostalAddress_HouseNumberFrom",  c.PostalAddress.HouseNumberFrom},
                        {"PostalAddress_HouseNumberTo",  c.PostalAddress.HouseNumberTo},
                        {"PostalAddress_LetterFrom",  c.PostalAddress.LetterFrom},
                        {"PostalAddress_LetterTo",  c.PostalAddress.LetterTo},
                        {"PostalAddress_Floor",  c.PostalAddress.Floor},
                        {"PostalAddress_SideDoor",  c.PostalAddress.SideDoor},
                        {"PostalAddress_ZipCode",  c.PostalAddress.ZipCode},
                        {"PostalAddress_PostalDistrict",  c.PostalAddress.PostalDistrict},
                        {"PostalAddress_CityName",  c.PostalAddress.CityName},
                        {"PostalAddress_MunicipalityCode",  c.PostalAddress.MunicipalityCode},
                        {"PostalAddress_MunicipalityText",  c.PostalAddress.MunicipalityText},
                        {"PostalAddress_PostalBox",  c.PostalAddress.PostalBox},
                        {"PostalAddress_CoName",  c.PostalAddress.CoName},
                        {"PostalAddress_AddressFreeText",  c.PostalAddress.AddressFreeText},
                    });
        }

        private string[] AdressKeys(string prefix)
        {
            prefix += "_";
            return new[]
            {
                prefix + "ValidFrom",
                prefix + "StreetName",
                prefix + "RoadCode",
                prefix + "HouseNumberFrom",
                prefix + "HouseNumberTo",
                prefix + "LetterFrom",
                prefix + "LetterTo",
                prefix + "Floor",
                prefix + "SideDoor",
                prefix + "ZipCode",
                prefix + "PostalDistrict",
                prefix + "CityName",
                prefix + "MunicipalityCode",
                prefix + "MunicipalityText",
                prefix + "PostalBox",
                prefix + "CoName",
                prefix + "AddressFreeText"
            };
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