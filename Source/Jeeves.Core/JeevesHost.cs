using System;
using Nancy.Hosting.Self;

namespace Jeeves.Core
{
    public sealed class JeevesHost : IDisposable
    {
        private readonly JeevesBootstrapper _bootstrapper;
        private readonly Uri _baseUrl;
        private readonly NancyHost _host;
        private readonly IJeevesLog _log;

        internal JeevesHost(Uri baseUrl, IDataStore store, IUserAuthenticator authenticator, IJeevesLog log)
        {
            _bootstrapper = new JeevesBootstrapper(baseUrl, store, authenticator, log);

            var config = new HostConfiguration()
            {
                UrlReservations = new UrlReservations() { CreateAutomatically = true }
            };

            _baseUrl = baseUrl;
            _host = new NancyHost(_bootstrapper, config, baseUrl);
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
    }
}
