using CsvHelper.Configuration;
using OpenCVR.Model;
using OpenCVR.Update.Parse.Mapping.Convert;

namespace OpenCVR.Update.Parse.Mapping
{
    internal sealed class CompanyTypeMap : CsvClassMap<CompanyType>
    {
        public CompanyTypeMap()
        {
            Map(m => m.ValidFrom).Name("virksomhedsform_gyldigFra").TypeConverter<CvrDateConverter>();
            Map(m => m.Code).Name("virksomhedsform_kode");
            Map(m => m.Text).Name("virksomhedsform_tekst").TypeConverter<NullStringConverter>();
            Map(m => m.ResponsibleDataSupplier).Name("virksomhedsform_ansvarligDataleverandoer").TypeConverter<NullStringConverter>();
        }
    }
}