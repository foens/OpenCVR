using System;
using Microsoft.Exchange.WebServices.Data;
using NUnit.Framework;
using OpenCVR.Update.Email;

namespace OpenCVR.Test.Unit.Update.Email
{
    [Ignore] // Requires active internet connection, and valid credentials
    [TestFixture]
    public class EmailServiceTests
    {
        private EmailService emailService;

        [SetUp]
        public void Setup()
        {
            emailService = new EmailService(new WebCredentials("kasper.foens", SecretConfigurationFetcher.ReadEchangeServicePassword()), "kasper.foens@cgm.com");
        }

        [Test]
        public void TestCanConnectToEmailService()
        {
            emailService.GetEarliestEmailNotProccessed(new DateTime(2015));
        }
    }
}
