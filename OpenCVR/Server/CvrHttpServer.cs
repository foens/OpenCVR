using System;
using System.IO;
using System.Net;
using System.Threading;
using NLog;
using OpenCVR.Model;
using OpenCVR.Persistence;

namespace OpenCVR.Server
{
    class CvrHttpServer : ICvrHttpServer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private ICvrPersistence persistence;
        private readonly HttpListener httpListener;
        private volatile bool isListening;
        private Thread serverThread;


        public CvrHttpServer(ICvrPersistence persistence)
        {
            this.persistence = persistence;
            httpListener = new HttpListener();
            SetupHttpListener();
        }

        void SetupHttpListener()
        {
            httpListener.Prefixes.Add("http://localhost:8134/");
        }

        public void Start()
        {
            isListening = true;
            httpListener.Start();
            serverThread = new Thread(Listen);
            serverThread.Start();
        }

        private void Listen()
        {
            while (isListening)
            {
                var beginGetContext = httpListener.BeginGetContext(ListenerCallback, httpListener);
                beginGetContext.AsyncWaitHandle.WaitOne();
            }
        }

        private void ListenerCallback(IAsyncResult result)
        {
            if (!isListening)
                return;
            
            var context = httpListener.EndGetContext(result);
            ThreadPool.QueueUserWorkItem(_ =>
            {
                HandleRequest(context);
            });
        }

        private void HandleRequest(HttpListenerContext context)
        {
            Logger.Info("Request received for: {0}", context.Request.Url);
            var streamWriter = new StreamWriter(context.Response.OutputStream);
            try
            {
                switch (context.Request.Url.LocalPath)
                {
                    case "/":
                        streamWriter.Write("Hello world");
                        break;
                    case "/api/1/search":
                        string search = context.Request.QueryString["q"];
                        var company = persistence.Search(search);
                        streamWriter.Write($"{{VatNumber = {company.VatNumber}}}");
                        break;
                    default:
                        context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Error("An exception was thrown when processing url: {0}. Exception: {1}", context.Request.Url, e);
            }
            finally
            {
                streamWriter.Close();
                context.Response.Close();
            }
        }

        public void Stop()
        {
            isListening = false;
            httpListener.Close();
            serverThread.Join();
        }
    }
}