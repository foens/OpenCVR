using CsvHelper.Configuration;
using OpenCVR.Model;
using OpenCVR.Update.Parse.Mapping.Convert;

namespace OpenCVR.Update.Parse.Mapping
{
    internal sealed class ContactDetailMap : CsvClassMap<ContactDetail>
    {
        public ContactDetailMap(string prefix)
        {
            Map(m => m.ValidFrom).Name(prefix + "_gyldigFra").TypeConverter<CvrDateConverter>();
            Map(m => m.Value).Name(prefix + "_kontaktoplysning").TypeConverter<NullStringConverter>();
        }
    }
}