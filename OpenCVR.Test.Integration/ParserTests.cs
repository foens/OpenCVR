﻿using System;
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
                parser.Parse(@"..\..\test-data.zip");
            }
            catch (CsvTypeConverterException e)
            {
                throw new Exception("CSV parse exception. Details:" + e.Data["CsvHelper"], e);
            }
        }
    }
}
