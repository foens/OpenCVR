using System.Globalization;
using CsvHelper.TypeConversion;

namespace OpenCVR.Update.Parse.Mapping.Convert
{
    class CvrDateConverter : DateTimeConverter
    {
        private static readonly CultureInfo CultureInfo = new CultureInfo("da-DK");

        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            options.Format = "dd-MM-yyyy";
            options.CultureInfo = CultureInfo;
            return base.ConvertFromString(options, text);
        }
    }
}
