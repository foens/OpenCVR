using CsvHelper.Configuration;
using OpenCVR.Model;

namespace OpenCVR.Update.Parse.Mapping
{
    internal sealed class IndustryMap : CsvClassMap<Industry>
    {
        public IndustryMap()
        {
            Map(m => m.ValidFrom).ConvertUsing(r => ConvertUtil.ConvertDate(r.GetField("hovedbranche_gyldigFra")));
            Map(m => m.Code).Name("hovedbranche_kode");
            Map(m => m.Text).Name("hovedbranche_tekst").TypeConverter<NullStringConverter>();
        }
    }
}