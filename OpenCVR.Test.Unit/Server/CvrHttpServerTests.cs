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
            server = new CvrHttpServer(persistence.Object);
            server.Start();
        }

        [Test]
        public void TestCanExecuteHelloWorld()
        {
            Assert.AreEqual("Hello world", GetResponseAndStopServer("").Body);
        }

        [Test]
        public void TestCanExecuteSearch()
        {
            Company c = new Company
            {
                VatNumber = 1234
            };
            persistence.Setup(e => e.Search("1234")).Returns(c);
            var response = GetResponseAndStopServer("api/1/search?q=1234");
            Assert.AreEqual("{VatNumber = 1234}", response.Body);
        }

        [Test]
        public void TestCanExecuteSearchWithName()
        {
            var c = new Company
            {
                VatNumber = 51234
            };
            persistence.Setup(e => e.Search("51")).Returns(c);
            var response = GetResponseAndStopServer("api/1/search?q=51");
            Assert.AreEqual("{VatNumber = 51234}", response.Body);
        }

        [Test]
        public void TestNonExistentResourceReturns404()
        {
            var response = GetResponseAndStopServer("api/1/lasmdf4g");
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
