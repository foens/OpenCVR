using CsvHelper.TypeConversion;

namespace OpenCVR.Update.Parse.Mapping.Convert
{
    class NullStringConverter : StringConverter
    {
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            return string.IsNullOrEmpty(text) ? null : base.ConvertFromString(options, text);
        }
    }
}