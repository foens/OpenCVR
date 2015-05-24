using System;
using System.Threading;
using OpenCVR;
using OpenCVR.Persistence;
using OpenCVR.Server;
using OpenCVR.Update;
using Ninject;

namespace OpenCVR
{
    public class OpenCvrRunner
    {
        private ICvrHttpServer httpServer;
        private ICvrUpdater updater;
        private Thread cvrUpdateThread;
        private IKernel kernel;

        public void Start()
        {
            kernel = new StandardKernel(new OpenCvrModule());
            kernel.Get<CvrPersistence>().UpgradeSchemaIfRequired();
            httpServer = kernel.Get<ICvrHttpServer>();
            updater = kernel.Get<ICvrUpdater>();

            httpServer.Start();
            cvrUpdateThread = new Thread(UpdateCvrLoop);
            cvrUpdateThread.Start();
        }

        public void Stop()
        {
            httpServer.Stop();
            cvrUpdateThread.Interrupt();
            cvrUpdateThread.Join();

            httpServer = null;
            updater = null;
            cvrUpdateThread = null;
            kernel.Dispose();
        }

        private void UpdateCvrLoop()
        {
            try
            {
                while (true)
                {
                    updater.TryUpdate();
                    Thread.Sleep(1000*60*60);
                }
            }
            catch (ThreadInterruptedException)
            {
                // Stop running
            }
        }

        public static void Main()
        {
            var runner = new OpenCvrRunner();
            runner.Start();
            Console.Read();
            runner.Stop();
        }
    }
}
