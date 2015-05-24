using System;
using System.IO;
using System.Net;
using System.Threading;
using OpenCVR.Persistence;

namespace OpenCVR.Server
{
    class CvrHttpServer : ICvrHttpServer
    {
        private ICvrPersistence persistence;
        private readonly HttpListener httpListener;
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
            httpListener.Start();
            serverThread = new Thread(Listen);
            serverThread.Start();
        }

        private void Listen()
        {
            while (httpListener.IsListening)
            {
                var beginGetContext = httpListener.BeginGetContext(ListenerCallback, httpListener);
                beginGetContext.AsyncWaitHandle.WaitOne();
            }
        }

        private void ListenerCallback(IAsyncResult result)
        {
            if (!httpListener.IsListening)
                return;
            
            HttpListenerContext context = httpListener.EndGetContext(result);
            ThreadPool.QueueUserWorkItem(_ =>
            {
                var streamWriter = new StreamWriter(context.Response.OutputStream);
                streamWriter.Write("Hello world");
                streamWriter.Close();
                context.Response.Close();
            });
        }

        public void Stop()
        {
            httpListener.Close();
            serverThread.Join();
        }
    }
}