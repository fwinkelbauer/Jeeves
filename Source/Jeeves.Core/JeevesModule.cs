using Nancy;
using Nancy.Authentication.Stateless;
using Nancy.ModelBinding;
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
                    parameters.user,
                    parameters.application,
                    parameters.key);

                if (string.IsNullOrEmpty(value))
                {
                    return HttpStatusCode.NoContent;
                }

                return (Response)value;
            };

            Post["/post/{user}/{application}/{key}"] = parameters =>
            {
                if (_settings.UseAuthentication)
                {
                    this.RequiresClaims($"user: {parameters.user}", $"app: {parameters.application}", "access: read/write");
                }

                var request = this.Bind<PostKeyRequest>();

                if (string.IsNullOrEmpty(request.Value))
                {
                    return HttpStatusCode.BadRequest;
                }

                _store.PutValue(
                    parameters.user,
                    parameters.application,
                    parameters.key,
                    request.Value);

                return HttpStatusCode.OK;
            };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Class is used in POST route (model binding)")]
        private class PostKeyRequest
        {
            public string Value { get; set; }
        }
    }
}
