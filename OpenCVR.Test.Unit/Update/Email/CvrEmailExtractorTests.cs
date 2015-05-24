using System;
using NUnit.Framework;
using OpenCVR.Update.Email;

namespace OpenCVR.Test.Unit.Update.Email
{
    [TestFixture]
    public class CvrEmailExtractorTests
    {
        private CvrEmailExtractor extractor;
        private const string EmailText = "<html>\r\n<head>\r\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">\r\n</head>\r\n<body>\r\n<p>Kære Kasper Føns</p>\r\n<p>Du modtager denne mail, da du har bestilt et udtræk på <a href=\"http://data.virk.dk\" target=\"_blank\">\r\ndata.virk.dk</a>, som nu er klar til download.</p>\r\n<p>Udtrækket kan hentes her: <a href=\"https://data.cvr.dk/39258347_54715_20150514001646.zip\" target=\"_blank\">\r\nhttps://data.cvr.dk/39258347_54715_20150514001646.zip</a></p>\r\n<p>For at tilgå dit udtræk, skal du benytte brugerId &quot;39258347&quot; og det password, som du anvender til at logge på data.virk.dk<br>\r\n</p>\r\n<p>Hvis du ikke ønsker at modtage data længere, skal du under Mine abonnementer og enkeltudtræk markere det abonnement, du ønsker at ophøre, vælg handlinger og vælg slet/ophør.<br>\r\n</p>\r\nVenlig hilsen<br>\r\nErhvervsstyrelsen<br>\r\n</body>\r\n</html>\r\n";

        [SetUp]
        public void Setup()
        {
            extractor = new CvrEmailExtractor();
        }

        [Test]
        public void TestCanParseOutZipFile()
        {
            Assert.AreEqual(new Uri("https://data.cvr.dk/39258347_54715_20150514001646.zip"), extractor.ParseOutZipfileUrl(EmailText));
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void TestDoesNotParseOutZipFileIfWrongHostname()
        {
            extractor.ParseOutZipfileUrl(EmailText.Replace("data.cvr.dk", "malicious.host"));
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void TestDoesNotParseOutZipFileIfNotUsingHttps()
        {
            extractor.ParseOutZipfileUrl(EmailText.Replace("https://", "http://"));
        }

        [Test]
        public void ParseOutLoginId()
        {
            Assert.AreEqual("39258347", extractor.ParseOutLoginId(EmailText));
        }
    }
}
