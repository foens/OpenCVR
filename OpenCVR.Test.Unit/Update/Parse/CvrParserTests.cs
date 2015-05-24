using System.IO;
using System.IO.Compression;
using Moq;
using NUnit.Framework;
using OpenCVR.Update.Parse;

namespace OpenCVR.Test.Unit.Update.Parse
{
    [TestFixture]
    public class CvrParserTests
    {
        [Test]
        public void TestCanParseCompanies()
        {
            var mock = new Mock<CompanyParser>();
            var parser = new CvrParser(mock.Object);
            var zipArchive = CreateZipArchive();
            mock.Setup(e => e.Parse(It.IsAny<StreamReader>())).Callback((StreamReader r) => VerifyStream(r));

            parser.Parse(zipArchive);

            mock.Verify(e => e.Parse(It.IsAny<StreamReader>())); // Would like to test
        }

        private void VerifyStream(StreamReader r)
        {
            if(!r.ReadToEnd().Equals("d"))
                throw new AssertionException("Expected stream to contain 'd'");
        }

        private ZipArchive CreateZipArchive()
        {
            var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                CreateFile(archive, "39213744_54613_20150513071406.properties", "a");
                CreateFile(archive, "39213744_54613_20150513071406_DELTAGERE.csv", "b");
                CreateFile(archive, "39213744_54613_20150513071406_PRODUKTIONSENHEDER.csv", "c");
                CreateFile(archive, "39213744_54613_20150513071406_VIRKSOMHEDER.csv", "d");
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new ZipArchive(memoryStream, ZipArchiveMode.Read);
        }

        private static void CreateFile(ZipArchive archive, string filename, string contents)
        {
            var file = archive.CreateEntry(filename);
            using (var entryStream = file.Open())
            using (var streamWriter = new StreamWriter(entryStream))
            {
                streamWriter.Write(contents);
            }
        }
    }
}
