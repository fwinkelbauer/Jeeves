using System;
using Nancy;
using Nancy.Authentication.Stateless;
using Nancy.Security;

namespace Jeeves.Core
{
    internal class JeevesModule : NancyModule
    {
        private readonly JeevesSettings _settings;
        private readonly IDataStore _store;

        public JeevesModule(JeevesSettings settings, IDataStore store)
        {
            _settings = settings;
            _store = store;

            ConfigureSecurity();
            ConfigureApi();
        }

        private void ConfigureSecurity()
        {
            var configuration = new StatelessAuthenticationConfiguration(ctx =>
            {
                if (!ctx.Request.Query.apikey.HasValue)
                {
                    return null;
                }

                return _store.RetrieveUser(ctx.Request.Query.apikey).ToUserIdentity();
            });

            StatelessAuthentication.Enable(this, configuration);
            this.RequiresAuthentication();

            if (_settings.UseHttps)
            {
                this.RequiresHttps();
            }
        }

        private void ConfigureApi()
        {
            Get["/get/{application}/{key}"] = parameters =>
            {
                this.RequiresAnyClaim("Admin", $"({parameters.application})");

                var value = _store.RetrieveValue(
                    parameters.application,
                    Context.CurrentUser.UserName,
                    parameters.key);

                if (string.IsNullOrEmpty(value))
                {
                    return HttpStatusCode.NoContent;
                }

                return (Response)value;
            };
        }
    }
}
