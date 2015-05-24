using System;
using CsvHelper.TypeConversion;
using OpenCVR.Model;

namespace OpenCVR.Update.Parse.Mapping.Convert
{
    class ModificationStatusConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            switch (text)
            {
                case "ny":
                    return ModificationStatus.New;
                case "fjernet":
                    return ModificationStatus.Removed;
                case "modificeret":
                    return ModificationStatus.Modified;
            }
            throw new Exception("Unknown ModificationStatus: " + text);
        }

        public override bool CanConvertFrom(Type type)
        {
            return type == typeof (string);
        }
    }
}
