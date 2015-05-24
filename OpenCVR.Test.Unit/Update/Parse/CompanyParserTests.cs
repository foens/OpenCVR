using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenCVR.Model;
using OpenCVR.Model.Occupation;
using OpenCVR.Update.Parse;

namespace OpenCVR.Test.Unit.Update.Parse
{
    [TestFixture]
    public class CompanyParserTests
    {
        const string Csv = "modifikationsstatus,cvrnr,livsforloeb_startdato,livsforloeb_ophoersdato,ajourfoeringsdato,reklamebeskyttelse,navn_gyldigFra,navn_tekst,beliggenhedsadresse_gyldigFra,beliggenhedsadresse_vejnavn,beliggenhedsadresse_vejkode,beliggenhedsadresse_husnummerFra,beliggenhedsadresse_husnummerTil,beliggenhedsadresse_bogstavFra,beliggenhedsadresse_bogstavTil,beliggenhedsadresse_etage,beliggenhedsadresse_sidedoer,beliggenhedsadresse_postnr,beliggenhedsadresse_postdistrikt,beliggenhedsadresse_bynavn,beliggenhedsadresse_kommune_kode,beliggenhedsadresse_kommune_tekst,beliggenhedsadresse_postboks,beliggenhedsadresse_coNavn,beliggenhedsadresse_adresseFritekst,postadresse_gyldigFra,postadresse_vejnavn,postadresse_vejkode,postadresse_husnummerFra,postadresse_husnummerTil,postadresse_bogstavFra,postadresse_bogstavTil,postadresse_etage,postadresse_sidedoer,postadresse_postnr,postadresse_postdistrikt,postadresse_bynavn,postadresse_kommune_kode,postadresse_kommune_tekst,postadresse_postboks,postadresse_coNavn,postadresse_adresseFritekst,virksomhedsform_gyldigFra,virksomhedsform_kode,virksomhedsform_tekst,virksomhedsform_ansvarligDataleverandoer,hovedbranche_gyldigFra,hovedbranche_kode,hovedbranche_tekst,bibranche1_gyldigFra,bibranche1_kode,bibranche1_tekst,bibranche2_gyldigFra,bibranche2_kode,bibranche2_tekst,bibranche3_gyldigFra,bibranche3_kode,bibranche3_tekst,telefonnummer_gyldigFra,telefonnummer_kontaktoplysning,telefax_gyldigFra,telefax_kontaktoplysning,email_gyldigFra,email_kontaktoplysning,kreditoplysninger_gyldigFra,kreditoplysninger_tekst,aarsbeskaeftigelse_aar,aarsbeskaeftigelse_antalAnsatte,aarsbeskaeftigelse_antalAnsatteInterval,aarsbeskaeftigelse_antalAarsvaerk,aarsbeskaeftigelse_antalAarsvaerkInterval,aarsbeskaeftigelse_antalInclEjere,aarsbeskaeftigelse_antalInclEjereInterval,kvartalsbeskaeftigelse_aar,kvartalsbeskaeftigelse_kvartal,kvartalsbeskaeftigelse_antalAnsatte,kvartalsbeskaeftigelse_antalAnsatteInterval,kvartalsbeskaeftigelse_antalAarsvaerk,kvartalsbeskaeftigelse_antalAarsvaerkInterval,maanedsbeskaeftigelse_aar,maanedsbeskaeftigelse_maaned,maanedsbeskaeftigelse_antalAnsatte,maanedsbeskaeftigelse_antalAnsatteInterval,maanedsbeskaeftigelse_antalAarsvaerk,maanedsbeskaeftigelse_antalAarsvaerkInterval,produktionsenheder,deltagere\n" +
            "ny,10538475,01-05-1967,,24-06-2014,0,01-05-1967,MOGENS CHRISTOFFERSEN,01-01-2007,Giesegårdvej,78,115,,,,,,4100,Ringsted            ,Gørslev,259,KØGE,,,,,,,,,,,,,,,,,,,,,,10,Enkeltmandsvirksomhed,T&S,01-01-2008,11100,\"Dyrkning af korn(undtagen ris), bælgfrugter og olieholdige frø\",,,,,,,,,,23-01-2000,56879257,01-01-2000,45828501,09-09-2009,lars.hoej@lhi.dk,,,2012,,0,,0,,1,,,,,,,,,,,,,1000092684,4000100945\n" +
            "fjernet,21138681,30-09-1998,12-05-2015,12-05-2015,1,30-09-1998,JOHNNY JENSEN,30-09-1998,Falkoner Alle,220,26,26,A,B,st,1,2000,Frederiksberg,,147,FREDERIKSBERG,570,Elin Holst,Aarhus Universitetshospital,31-10-2013,Danmarksgade,1220,72,,,,st,,9100,Aalborg,,851,AALBORG,1438,,,,80,Anpartsselskab,E&S,03-05-2003,439990,\"Anden bygge- og anlægsvirksomhed, som kræver specialisering\",,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\"1002664349,1013707274,1013727909,1013736460,1013736525,1013736541\",\n" +
            "modificeret,21138681,30-09-1998,12-05-2015,12-05-2015,1,30-09-1998,JOHNNY JENSEN,30-09-1998,Falkoner Alle,220,26,26,A,B,st,1,2000,Frederiksberg,,147,FREDERIKSBERG,570,Elin Holst,Aarhus Universitetshospital,31-10-2013,Danmarksgade,1220,72,,,,st,,9100,Aalborg,,851,AALBORG,1438,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\"1002664349,1013707274,1013727909,1013736460,1013736525,1013736541\",";

        [Test]
        public void TestCanReadModificationStatus()
        {
            var items = ParseCsv();
            Assert.AreEqual(ModificationStatus.New, items[0].ModificationStatus);
            Assert.AreEqual(ModificationStatus.Removed, items[1].ModificationStatus);
            Assert.AreEqual(ModificationStatus.Modified, items[2].ModificationStatus);
        }

        [Test]
        public void TestCanReadVatNumber()
        {
            var items = ParseCsv();
            Assert.AreEqual(10538475, items[0].VatNumber);
            Assert.AreEqual(21138681, items[1].VatNumber);
        }

        [Test]
        public void TestCanReadStartDate()
        {
            var items = ParseCsv();
            Assert.AreEqual(new DateTime(1967, 05, 01), items[0].StartDate);
            Assert.AreEqual(new DateTime(1998, 09, 30), items[1].StartDate);
        }

        [Test]
        public void TestCanReadEndDate()
        {
            var items = ParseCsv();
            Assert.AreEqual(null, items[0].EndDate);
            Assert.AreEqual(new DateTime(2015, 05, 12), items[1].EndDate);
        }

        [Test]
        public void TestCanReadDataUpdatedDate()
        {
            var items = ParseCsv();
            Assert.AreEqual(new DateTime(2014, 06, 24), items[0].UpdatedDate);
            Assert.AreEqual(new DateTime(2015, 05, 12), items[1].UpdatedDate);
        }

        [Test]
        public void TestCanReadOptedOutForUnsolicictedAdvertising()
        {
            var items = ParseCsv();
            Assert.AreEqual(false, items[0].OptedOutForUnsolicictedAdvertising);
            Assert.AreEqual(true, items[1].OptedOutForUnsolicictedAdvertising);
        }

        [Test]
        public void TestCanReadNameValidFrom()
        {
            var items = ParseCsv();
            Assert.AreEqual(new DateTime(1967, 05, 01), items[0].NameValidFrom);
            Assert.AreEqual(new DateTime(1998, 09, 30), items[1].NameValidFrom);
        }

        [Test]
        public void TestCanReadName()
        {
            var items = ParseCsv();
            Assert.AreEqual("MOGENS CHRISTOFFERSEN", items[0].Name);
            Assert.AreEqual("JOHNNY JENSEN", items[1].Name);
        }

        [Test]
        public void TestCanReadLocationAddress()
        {
            var items = ParseCsv();
            Assert.AreEqual(new Address
            {
                ValidFrom = new DateTime(2007, 01, 01),
                StreetName = "Giesegårdvej",
                RoadCode = 78,
                HouseNumberFrom = 115,
                HouseNumberTo = null,
                LetterFrom = null,
                LetterTo = null,
                Floor = null,
                SideDoor = null,
                ZipCode = 4100,
                PostalDisrict = "Ringsted",
                CityName = "Gørslev",
                MunicipalityCode = 259,
                MunicipalityText = "KØGE",
                PostalBox = null,
                CoName = null,
                AddressFreeText = null
            }, items[0].LocationAddress);
            Assert.AreEqual(new Address
            {
                ValidFrom = new DateTime(1998, 09, 30),
                StreetName = "Falkoner Alle",
                RoadCode = 220,
                HouseNumberFrom = 26,
                HouseNumberTo = 26,
                LetterFrom = "A",
                LetterTo = "B",
                Floor = "st",
                SideDoor = "1",
                ZipCode = 2000,
                PostalDisrict = "Frederiksberg",
                CityName = null,
                MunicipalityCode = 147,
                MunicipalityText = "FREDERIKSBERG",
                PostalBox = 570,
                CoName = "Elin Holst",
                AddressFreeText = "Aarhus Universitetshospital"
            }, items[1].LocationAddress);
        }

        [Test]
        public void TestCanReadPostalAddress()
        {
            var items = ParseCsv();
            Assert.AreEqual(new Address
            {
                ValidFrom = null,
                StreetName = null,
                RoadCode = null,
                HouseNumberFrom = null,
                HouseNumberTo = null,
                LetterFrom = null,
                LetterTo = null,
                Floor = null,
                SideDoor = null,
                ZipCode = null,
                PostalDisrict = null,
                CityName = null,
                MunicipalityCode = null,
                MunicipalityText = null,
                PostalBox = null,
                CoName = null,
                AddressFreeText = null
            }, items[0].PostalAddress);
            Assert.AreEqual(new Address
            {
                ValidFrom = new DateTime(2013, 10, 31),
                StreetName = "Danmarksgade",
                RoadCode = 1220,
                HouseNumberFrom = 72,
                HouseNumberTo = null,
                LetterFrom = null,
                LetterTo = null,
                Floor = "st",
                SideDoor = null,
                ZipCode = 9100,
                PostalDisrict = "Aalborg",
                CityName = null,
                MunicipalityCode = 851,
                MunicipalityText = "AALBORG",
                PostalBox = 1438,
                CoName = null,
                AddressFreeText = null
            }, items[1].PostalAddress);
        }

        [Test]
        public void TestCanReadCompanyType()
        {
            var items = ParseCsv();
            Assert.AreEqual(new CompanyType
            {
                ValidFrom = null,
                Code = 10,
                Text = "Enkeltmandsvirksomhed",
                ResponsibleDataSupplier = "T&S"
            }, items[0].CompanyType);
            Assert.AreEqual(new CompanyType
            {
                ValidFrom = null,
                Code = 80,
                Text = "Anpartsselskab",
                ResponsibleDataSupplier = "E&S"
            }, items[1].CompanyType);
            Assert.AreEqual(new CompanyType
            {
                ValidFrom = null,
                Code = null,
                Text = null,
                ResponsibleDataSupplier = null
            }, items[2].CompanyType);
        }

        [Test]
        public void TestCanReadMainIndustry()
        {
            var items = ParseCsv();
            Assert.AreEqual(new Industry
            {
                ValidFrom = new DateTime(2008, 01, 01),
                Code = 11100,
                Text = "Dyrkning af korn(undtagen ris), bælgfrugter og olieholdige frø"
            }, items[0].MainIndustry);
            Assert.AreEqual(new Industry
            {
                ValidFrom = new DateTime(2003, 05, 03),
                Code = 439990,
                Text = "Anden bygge- og anlægsvirksomhed, som kræver specialisering"
            }, items[1].MainIndustry);
            Assert.AreEqual(new Industry
            {
                ValidFrom = null,
                Code = null,
                Text = null
            }, items[2].MainIndustry);
        }

        [Test]
        [Ignore]
        public void TestCanReadCarIndustryCodes()
        {
            
        }

        [Test]
        public void TestCanReadTelehpone()
        {
            var items = ParseCsv();
            Assert.AreEqual(new ContactDetail
            {
                ValidFrom = new DateTime(2000, 01, 23),
                Value = "56879257"
            }, items[0].TelephoneContactDetails);
            Assert.AreEqual(new ContactDetail
            {
                ValidFrom = null,
                Value = null
            }, items[1].TelephoneContactDetails);
        }

        [Test]
        public void TestCanReadFax()
        {
            var items = ParseCsv();
            Assert.AreEqual(new ContactDetail
            {
                ValidFrom = new DateTime(2000, 01, 01),
                Value = "45828501"
            }, items[0].FaxContactDetails);
            Assert.AreEqual(new ContactDetail
            {
                ValidFrom = null,
                Value = null
            }, items[1].FaxContactDetails);
        }

        [Test]
        public void TestCanReadEmail()
        {
            var items = ParseCsv();
            Assert.AreEqual(new ContactDetail
            {
                ValidFrom = new DateTime(2009, 09, 09),
                Value = "lars.hoej@lhi.dk"
            }, items[0].EmailContactDetails);
            Assert.AreEqual(new ContactDetail
            {
                ValidFrom = null,
                Value = null
            }, items[1].EmailContactDetails);
        }

        [Test]
        public void TestCanReadCreditDetails()
        {
            var items = ParseCsv();
            Assert.AreEqual(null, items[0].CreditDetailsValidFrom);
            Assert.AreEqual(null, items[0].CreditDetails);
        }

        [Test]
        [Ignore]
        public void TestCanReadOccupationStatistics()
        {
            // TODO
            var items = ParseCsv();
            
            Assert.AreEqual(new OccupationStatistics
            {
                YearlyStatistic = new YearlyOccupationStatistic
                {
                    Year = 1923,
                    Employees = 2,
                    EmployeeInterval = new Interval
                    {
                        Min = 2,
                        Max = 6
                    },
                    ManYears = 9,
                    ManYearsInterval = new Interval
                    {
                        Min = 9,
                        Max = 3
                    },
                    Owners = 5
                },
                QuarterlyStatistic = new QuaterlyOccupationStatistic
                {
                    Year = 1234,
                    Quater = 1,
                    Employees = 2,
                    EmployeeInterval = new Interval
                    {
                        Min = 3,
                        Max = 7
                    },
                    ManYears = 9,
                    ManYearsInterval = new Interval
                    {
                        Min = 123,
                        Max = 999
                    },
                },
                MonthlyStatistic = new MonthlyOccupationStatistic
                {
                    Year = 123,
                    Month = 3,
                    Employees = 2,
                    EmployeeInterval = new Interval
                    {
                        Min = 2,
                        Max = 3
                    },
                    ManYears = 9,
                    ManYearsInterval = new Interval
                    {
                        Min = 3,
                        Max = 4
                    },
                }
            }, items[0].OccupationStatistics);
        }

        [Test]
        public void TestCanReadProductionUnits()
        {
            var items = ParseCsv();

            CollectionAssert.AreEqual(new long[]
            {
                1000092684
            }, items[0].ProductionUnits);
            CollectionAssert.AreEqual(new long[]
            {
                1002664349,1013707274,1013727909,1013736460,1013736525,1013736541
            }, items[1].ProductionUnits);
        }

        [Test]
        public void TestCanReadParticipants()
        {
            var items = ParseCsv();

            CollectionAssert.AreEqual(new long[]
            {
                4000100945
            }, items[0].Participants);
            CollectionAssert.AreEqual(new long[0], items[1].Participants);
        }

        private static List<DeltaCompany> ParseCsv()
        {
            var parser = new CompanyParser();
            var utf8 = Encoding.UTF8;
            var items = parser.Parse(new StreamReader(new MemoryStream(utf8.GetBytes(Csv)), utf8, true)).ToList();
            Assert.AreEqual(3, items.Count);
            return items;
        }
    }
}
