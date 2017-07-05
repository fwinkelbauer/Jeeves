using Jeeves.Common;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace Jeeves.Core
{
    internal class JeevesBootstrapper : DefaultNancyBootstrapper
    {
        private readonly JeevesSettings _settings;
        private readonly IDataStore _store;

        public JeevesBootstrapper(JeevesSettings settings, IDataStore store)
        {
            _settings = settings.ThrowIfNull(nameof(settings));
            _store = store.ThrowIfNull(nameof(store));
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            container.ThrowIfNull(nameof(container)).Register(_store);
            container.Register(_settings);

            RegisterModules(container, new[] { new ModuleRegistration(typeof(JeevesModule)) });
        }
    }
}
