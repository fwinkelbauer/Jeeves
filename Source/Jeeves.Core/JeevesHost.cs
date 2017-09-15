using System;
using Nancy.Hosting.Self;

namespace Jeeves.Core
{
    public sealed class JeevesHost : IDisposable
    {
        private readonly JeevesBootstrapper _bootstrapper;
        private readonly string _baseUrl;
        private readonly NancyHost _host;
        private readonly IJeevesLog _log;

        public JeevesHost(JeevesSettings settings, IDataStore store)
            : this(settings, store, new NullLog())
        {
        }

        public JeevesHost(JeevesSettings settings, IDataStore store, IJeevesLog log)
        {
            _bootstrapper = new JeevesBootstrapper(
                settings.ThrowIfNull(nameof(settings)),
                store.ThrowIfNull(nameof(store)),
                log.ThrowIfNull(nameof(log)));

            var config = new HostConfiguration()
            {
                UrlReservations = new UrlReservations() { CreateAutomatically = true }
            };

            _baseUrl = settings.BaseUrl;
            _host = new NancyHost(_bootstrapper, config, new Uri(settings.BaseUrl));
            _log = log;
        }

        public void Dispose()
        {
            // TODO fw should these statements be swapped?
            _bootstrapper.Dispose();
            _host.Dispose();
        }

        public void Start()
        {
            _log.Information("Starting Jeeves web service at {url}", _baseUrl);
            _host.Start();
        }

        public void Stop()
        {
            _log.Information("Stopping Jeeves web service at {url}", _baseUrl);
            _host.Stop();
        }

        private class NullLog : IJeevesLog
        {
            public void Information(string messageTemplate, params object[] values)
            {
                // do nothing
            }

            public void Error(Exception ex, string messageTemplate, params object[] values)
            {
                // do nothing
            }
        }
    }
}
