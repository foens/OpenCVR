using CsvHelper.Configuration;
using OpenCVR.Model;
using OpenCVR.Update.Parse.Mapping.Convert;

namespace OpenCVR.Update.Parse.Mapping
{
    internal sealed class IndustryMap : CsvClassMap<Industry>
    {
        public IndustryMap()
        {
            Map(m => m.ValidFrom).Name("hovedbranche_gyldigFra").TypeConverter<CvrDateConverter>();
            Map(m => m.Code).Name("hovedbranche_kode");
            Map(m => m.Text).Name("hovedbranche_tekst").TypeConverter<NullStringConverter>();
        }
    }
}