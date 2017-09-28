using System;
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
        private readonly IJeevesLog _log;

        public JeevesModule(JeevesSettings settings, IDataStore store, IJeevesLog log)
        {
            _settings = settings;
            _store = store;
            _log = log;

            ConfigureSecurity();
            ConfigureAuthentication();
            ConfigureApi();
        }

        private void ConfigureSecurity()
        {
            if (_settings.Security == SecurityOption.Https
                || _settings.Security == SecurityOption.HttpsAndAuthentication)
            {
                this.RequiresHttps();
            }
        }

        private void ConfigureAuthentication()
        {
            if (_settings.Security != SecurityOption.HttpsAndAuthentication)
            {
                return;
            }

            var configuration = new StatelessAuthenticationConfiguration(ctx =>
            {
                if (!ctx.Request.Query.apikey.HasValue)
                {
                    _log.Information("Authentication failed: no API key provided");
                    return null;
                }

                JeevesUser user = null;

                try
                {
                    user = _store.RetrieveUser(ctx.Request.Query.apikey);
                }
                catch (Exception e)
                {
                    _log.Error(e, "Authentication failed");
                    throw;
                }

                if (user == null)
                {
                    _log.Information("User does not exist");
                    return null;
                }

                _log.Information("Authenticated user: {user}", user.UserName);
                return user.ToUserIdentity();
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
                var route = $"/get/{user}/{application}/{key}";

                if (_settings.Security == SecurityOption.HttpsAndAuthentication)
                {
                    this.RequiresClaims($"user: {user}", $"app: {application}");
                }

                try
                {
                    string value = _store.RetrieveValue(user, application, key);

                    if (string.IsNullOrEmpty(value))
                    {
                        _log.Information("No data found for request {route}", route);

                        return HttpStatusCode.NoContent;
                    }

                    _log.Information("Retriving value for request {route}", route);

                    return value;
                }
                catch (Exception e)
                {
                    _log.Error(e, "Error while processing request route", route);

                    return HttpStatusCode.NoContent;
                }
            };

            Post["/post/{user}/{application}/{key}"] = parameters =>
            {
                string user = parameters.user;
                string application = parameters.application;
                string key = parameters.key;
                var route = $"/post/{user}/{application}/{key}";

                if (_settings.Security == SecurityOption.HttpsAndAuthentication)
                {
                    this.RequiresClaims($"user: {user}", $"app: {application}");
                }

                var request = this.Bind<PostKeyRequest>();

                if (request == null || string.IsNullOrEmpty(request.Value))
                {
                    _log.Information("No value provided for request {route}", route);

                    return HttpStatusCode.BadRequest;
                }

                try
                {
                    _store.StoreValue(user, application, key, request.Value);

                    _log.Information("Stored value for request {route}", route);

                    return HttpStatusCode.OK;
                }
                catch (Exception e)
                {
                    _log.Error(e, "Error while processing request {route}", route);

                    return HttpStatusCode.BadRequest;
                }
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
