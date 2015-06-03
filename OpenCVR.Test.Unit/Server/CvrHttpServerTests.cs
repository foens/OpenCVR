using System;
using System.IO;
using System.Net;
using Moq;
using NUnit.Framework;
using OpenCVR.Model;
using OpenCVR.Persistence;
using OpenCVR.Server;

namespace OpenCVR.Test.Unit.Server
{
    [TestFixture]
    public class CvrHttpServerTests
    {
        private Mock<ICvrPersistence> persistence;
        private CvrHttpServer server;

        [SetUp]
        public void Setup()
        {
            persistence = new Mock<ICvrPersistence>();
            server = new CvrHttpServer(persistence.Object, "http://localhost:8134/");
            server.Start();
        }

        [Test]
        public void TestCanExecuteSearch()
        {
            Company c = new Company
            {
                VatNumber = 1234,
                Name = "Foobar"
            };
            persistence.Setup(e => e.Search("1234")).Returns(c);
            var response = GetResponseAndStopServer("api/v1/search/1234");
            StringAssert.Contains("\"VatNumber\":1234", response.Body);
            StringAssert.Contains("\"Name\":\"Foobar\"", response.Body);
        }

        [Test]
        public void TestCanExecuteSearchWithName()
        {
            var c = new Company
            {
                VatNumber = 51234,
                Name = "Test"
            };
            persistence.Setup(e => e.Search("51")).Returns(c);
            var response = GetResponseAndStopServer("api/v1/search/51");
            StringAssert.Contains("{\"VatNumber\":51234", response.Body);
        }

        [Test]
        public void TestNonExistentResourceReturns404()
        {
            var response = GetResponseAndStopServer("api/v1/lasmdf4g");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        private Response GetResponseAndStopServer(string path)
        {
            try
            {
                var url = "http://localhost:8134/" + path;
                var request = (HttpWebRequest) WebRequest.Create(url);
                request.Timeout = 1000;
                using (var response = request.GetResponse())
                {
                    return ReadResponse(response);
                }
            }
            catch (WebException e)
            {
                return ReadResponse(e.Response);
            }
            finally
            {
                server.Stop();
            }
        }

        private static Response ReadResponse(WebResponse response)
        {
            var r = (HttpWebResponse) response;
            using (var responseStream = r.GetResponseStream())
            {
                if (responseStream == null)
                    throw new ArgumentNullException();
                using (var reader = new StreamReader(responseStream))
                {
                    return new Response
                    {
                        Body = reader.ReadToEnd(),
                        StatusCode = r.StatusCode
                    };
                }
            }
        }
    }

    internal class Response
    {
        public string Body { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
