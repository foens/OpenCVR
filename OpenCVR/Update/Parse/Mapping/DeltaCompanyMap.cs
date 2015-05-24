using System;
using CsvHelper.Configuration;
using OpenCVR.Model;
using OpenCVR.Update.Parse.Mapping.Convert;

namespace OpenCVR.Update.Parse.Mapping
{
    internal sealed class DeltaCompanyMap : CsvClassMap<DeltaCompany>
    {
        public DeltaCompanyMap()
        {
            Map(m => m.ModificationStatus).Name("modifikationsstatus").TypeConverter<ModificationStatusConverter>();
            Map(m => m.VatNumber).Name("cvrnr");
            Map(m => m.StartDate).Name("livsforloeb_startdato").TypeConverter<CvrDateConverter>();
            Map(m => m.StartDate).Name("livsforloeb_startdato").TypeConverter<CvrDateConverter>();
            Map(m => m.EndDate).Name("livsforloeb_ophoersdato").TypeConverter<CvrDateConverter>();
            Map(m => m.UpdatedDate).Name("ajourfoeringsdato").TypeConverter<CvrDateConverter>();
            Map(m => m.OptedOutForUnsolicictedAdvertising).Name("reklamebeskyttelse");
            Map(m => m.NameValidFrom).Name("navn_gyldigFra").TypeConverter<CvrDateConverter>();
            Map(m => m.Name).Name("navn_tekst");
            References<AddressMap>(m => m.LocationAddress, "beliggenhedsadresse");
            References<AddressMap>(m => m.PostalAddress, "postadresse");
            References<CompanyTypeMap>(m => m.CompanyType);
            References<IndustryMap>(m => m.MainIndustry);
            References<ContactDetailMap>(m => m.TelephoneContactDetails, "telefonnummer");
            References<ContactDetailMap>(m => m.FaxContactDetails, "telefax");
            References<ContactDetailMap>(m => m.EmailContactDetails, "email");
            Map(m => m.ProductionUnits).Name("produktionsenheder").TypeConverter<LongArrayConverter>();
            Map(m => m.Participants).Name("deltagere").TypeConverter<LongArrayConverter>();
        }
    }
}
