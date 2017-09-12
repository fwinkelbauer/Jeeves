using Jeeves.Core;
using Topshelf;

namespace Jeeves
{
    internal class Topshelf
    {
        public static void Start(string database, string sqlScriptsDirectory, JeevesSettings settings)
        {
            HostFactory.Run(hc =>
            {
                hc.Service<Service>(sc =>
                {
                    sc.ConstructUsing(() => new Service(
                        database,
                        settings,
                        sqlScriptsDirectory));
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
