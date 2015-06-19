using System;
using System.Net;
using NLog;
using OpenCVR.Model;
using OpenCVR.Persistence;
using OpenCVR.Update.Email;
using OpenCVR.Update.Http;
using OpenCVR.Update.Parse;

namespace OpenCVR.Update
{
    class CvrUpdater : ICvrUpdater
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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

        public bool TryUpdate()
        {
            var email = emailService.GetEarliestEmailNotProccessed(persistence.GetLastProcessedEmailReceivedTime());
            if (email == null)
                return false;

            Logger.Info("Updates are available. Starting update process.");
            var zipfileUrl = extractor.ParseOutZipfileUrl(email.Text);
            var loginId = extractor.ParseOutLoginId(email.Text);

            var localZipFilePath = httpService.Download(zipfileUrl, new NetworkCredential(loginId, cvrFetchPassword));
            var cvrEntries = parser.Parse(localZipFilePath);
            UpdatePersistence(cvrEntries);
            persistence.SetLastProcessedEmailReceivedTime(email.DateTimeReceived);
            Logger.Info("Update process complete.");
            return true;
        }

        private void UpdatePersistence(CvrEntries entries)
        {
            DateTime start = DateTime.Now;
            using (var t = persistence.BeginCompanyUpdateTransaction())
            {
                foreach (var c in entries.Companies)
                    ApplyCompanyChangesToPersistence(c, t);
                t.Commit();
            }
            DateTime end = DateTime.Now;
            Logger.Info("SQL operations took {0} ms", (end - start).TotalMilliseconds);
        }

        private void ApplyCompanyChangesToPersistence(DeltaCompany c, ICompanyUpdateTransaction t)
        {
            switch (c.ModificationStatus)
            {
                case ModificationStatus.New:
                case ModificationStatus.Modified:
                    t.InsertOrReplaceCompany(c);
                    break;
                case ModificationStatus.Removed:
                    t.DeleteCompany(c.VatNumber);
                    break;
            }
        }
    }
}