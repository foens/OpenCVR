using System;
using System.Globalization;

namespace OpenCVR.Update.Parse.Mapping
{
    static internal class ConvertUtil
    {
        private static readonly CultureInfo CultureInfo = new CultureInfo("da-DK");

        public static DateTime? ConvertDate(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            return DateTime.ParseExact(value, "dd-MM-yyyy", CultureInfo);
        }
    }
}