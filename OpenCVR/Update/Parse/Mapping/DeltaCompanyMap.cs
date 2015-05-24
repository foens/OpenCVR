using System;
using CsvHelper.Configuration;
using OpenCVR.Model;

namespace OpenCVR.Update.Parse.Mapping
{
    internal sealed class DeltaCompanyMap : CsvClassMap<DeltaCompany>
    {
        public DeltaCompanyMap()
        {
            Map(m => m.ModificationStatus).ConvertUsing(r => ConvertModificationStatus(r.GetField("modifikationsstatus")));
            Map(m => m.VatNumber).Name("cvrnr");
            Map(m => m.StartDate).ConvertUsing(r => ConvertUtil.ConvertDate(r.GetField("livsforloeb_startdato")));
            Map(m => m.EndDate).ConvertUsing(r => ConvertUtil.ConvertDate(r.GetField("livsforloeb_ophoersdato")));
            Map(m => m.UpdatedDate).ConvertUsing(r => ConvertUtil.ConvertDate(r.GetField("ajourfoeringsdato")));
            Map(m => m.OptedOutForUnsolicictedAdvertising).Name("reklamebeskyttelse");
            Map(m => m.NameValidFrom).ConvertUsing(r => ConvertUtil.ConvertDate(r.GetField("navn_gyldigFra")));
            Map(m => m.Name).Name("navn_tekst");
            References<AddressMap>(m => m.LocationAddress, "beliggenhedsadresse");
            References<AddressMap>(m => m.PostalAddress, "postadresse");
            References<CompanyTypeMap>(m => m.CompanyType);
            References<IndustryMap>(m => m.MainIndustry);
            References<ContactDetailMap>(m => m.TelephoneContactDetails, "telefonnummer");
            References<ContactDetailMap>(m => m.FaxContactDetails, "telefax");
            References<ContactDetailMap>(m => m.EmailContactDetails, "email");
            Map(m => m.ProductionUnits).ConvertUsing(r => ConvertLongArray(r.GetField("produktionsenheder")));
            Map(m => m.Participants).ConvertUsing(r => ConvertLongArray(r.GetField("deltagere")));
        }

        private long[] ConvertLongArray(string value)
        {
            if (string.IsNullOrEmpty(value))
                return new long[0];
            var presumeablyLongs = value.Split(',');
            var r = new long[presumeablyLongs.Length];
            for (int i = 0; i < presumeablyLongs.Length; i++)
                r[i] = long.Parse(presumeablyLongs[i]);
            return r;
        }

        private static ModificationStatus ConvertModificationStatus(string value)
        {
            switch (value)
            {
                case "ny":
                    return ModificationStatus.New;
                case "fjernet":
                    return ModificationStatus.Removed;
                case "modificeret":
                    return ModificationStatus.Modified;
            }
            throw new Exception("Unknown ModificationStatus: " + value);
        }
    }
}
