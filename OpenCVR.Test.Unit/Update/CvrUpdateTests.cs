using System;
using System.Net;
using Moq;
using NUnit.Framework;
using OpenCVR.Persistence;
using OpenCVR.Update;
using OpenCVR.Update.Email;
using OpenCVR.Update.Http;
using OpenCVR.Update.Parse;
using OpenCVR.Update.Parse.Model;

namespace OpenCVR.Test.Unit.Update
{
    [TestFixture]
    public class CvrUpdateTests
    {
        private const string Password = "password";
        private Mock<ICvrPersistence> persistence;
        private Mock<IEmailService> emailService;
        private Mock<ICvrEmailExtractor> emailExtractor;
        private Mock<IHttpService> httpService;
        private Mock<ICvrParser> parser;
        private CvrUpdater cvrUpdater;
        

        [SetUp]
        public void Setup()
        {
            persistence = new Mock<ICvrPersistence>();
            emailService = new Mock<IEmailService>();
            emailExtractor = new Mock<ICvrEmailExtractor>();
            httpService = new Mock<IHttpService>();
            parser = new Mock<ICvrParser>();
            cvrUpdater = new CvrUpdater(Password, persistence.Object, emailService.Object, emailExtractor.Object, httpService.Object, parser.Object);
        }

        [Test]
        public void TestFetchingEmailWhichReturnsNullDoesNoting()
        {
            emailService.Setup(e => e.GetEarliestEmailNotProccessed(new DateTime())).Returns((CvrEmail) null);

            cvrUpdater.TryUpdate();

            emailService.Verify(e => e.GetEarliestEmailNotProccessed(new DateTime()));
        }

        [Test]
        public void TestWhenCvrEmailReturnedTheCvrDatabaseIsUpadeted()
        {
            CvrEmail email = new CvrEmail("id", DateTime.Now, "foobar");
            var uri = new Uri("https://foobar.com/file.zip");
            const string loginId = "123533";
            const string path = "some download path";
            var deltaCompanies = new [] {new DeltaCompany() };
            var cvrEntries = new CvrEntries
            {
                Companies = deltaCompanies
            };
            emailService.Setup(e => e.GetEarliestEmailNotProccessed(new DateTime())).Returns(email);
            emailExtractor.Setup(e => e.ParseOutZipfileUrl(email.Text)).Returns(uri);
            emailExtractor.Setup(e => e.ParseOutLoginId(email.Text)).Returns(loginId);
            httpService.Setup(e => e.Download(uri, It.Is<NetworkCredential>(nw => nw.UserName.Equals(loginId) && nw.Password.Equals(Password)))).Returns(path);
            parser.Setup(e => e.Parse(path)).Returns(cvrEntries);

            cvrUpdater.TryUpdate();
            
            persistence.Verify(e => e.InsertCompanies(cvrEntries.Companies));
        }
    }
}
