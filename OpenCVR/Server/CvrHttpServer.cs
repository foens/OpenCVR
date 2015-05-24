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
            isListening = false;
            httpListener.Close();
            serverThread.Join();
        }
    }
}