using System;
using Nancy.Hosting.Self;

namespace Jeeves.Core
{
    public sealed class JeevesHost : IDisposable
    {
        private readonly JeevesBootstrapper _bootstrapper;
        private readonly NancyHost _host;

        public JeevesHost(Uri baseUrl, JeevesSettings settings, IDataStore store, IJeevesLog log)
        {
            var config = new HostConfiguration()
            {
                UrlReservations = new UrlReservations() { CreateAutomatically = true }
            };

            _bootstrapper = new JeevesBootstrapper(
                settings.ThrowIfNull(nameof(settings)),
                store.ThrowIfNull(nameof(store)),
                log.ThrowIfNull(nameof(log)));

            _host = new NancyHost(_bootstrapper, config, baseUrl.ThrowIfNull(nameof(baseUrl)));
        }

        public void Dispose()
        {
            // TODO fw should these statements be swapped?
            _bootstrapper.Dispose();
            _host.Dispose();
        }

        public void Start()
        {
            _host.Start();
        }

        public void Stop()
        {
            _host.Stop();
        }
    }
}
