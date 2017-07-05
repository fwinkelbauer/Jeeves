using System;
using Jeeves.Common;
using Newtonsoft.Json;
using RestSharp;

namespace Jeeves.Client
{
    public class JeevesConfigurationProvider
    {
        private readonly Uri _baseUrl;
        private readonly string _application;
        private readonly string _apikey;

        public JeevesConfigurationProvider(Uri baseUrl, string application, string apikey)
        {
            _baseUrl = baseUrl.ThrowIfNull(nameof(baseUrl));
            _application = application.ThrowIfNull(nameof(application));
            _apikey = apikey.ThrowIfNull(nameof(apikey));
        }

        public JeevesConfiguration<T> RetrieveConfiguration<T>(string key)
        {
            key.ThrowIfNull(nameof(key));

            return JsonConvert.DeserializeObject<JeevesConfiguration<T>>(Retrieve("json", key));
        }

        public int RetrieveRevision(string key)
        {
            key.ThrowIfNull(nameof(key));

            return Convert.ToInt32(Retrieve("revision", key));
        }

        private string Retrieve(string resource, string key)
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest($"/get/{resource}/{_application}/{key}", Method.GET);
            request.AddParameter("apikey", _apikey);

            var response = client.Execute(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new InvalidOperationException("Could not retrieve data from server", response.ErrorException);
            }

            return response.Content;
        }
    }
}
