using Nancy;
using Nancy.Authentication.Stateless;
using Nancy.Security;

namespace Jeeves.Core
{
    internal class JeevesModule : NancyModule
    {
        private readonly IDataStore _store;

        public JeevesModule(JeevesSettings settings, IDataStore store)
        {
            _store = store;

            if (settings.UseHttps)
            {
                this.RequiresHttps();
            }

            ConfigureAuthentication();
            ConfigureApi();
        }

        private void ConfigureAuthentication()
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
