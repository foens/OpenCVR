using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using NLog;
using OpenCVR.Update.Parse.Mapping;

namespace OpenCVR.Update.Parse
{
    public class CompanyParser
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public virtual IEnumerable<DeltaCompany> Parse(StreamReader s)
        {
            DateTime start = DateTime.Now;
            logger.Info("Reading DeltaCompanies from stream");
            var csv = new CsvReader(s);
            csv.Configuration.RegisterClassMap(new DeltaCompanyMap());
            csv.Configuration.TrimFields = true;
            var deltaCompanies = csv.GetRecords<DeltaCompany>().ToList();
            logger.Info("Done reading DeltaCompanies. There were {0:n0} companies", deltaCompanies.Count);
            DateTime end = DateTime.Now;
            logger.Info("Took {0} ms", (end-start).TotalMilliseconds);
            return deltaCompanies;
        }
    }
}
