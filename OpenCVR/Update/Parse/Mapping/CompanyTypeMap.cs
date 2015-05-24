using CsvHelper.Configuration;

namespace OpenCVR.Update.Parse.Mapping
{
    internal sealed class CompanyTypeMap : CsvClassMap<CompanyType>
    {
        public CompanyTypeMap()
        {
            Map(m => m.ValidFrom).ConvertUsing(r => ConvertUtil.ConvertDate(r.GetField("virksomhedsform_gyldigFra")));
            Map(m => m.Code).Name("virksomhedsform_kode");
            Map(m => m.Text).Name("virksomhedsform_tekst");
            Map(m => m.ResponsibleDataSupplier).Name("virksomhedsform_ansvarligDataleverandoer");
        }
    }
}