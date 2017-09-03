﻿using System;
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
                    _log.DebugFormat("Authentication failed: no API key provided");
                    return null;
                }

                JeevesUser user = null;

                try
                {
                    user = _store.RetrieveUser(ctx.Request.Query.apikey);
                }
                catch (Exception e)
                {
                    _log.ErrorFormat(e, "Authentication failed");
                    throw;
                }

                if (user == null)
                {
                    _log.DebugFormat("Authentication failed: user not found (null)");
                    return null;
                }

                _log.DebugFormat("Authenticated user: {user}", user.UserName);
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

                if (_settings.UseAuthentication)
                {
                    this.RequiresClaims($"user: {user}", $"app: {application}");
                    this.RequiresAnyClaim("access: read", "access: read/write");
                }

                try
                {
                    string value = _store.RetrieveValue(user, application, key);

                    if (string.IsNullOrEmpty(value))
                    {
                        _log.DebugFormat("No data found for request /get/{user}/{application}/{key}", user, application, key);

                        return HttpStatusCode.NoContent;
                    }

                    _log.DebugFormat("Retriving value for request /get/{user}/{application}/{key}", user, application, key);

                    return (Response)value;
                }
                catch (Exception e)
                {
                    _log.ErrorFormat(e, "Error while processing request /get/{user}/{application}/{key}", user, application, key);

                    return HttpStatusCode.NoContent;
                }
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
                    _log.DebugFormat("No value provided for request /post/{user}/{application}/{key}", user, application, key);

                    return HttpStatusCode.BadRequest;
                }

                try
                {
                    _store.PutValue(user, application, key, request.Value);

                    _log.DebugFormat("Stored value for request /post/{user}/{application}/{key}", user, application, key);

                    return HttpStatusCode.OK;
                }
                catch (Exception e)
                {
                    _log.ErrorFormat(e, "Error while processing request /post/{user}/{application}/{key}", user, application, key);

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
