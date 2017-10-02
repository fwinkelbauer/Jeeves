using System;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace Jeeves.Core
{
    internal class JeevesBootstrapper : DefaultNancyBootstrapper
    {
        private readonly ModuleSettings _settings;
        private readonly IDataStore _store;
        private readonly IUserAuthenticator _authenticator;
        private readonly IJeevesLog _log;

        public JeevesBootstrapper(Uri baseUrl, IDataStore store, IUserAuthenticator authenticator, IJeevesLog log)
        {
            _settings = new ModuleSettings(baseUrl.Scheme == Uri.UriSchemeHttps);
            _store = store;
            _authenticator = authenticator;
            _log = log;

            if (_authenticator != null && !_settings.UseHttps)
            {
                throw new InvalidOperationException("Authentication can only be used with HTTPS");
            }
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            container.ThrowIfNull(nameof(container)).Register(_settings);
            container.Register(_store);
            container.Register(_authenticator);
            container.Register(_log);

            RegisterModules(container, new[] { new ModuleRegistration(typeof(JeevesModule)) });

            pipelines.ThrowIfNull(nameof(pipelines)).OnError += (ctx, ex) =>
            {
                _log.Error(ex, "An error occured while processing a request");
                return null;
            };
        }
    }
}
