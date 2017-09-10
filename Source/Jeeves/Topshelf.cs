using System;
using System.IO;
using Jeeves.Core;
using Topshelf;

namespace Jeeves
{
    internal class Topshelf
    {
        public static void Start(AppSettings settings)
        {
            HostFactory.Run(hc =>
            {
                hc.Service<Service>(sc =>
                {
                    sc.ConstructUsing(() => new Service(
                        settings.Database,
                        new Uri(settings.BaseUrl),
                        new JeevesSettings(settings.UseHttps, settings.UseAuthentication),
                        settings.SqlScriptsDirectory));
                    sc.WhenStarted(s => s.Start());
                    sc.WhenStopped(s =>
                    {
                        s.Stop();
                        s.Dispose();
                    });
                });

                hc.SetDescription("A simple REST service which provides configuration data for applications");
                hc.SetServiceName("Jeeves");
            });
        }
    }
}
