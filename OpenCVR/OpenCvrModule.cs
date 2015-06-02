using System;
using System.IO;
using System.Configuration;
using OpenCVR.Persistence;
using OpenCVR.Server;
using OpenCVR.Update;
using OpenCVR.Update.Email;
using OpenCVR.Update.Http;
using OpenCVR.Update.Parse;
using Microsoft.Exchange.WebServices.Data;
using Ninject.Modules;
using ConfigurationManager = System.Configuration.ConfigurationManager;
#if (__MonoCS__)
using SQLiteConnection = Mono.Data.Sqlite.SqliteConnection;
#else
using System.Data.SQLite;
#endif

namespace OpenCVR
{
    class OpenCvrModule : NinjectModule
    {
        public override void Load()
        {
            BindUpdater();

            Bind<ICvrHttpServer>().To<CvrHttpServer>();
            Bind<string>().ToConstant(ConfigurationManager.AppSettings["staticFilesServePath"]).WhenInjectedInto<CvrHttpServer>();
            Bind<ICvrEmailExtractor>().To<CvrEmailExtractor>();
            Bind<ICvrParser>().To<CvrParser>();

            BindPersistence();
            
            Bind<IHttpService>().To<HttpService>();
            Bind<string>().ToConstant(Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["downloadTempPath"])).WhenInjectedInto<HttpService>();
            BindEmailService();
        }

        private void BindEmailService()
        {
            Bind<IEmailService>().To<EmailService>().WithConstructorArgument("host", SecretConfigurationFetcher.ReadExchangeHost());
            Bind<WebCredentials>()
                .ToConstructor(c => new WebCredentials(SecretConfigurationFetcher.ReadExchangeServiceUserName(), SecretConfigurationFetcher.ReadEchangeServicePassword()))
                .WhenInjectedInto<EmailService>();
            Bind<string>().ToConstant(SecretConfigurationFetcher.ReadEmailAddress()).WhenInjectedInto<EmailService>();
        }

        private void BindUpdater()
        {
            Bind<ICvrUpdater>().To<CvrUpdater>();
            Bind<string>().ToConstant(SecretConfigurationFetcher.ReadCvrPassword()).WhenInjectedInto<CvrUpdater>();
        }

        private void BindPersistence()
        {
            Bind<SQLiteConnection>().ToConstructor(c => new SQLiteConnection(c.Inject<string>()));
            Bind<string>().ToConstant("Data Source=" + ConfigurationManager.AppSettings["databaseFile"] + ";Version=3;").WhenInjectedInto<SQLiteConnection>();
            Bind<ICvrPersistence>().To<CvrPersistence>();
        }
    }
}
