using System;
using System.Net;
using NLog;
using OpenCVR.Persistence;
using OpenCVR.Update.Email;
using OpenCVR.Update.Http;
using OpenCVR.Update.Parse;

namespace OpenCVR.Update
{
    class CvrUpdater : ICvrUpdater
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly string cvrFetchPassword;
        private readonly ICvrPersistence persistence;
        private readonly IEmailService emailService;
        private readonly ICvrEmailExtractor extractor;
        private readonly IHttpService httpService;
        private readonly ICvrParser parser;

        public CvrUpdater(string cvrFetchPassword, ICvrPersistence persistence, IEmailService emailService, ICvrEmailExtractor extractor, IHttpService httpService, ICvrParser parser)
        {
            this.cvrFetchPassword = cvrFetchPassword;
            this.persistence = persistence;
            this.emailService = emailService;
            this.extractor = extractor;
            this.httpService = httpService;
            this.parser = parser;
        }

        public void TryUpdate()
        {
            var email = emailService.GetEarliestEmailNotProccessed(new DateTime());
            if (email == null)
                return;

            logger.Info("Updates are available. Starting update process.");
            var zipfileUrl = extractor.ParseOutZipfileUrl(email.Text);
            var loginId = extractor.ParseOutLoginId(email.Text);

            var localZipFilePath = httpService.Download(zipfileUrl, new NetworkCredential(loginId, cvrFetchPassword));
            var cvrEntries = parser.Parse(localZipFilePath);
            persistence.InsertCompanies(cvrEntries.Companies);
            logger.Info("Update process complete.");
        }
    }
}