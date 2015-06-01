using System;
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;
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
            DateTime start = DateTime.Now;
            Logger.Info("Request received for: {0}", context.Request.Url);
            var streamWriter = new StreamWriter(context.Response.OutputStream);
            try
            {
                var localPath = context.Request.Url.LocalPath;
                if (localPath.StartsWith("/api/v1/"))
                {
                    HandleApiCall(context, streamWriter);
                }
                else if (localPath.Equals("/"))
                {
                    streamWriter.Write("Hello world");
                }
                else
                {
                    HandleRequestNotFound(context, streamWriter);
                }
            } catch(Exception e)
            {
                Logger.Error("An exception was thrown when processing url: {0}. Exception: {1}", context.Request.Url, e);
            }
            finally
            {
                streamWriter.Close();
                context.Response.Close();
                DateTime end = DateTime.Now;
                Logger.Info("Request handled in {0}ms", (end-start).TotalMilliseconds);
            }
        }

        private void HandleApiCall(HttpListenerContext context, StreamWriter streamWriter)
        {
            var localPath = context.Request.Url.LocalPath;
            var apiV1Search = "/api/v1/search/";
            if (localPath.StartsWith(apiV1Search))
            {
                string search = localPath.Substring(apiV1Search.Length);
                var company = persistence.Search(search);
                var isoDateTimeConverter = new IsoDateTimeConverter();
                isoDateTimeConverter.DateTimeFormat = "dd-MM-yyyy";
                streamWriter.Write(JsonConvert.SerializeObject(company, isoDateTimeConverter));
            }
            else
            {
                HandleRequestNotFound(context, streamWriter);
            }
        }

        private void HandleRequestNotFound(HttpListenerContext context, StreamWriter streamWriter)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            streamWriter.Write("Error 404 - Not found");
        }

        public void Stop()
        {
            isListening = false;
            httpListener.Close();
            serverThread.Join();
        }
    }
}