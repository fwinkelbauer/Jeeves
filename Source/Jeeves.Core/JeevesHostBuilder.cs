using System;

namespace Jeeves.Core
{
    public class JeevesHostBuilder
    {
        private readonly Uri _baseUrl;
        private readonly IDataStore _store;

        private IUserAuthenticator _authenticator;
        private IJeevesLog _log;

        public JeevesHostBuilder(Uri baseUrl, IDataStore store)
        {
            _baseUrl = baseUrl.ThrowIfNull(nameof(baseUrl));
            _store = store.ThrowIfNull(nameof(store));
            _authenticator = null;
            _log = new NullLog();
        }

        public JeevesHostBuilder LogTo(IJeevesLog log)
        {
            _log = log.ThrowIfNull(nameof(log));

            return this;
        }

        public JeevesHostBuilder WithUserAuthentication(IUserAuthenticator authenticator)
        {
            _authenticator = authenticator.ThrowIfNull(nameof(authenticator));

            return this;
        }

        public JeevesHost Build()
        {
            return new JeevesHost(_baseUrl, _store, _authenticator, _log);
        }

        private class NullLog : IJeevesLog
        {
            public void Information(string messageTemplate, params object[] values)
            {
                // do nothing
            }

            public void Error(Exception ex, string messageTemplate, params object[] values)
            {
                // do nothing
            }
        }
    }
}
