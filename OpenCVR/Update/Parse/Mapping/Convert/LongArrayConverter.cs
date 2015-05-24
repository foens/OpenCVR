using System;
using CsvHelper.TypeConversion;

namespace OpenCVR.Update.Parse.Mapping.Convert
{
    class LongArrayConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            if (string.IsNullOrEmpty(text))
                return new long[0];
            var presumeablyLongs = text.Split(',');
            var r = new long[presumeablyLongs.Length];
            for (int i = 0; i < presumeablyLongs.Length; i++)
                r[i] = long.Parse(presumeablyLongs[i]);
            return r;
        }

        public override bool CanConvertFrom(Type type)
        {
            return type == typeof(string);
        }
    }
}
