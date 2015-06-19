using System;
using System.Net;
using System.Transactions;
using Moq;
using NUnit.Framework;
using OpenCVR.Model;
using OpenCVR.Persistence;
using OpenCVR.Update;
using OpenCVR.Update.Email;
using OpenCVR.Update.Http;
using OpenCVR.Update.Parse;

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
            var date = DateTime.Now;
            persistence.Setup(e => e.GetLastProcessedEmailReceivedTime()).Returns(date);
            emailService.Setup(e => e.GetEarliestEmailNotProccessed(date)).Returns((CvrEmail) null);

            var updateApplied = cvrUpdater.TryUpdate();

            emailService.Verify(e => e.GetEarliestEmailNotProccessed(date));
            Assert.IsFalse(updateApplied);
        }

        [Test]
        public void TestWhenCvrEmailReturnedTheCvrDatabaseIsUpadeted()
        {
            var date = DateTime.Now;
            CvrEmail email = new CvrEmail("id", DateTime.Now.AddDays(-2), "foobar");
            var uri = new Uri("https://foobar.com/file.zip");
            const string loginId = "123533";
            const string path = "some download path";
            var deltaCompanies = new []
            {
                new DeltaCompany
                {
                    ModificationStatus = ModificationStatus.New,
                    VatNumber = 999
                },
                new DeltaCompany
                {
                    ModificationStatus = ModificationStatus.New,
                    VatNumber = 123
                }

            };
            var cvrEntries = new CvrEntries
            {
                Companies = deltaCompanies
            };
            persistence.Setup(e => e.GetLastProcessedEmailReceivedTime()).Returns(date);
            emailService.Setup(e => e.GetEarliestEmailNotProccessed(date)).Returns(email);
            emailExtractor.Setup(e => e.ParseOutZipfileUrl(email.Text)).Returns(uri);
            emailExtractor.Setup(e => e.ParseOutLoginId(email.Text)).Returns(loginId);
            httpService.Setup(e => e.Download(uri, It.Is<NetworkCredential>(nw => nw.UserName.Equals(loginId) && nw.Password.Equals(Password)))).Returns(path);
            parser.Setup(e => e.Parse(path)).Returns(cvrEntries);
            var transaction = new Mock<ICompanyUpdateTransaction>();
            persistence.Setup(e => e.BeginCompanyUpdateTransaction()).Returns(transaction.Object);

            var updateApplied = cvrUpdater.TryUpdate();

            persistence.Verify(e => e.InsertOrReplaceCompany(deltaCompanies[0]));
            persistence.Verify(e => e.InsertOrReplaceCompany(deltaCompanies[1]));
            persistence.Verify(e => e.SetLastProcessedEmailReceivedTime(email.DateTimeReceived));
            transaction.Verify(e => e.Commit());
            Assert.IsTrue(updateApplied);
        }

        [Test]
        public void TestDeletedCompaniesAreRemoved()
        {
            var date = DateTime.Now;
            CvrEmail email = new CvrEmail("id", DateTime.Now.AddDays(-2), "foobar");
            var uri = new Uri("https://foobar.com/file.zip");
            const string loginId = "123533";
            const string path = "some download path";
            var deltaCompanies = new[] { new DeltaCompany
            {
                ModificationStatus = ModificationStatus.Removed,
                VatNumber = 123
            } };
            var cvrEntries = new CvrEntries
            {
                Companies = deltaCompanies
            };
            persistence.Setup(e => e.GetLastProcessedEmailReceivedTime()).Returns(date);
            emailService.Setup(e => e.GetEarliestEmailNotProccessed(date)).Returns(email);
            emailExtractor.Setup(e => e.ParseOutZipfileUrl(email.Text)).Returns(uri);
            emailExtractor.Setup(e => e.ParseOutLoginId(email.Text)).Returns(loginId);
            httpService.Setup(e => e.Download(uri, It.Is<NetworkCredential>(nw => nw.UserName.Equals(loginId) && nw.Password.Equals(Password)))).Returns(path);
            parser.Setup(e => e.Parse(path)).Returns(cvrEntries);
            var transaction = new Mock<ICompanyUpdateTransaction>();
            persistence.Setup(e => e.BeginCompanyUpdateTransaction()).Returns(transaction.Object);

            cvrUpdater.TryUpdate();

            persistence.Verify(e => e.DeleteCompany(deltaCompanies[0].VatNumber));
        }

        [Test]
        public void TestDeletedCompaniesAreModified()
        {
            var date = DateTime.Now;
            CvrEmail email = new CvrEmail("id", DateTime.Now.AddDays(-2), "foobar");
            var uri = new Uri("https://foobar.com/file.zip");
            const string loginId = "123533";
            const string path = "some download path";
            var deltaCompanies = new[] { new DeltaCompany
            {
                ModificationStatus = ModificationStatus.Modified,
                VatNumber = 123
            } };
            var cvrEntries = new CvrEntries
            {
                Companies = deltaCompanies
            };
            persistence.Setup(e => e.GetLastProcessedEmailReceivedTime()).Returns(date);
            emailService.Setup(e => e.GetEarliestEmailNotProccessed(date)).Returns(email);
            emailExtractor.Setup(e => e.ParseOutZipfileUrl(email.Text)).Returns(uri);
            emailExtractor.Setup(e => e.ParseOutLoginId(email.Text)).Returns(loginId);
            httpService.Setup(e => e.Download(uri, It.Is<NetworkCredential>(nw => nw.UserName.Equals(loginId) && nw.Password.Equals(Password)))).Returns(path);
            parser.Setup(e => e.Parse(path)).Returns(cvrEntries);
            var transaction = new Mock<ICompanyUpdateTransaction>();
            persistence.Setup(e => e.BeginCompanyUpdateTransaction()).Returns(transaction.Object);

            cvrUpdater.TryUpdate();

            persistence.Verify(e => e.InsertOrReplaceCompany(deltaCompanies[0]));
        }
    }
}
