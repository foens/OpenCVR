using System;
using System.IO;
using System.IO.Compression;
using NLog;

namespace OpenCVR.Update.Parse
{
    public interface ICvrParser
    {
        CvrEntries Parse(string zipFile);
    }

    public class CvrParser : ICvrParser
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly CompanyParser companyParser;

        public CvrParser(CompanyParser companyParser)
        {
            this.companyParser = companyParser;
        }

        public CvrEntries Parse(string zipFile)
        {
            return Parse(new ZipArchive(File.OpenRead(zipFile), ZipArchiveMode.Read));
        }

        internal CvrEntries Parse(ZipArchive z)
        {
            var companyEntry = FindCompanyEntry(z);
            using (var companyReader = new StreamReader(companyEntry.Open()))
            {
                logger.Info("Parse companies with zip entry of size {0}", SizeSuffix(companyEntry.Length));
                return new CvrEntries
                {
                    Companies = companyParser.Parse(companyReader)
                };
            }
        }

        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        static string SizeSuffix(long value)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }

        private ZipArchiveEntry FindCompanyEntry(ZipArchive z)
        {
            foreach (var entry in z.Entries)
            {
                if (entry.Name.Contains("VIRKSOMHEDER"))
                    return entry;
            }
            throw new Exception("Could not find company entry: " + z.Entries);
        }
    }
}
