﻿using System.IO;
using OpenCVR;
using OpenCVR.Persistence;
using OpenCVR.Server;
using OpenCVR.Update;
using OpenCVR.Update.Email;
using OpenCVR.Update.Http;
using OpenCVR.Update.Parse;
using Microsoft.Exchange.WebServices.Data;
using Mono.Data.Sqlite;
using Ninject.Modules;

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
            Bind<string>().ToConstant(Path.GetTempPath()).WhenInjectedInto<HttpService>();
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
            Bind<SqliteConnection>().ToConstructor(c => new SqliteConnection(c.Inject<string>()));
            Bind<string>().ToConstant("Data Source=cvr.sqlite;Version=3;").WhenInjectedInto<SqliteConnection>();
            Bind<ICvrPersistence>().To<CvrPersistence>();
        }
    }
}
