using CsvHelper.Configuration;

namespace OpenCVR.Update.Parse.Mapping
{
    internal sealed class ContactDetailMap : CsvClassMap<ContactDetail>
    {
        public ContactDetailMap(string prefix)
        {
            Map(m => m.ValidFrom).ConvertUsing(r => ConvertUtil.ConvertDate(r.GetField(prefix + "_gyldigFra")));
            Map(m => m.Value).Name(prefix + "_kontaktoplysning").TypeConverter<NullStringConverter>();
        }
    }
}