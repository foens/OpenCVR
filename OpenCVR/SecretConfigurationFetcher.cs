using System;
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

        private static string FindValue(string key)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "CvrSecrets.xml");
            var doc = new XmlDocument();
            doc.Load(path);
            var selectSingleNode = doc.SelectSingleNode("/CvrConfiguration/@" + key);
            if (selectSingleNode != null)
                return selectSingleNode.Value;
            throw new Exception("Could not find configuration for " + key + " in file: " + path);
        }
    }
}