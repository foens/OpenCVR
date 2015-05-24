using CsvHelper.Configuration;
using OpenCVR.Model;

namespace OpenCVR.Update.Parse.Mapping
{
    internal sealed class CompanyTypeMap : CsvClassMap<CompanyType>
    {
        public CompanyTypeMap()
        {
            Map(m => m.ValidFrom).ConvertUsing(r => ConvertUtil.ConvertDate(r.GetField("virksomhedsform_gyldigFra")));
            Map(m => m.Code).Name("virksomhedsform_kode");
            Map(m => m.Text).Name("virksomhedsform_tekst").TypeConverter<NullStringConverter>();
            Map(m => m.ResponsibleDataSupplier).Name("virksomhedsform_ansvarligDataleverandoer").TypeConverter<NullStringConverter>();
        }
    }
}