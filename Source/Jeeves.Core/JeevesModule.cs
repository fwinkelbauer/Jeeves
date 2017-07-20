using Jeeves.Core.Logging;
using Nancy;
using Nancy.Authentication.Stateless;
using Nancy.ModelBinding;
using Nancy.Security;

namespace Jeeves.Core
{
    internal class JeevesModule : NancyModule
    {
        private static readonly ILog Log = LogProvider.For<JeevesModule>();

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
                    Log.Debug("Authentication failed: no API key provided");
                    return null;
                }

                IUserIdentity user = _store.RetrieveUser(ctx.Request.Query.apikey).ToUserIdentity();

                Log.DebugFormat("Authenticated user: {user}", user.UserName);

                return user;
            });

            StatelessAuthentication.Enable(this, configuration);
            this.RequiresAuthentication();
        }

        private void ConfigureApi()
        {
            Get["/get/{user}/{application}/{key}"] = parameters =>
            {
                string user = parameters.user;
                string application = parameters.application;
                string key = parameters.key;

                if (_settings.UseAuthentication)
                {
                    this.RequiresClaims($"user: {user}", $"app: {application}");
                    this.RequiresAnyClaim("access: read", "access: read/write");
                }

                var value = _store.RetrieveValue(user, application, key);

                if (string.IsNullOrEmpty(value))
                {
                    Log.DebugFormat(
                        "No data found for request /get/{user}/{application}/{key}",
                        user,
                        application,
                        key);

                    return HttpStatusCode.NoContent;
                }

                Log.DebugFormat(
                        "Returning value for request /get/{user}/{application}/{key}",
                        user,
                        application,
                        key);

                return (Response)value;
            };

            Post["/post/{user}/{application}/{key}"] = parameters =>
            {
                string user = parameters.user;
                string application = parameters.application;
                string key = parameters.key;

                if (_settings.UseAuthentication)
                {
                    this.RequiresClaims($"user: {user}", $"app: {application}", "access: read/write");
                }

                var request = this.Bind<PostKeyRequest>();

                if (string.IsNullOrEmpty(request.Value))
                {
                    Log.DebugFormat(
                        "No value provided for /post/{user}/{application}/{key}",
                        user,
                        application,
                        key);

                    return HttpStatusCode.BadRequest;
                }

                _store.PutValue(user, application, key, request.Value);

                Log.DebugFormat(
                        "Stored value for /post/{user}/{application}/{key}",
                        user,
                        application,
                        key);

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
