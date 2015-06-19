using System.Collections.Generic;
using System.Data.Common;
using OpenCVR.Model;

namespace OpenCVR.Persistence
{
    internal class CompanyUpdateTransaction : ICompanyUpdateTransaction
    {
        private readonly DbTransaction transaction;
        private readonly DbCommand insertOrReplaceCommand;
        private readonly DbCommand deleteCommand;

        public CompanyUpdateTransaction(DbTransaction transaction, DbConnection connection)
        {
            this.transaction = transaction;
            insertOrReplaceCommand = PersistenceUtil.CreateCommand(connection, "INSERT OR REPLACE INTO Company " +
                                   "(Vat, VatText, StartDate, EndDate, UpdatedDate, OptedOutForUnsolicictedAdvertising, NameValidFrom, Name, " + string.Join(",", AdressKeys("LocationAddress")) + "," + string.Join(",", AdressKeys("PostalAddress")) + ") VALUES " +
                                   "(@vat,@vatText,@startDate,@endDate,@updateDate,@OptedOutForUnsolicictedAdvertising,@nameValidFrom,@name,@" + string.Join(",@", AdressKeys("LocationAddress")) + ",@" + string.Join(",@", AdressKeys("PostalAddress")) + ")");
            insertOrReplaceCommand.Prepare();
            deleteCommand = PersistenceUtil.CreateCommand(connection, "DELETE FROM Company WHERE Vat = @vat");
            deleteCommand.Prepare();
        }

        private static string[] AdressKeys(string prefix)
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

        public void Dispose()
        {
            insertOrReplaceCommand.Dispose();
            deleteCommand.Dispose();
            transaction.Dispose();
        }

        public void Commit()
        {
            transaction.Commit();
        }

        public void InsertOrReplaceCompany(Company c)
        {
            insertOrReplaceCommand.Parameters.Clear();
            PersistenceUtil.AddParametersToCommand(insertOrReplaceCommand, new Dictionary<string, object>
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
            insertOrReplaceCommand.ExecuteNonQuery();
        }

        public void DeleteCompany(int vatNumber)
        {
            deleteCommand.Parameters.Clear();
            PersistenceUtil.AddParametersToCommand(deleteCommand, new Dictionary<string, object>
                    {
                        {"@vat", vatNumber }
                    });
            deleteCommand.ExecuteNonQuery();
        }
    }
}