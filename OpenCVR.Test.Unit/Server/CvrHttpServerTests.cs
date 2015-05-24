using System.IO;
using System.Net;
using Moq;
using NUnit.Framework;
using OpenCVR.Persistence;
using OpenCVR.Server;

namespace OpenCVR.Test.Unit.Server
{
    [TestFixture]
    public class CvrHttpServerTests
    {
        [Test]
        public void TestCanExecuteHelloWorld()
        {
            var persistence = new Mock<ICvrPersistence>();
            var server = new CvrHttpServer(persistence.Object);
            server.Start();
            try
            {
                string url = "http://localhost:8134/";
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string text = reader.ReadToEnd();
                Assert.AreEqual("Hello world", text);
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
