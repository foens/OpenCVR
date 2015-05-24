using System;
using System.IO;
using CsvHelper.TypeConversion;
using NUnit.Framework;
using OpenCVR.Update.Parse;

namespace OpenCVR.Test.Integration
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void CanParseEverything()
        {
            try
            {
                var parser = new CvrParser(new CompanyParser());
                parser.Parse(Path.Combine("..", "..", @"test-data.zip"));
            }
            catch (Exception e)
            {
                if(e.Data.Contains("CsvHelper"))
                    throw new Exception("CSV parse exception. Details:" + e.Data["CsvHelper"], e);
                throw;
            }
        }
    }
}
