using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace Jeeves.Core
{
    internal class JeevesBootstrapper : DefaultNancyBootstrapper
    {
        private readonly JeevesSettings _settings;
        private readonly IDataStore _store;
        private readonly IJeevesLog _log;

        public JeevesBootstrapper(JeevesSettings settings, IDataStore store, IJeevesLog log)
        {
            _settings = settings;
            _store = store;
            _log = log;
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            container.ThrowIfNull(nameof(container)).Register(_store);
            container.Register(_settings);
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
