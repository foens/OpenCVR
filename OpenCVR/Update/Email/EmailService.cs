using System;
using System.Collections.Generic;
using Microsoft.Exchange.WebServices.Data;
using NLog;

namespace OpenCVR.Update.Email
{
    class EmailService : IEmailService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly WebCredentials exhangeServiceCredentials;
        private readonly string exchangeEmailAddress;
        private readonly string host;

        public EmailService(WebCredentials exhangeServiceCredentials, string exchangeEmailAddress, string host = null)
        {
            this.exhangeServiceCredentials = exhangeServiceCredentials;
            this.exchangeEmailAddress = exchangeEmailAddress;
            this.host = host;
        }

        public CvrEmail GetEarliestEmailNotProccessed(DateTime lastItemReceivedTime)
        {
            logger.Info("Looking for new email received from cvr.dk");
            var service = CreateService();
            return SearchAndFindEmailIfAny(lastItemReceivedTime, service);
        }
        
        private ExchangeService CreateService()
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2007_SP1, TimeZoneInfo.Utc)
            {
                //TraceEnabled = true,
                //TraceFlags = TraceFlags.All,
                Credentials = exhangeServiceCredentials,
                UseDefaultCredentials = false,
                DateTimePrecision = DateTimePrecision.Milliseconds
            };
            if (host != null)
                service.Url = new Uri(host);
            else
                service.AutodiscoverUrl(exchangeEmailAddress, RedirectionUrlValidationCallback);
            return service;
        }

        private static CvrEmail SearchAndFindEmailIfAny(DateTime lastItemReceivedTime, ExchangeService service)
        {
            ItemView view = new ItemView(10);
            view.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Ascending);
            FindItemsResults<Item> findResults = service.FindItems(WellKnownFolderName.Inbox, GenerateSearchFilters(lastItemReceivedTime), view);
            logger.Info("Found " + findResults.Items.Count + " emails");
            return ToCvrEmail(findResults);
        }

        private static CvrEmail ToCvrEmail(FindItemsResults<Item> findResults)
        {
            if (findResults.Items.Count == 0)
                return null;
            var item = findResults.Items[0];
            item.Load(new PropertySet(ItemSchema.Id, ItemSchema.DateTimeReceived, ItemSchema.Body));
            return new CvrEmail(item.Id.UniqueId, item.DateTimeReceived, item.Body.Text);
        }

        private static SearchFilter GenerateSearchFilters(DateTime lastItemReceivedTime)
        {
            lastItemReceivedTime = FixBugWhereServerDoesNotCommunicateExactTime(lastItemReceivedTime);
            List<SearchFilter> filters = new List<SearchFilter>
            {
                new SearchFilter.IsEqualTo(ItemSchema.Subject, "Udtræk fra cvr.dk er klar til download"),
                new SearchFilter.IsGreaterThan(ItemSchema.DateTimeReceived, lastItemReceivedTime)
            };
            return new SearchFilter.SearchFilterCollection(LogicalOperator.And, filters);
        }

        private static DateTime FixBugWhereServerDoesNotCommunicateExactTime(DateTime dateTime)
        {
            // The EWS server does not return the exact email ReceiveTime. It only goes down to millseconds,
            // but the server seems to store it with microseconds. Thus, searching for an item that is received
            // later than the last received email, may actually return the same email.
            // Fix by adding a millisecond.
            return dateTime.AddMilliseconds(1);
        }

        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            return new Uri(redirectionUrl).Scheme == "https";
        }
    }
}