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
            ConfigureAuthentication();
            ConfigureApi();
        }

        private void ConfigureSecurity()
        {
            if (_settings.UseHttps)
            {
                this.RequiresHttps();
            }
        }

        private void ConfigureAuthentication()
        {
            if (!_settings.UseAuthentication)
            {
                return;
            }

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
        }

        private void ConfigureApi()
        {
            Get["/get/{user}/{application}/{key}"] = parameters =>
            {
                if (_settings.UseAuthentication)
                {
                    this.RequiresClaims($"user: {parameters.user}", $"app: {parameters.application}");
                    this.RequiresAnyClaim("access: read", "access: read/write");
                }

                var value = _store.RetrieveValue(
                    parameters.application,
                    parameters.user,
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
