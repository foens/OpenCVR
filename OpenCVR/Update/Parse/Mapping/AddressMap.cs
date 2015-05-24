using CsvHelper.Configuration;
using OpenCVR.Update.Parse.Model;

namespace OpenCVR.Update.Parse.Mapping
{
    internal sealed class AddressMap : CsvClassMap<Address>
    {
        public AddressMap(string prefix)
        {
            Map(m => m.ValidFrom).ConvertUsing(r => ConvertUtil.ConvertDate(r.GetField(prefix + "_gyldigFra")));
            Map(m => m.StreetName).Name(prefix + "_vejnavn").TypeConverter<NullStringConverter>();
            Map(m => m.RoadCode).Name(prefix + "_vejkode");
            Map(m => m.HouseNumberFrom).Name(prefix + "_husnummerFra");
            Map(m => m.HouseNumberTo).Name(prefix + "_husnummerTil");
            Map(m => m.LetterFrom).Name(prefix + "_bogstavFra").TypeConverter<NullStringConverter>();
            Map(m => m.LetterTo).Name(prefix + "_bogstavTil").TypeConverter<NullStringConverter>();
            Map(m => m.Floor).Name(prefix + "_etage").TypeConverter<NullStringConverter>();
            Map(m => m.SideDoor).Name(prefix + "_sidedoer").TypeConverter<NullStringConverter>();
            Map(m => m.ZipCode).Name(prefix + "_postnr");
            Map(m => m.PostalDisrict).Name(prefix + "_postdistrikt").TypeConverter<NullStringConverter>();
            Map(m => m.CityName).Name(prefix + "_bynavn").TypeConverter<NullStringConverter>();
            Map(m => m.MunicipalityCode).Name(prefix + "_kommune_kode");
            Map(m => m.MunicipalityText).Name(prefix + "_kommune_tekst").TypeConverter<NullStringConverter>();
            Map(m => m.PostalBox).Name(prefix + "_postboks");
            Map(m => m.CoName).Name(prefix + "_coNavn").TypeConverter<NullStringConverter>();
            Map(m => m.AddressFreeText).Name(prefix + "_adresseFritekst").TypeConverter<NullStringConverter>();
        }
    }
}