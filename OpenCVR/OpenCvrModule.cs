using System.IO;
using OpenCVR.Persistence;
using OpenCVR.Server;
using OpenCVR.Update;
using OpenCVR.Update.Email;
using OpenCVR.Update.Http;
using OpenCVR.Update.Parse;
using Microsoft.Exchange.WebServices.Data;
using Ninject.Modules;
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
            Bind<ICvrEmailExtractor>().To<CvrEmailExtractor>();
            Bind<ICvrParser>().To<CvrParser>();

            BindPersistence();

            Bind<IEmailService>().To<EmailService>().WithConstructorArgument("host", "https://mail.cgm.com/EWS/Exchange.asmx");

            Bind<IHttpService>().To<HttpService>();
            Bind<string>().ToConstant(Path.Combine(Path.GetTempPath(), "OpenCVR", "DownloadCache")).WhenInjectedInto<HttpService>();
            BindEmailService();
        }

        private void BindEmailService()
        {
            Bind<WebCredentials>()
                .ToConstructor(c => new WebCredentials("kasper.foens", SecretConfigurationFetcher.ReadEchangeServicePassword()))
                .WhenInjectedInto<EmailService>();
            Bind<string>()
                .ToConstant("kasper.foens@cgm.com")
                .WhenInjectedInto<EmailService>();
        }

        private void BindUpdater()
        {
            Bind<ICvrUpdater>().To<CvrUpdater>();
            Bind<string>().ToConstant(SecretConfigurationFetcher.ReadCvrPassword()).WhenInjectedInto<CvrUpdater>();
        }

        private void BindPersistence()
        {
            Bind<SQLiteConnection>().ToConstructor(c => new SQLiteConnection(c.Inject<string>()));
            Bind<string>().ToConstant("Data Source=cvr.sqlite;Version=3;").WhenInjectedInto<SQLiteConnection>();
            Bind<ICvrPersistence>().To<CvrPersistence>();
        }
    }
}
