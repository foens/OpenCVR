using System;
using System.Configuration;
using System.IO;
using System.Xml;

namespace OpenCVR
{
    static internal class SecretConfigurationFetcher
    {
        public static string ReadEchangeServicePassword()
        {
            return FindValue("ExchangeServicePassword");
        }

        public static string ReadCvrPassword()
        {
            return FindValue("CvrPassword");
        }

        public static string ReadExchangeHost()
        {
            return FindValue("ExchangeServiceHost");
        }

        public static string ReadExchangeServiceUserName()
        {
            return FindValue("ExchangeServiceUserName");
        }

        public static string ReadEmailAddress()
        {
            return FindValue("EmailAddress");
        }

        private static string FindValue(string key)
        {
            var path = Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["secretsLocation"]);
            var doc = new XmlDocument();
            doc.Load(path);
            var selectSingleNode = doc.SelectSingleNode("/CvrConfiguration/@" + key);
            if (selectSingleNode != null)
                return selectSingleNode.Value;
            throw new Exception("Could not find configuration for " + key + " in file: " + path);
        }
    }
}