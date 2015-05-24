using System;
using System.Text.RegularExpressions;

namespace OpenCVR.Update.Email
{
    class CvrEmailExtractor : ICvrEmailExtractor
    {
        private const string Regex = @"https://data\.cvr\.dk/(?<userId>\d+)_\d+_\d+.zip";

        public Uri ParseOutZipfileUrl(string emailText)
        {
            return new Uri(RunRegex(emailText).Value);
        }

        public string ParseOutLoginId(string emailText)
        {
            return RunRegex(emailText).Groups["userId"].Value;
        }

        private static Match RunRegex(string t)
        {
            var r = new Regex(Regex);
            var mc = r.Matches(t);
            if (mc.Count == 0)
                throw new Exception("Could not find match in " + t);
            return mc[0];
        }
    }
}