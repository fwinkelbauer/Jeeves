using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nancy;
using Nancy.Testing;
using NSubstitute;

namespace Jeeves.Core.UnitTests
{
    [TestClass]
    public class JeevesModuleTests
    {
        [TestMethod]
        public void GetJson_Ok()
        {
            var settings = new JeevesSettings(false);
            var store = CreateStoreWithAdmin("my_api");
            store.RetrieveJson("my_app", "admin", "my_key").Returns("{ \"Data\" : \"Foo\" }");

            var response = PerformHttpRequest(settings, store, "/get/json/my_app/my_key", "my_api");

            AssertOkResponse("{ \"Data\" : \"Foo\" }", response);
        }

        [TestMethod]
        public void GetJson_UnauthorizedNoApikey()
        {
            var settings = new JeevesSettings(false);
            var store = Substitute.For<IDataStore>();

            var response = PerformRequest(settings, store, "/get/json/my_app/my_key", with => with.HttpRequest());

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public void GetJson_RequireHttps_DoNotUseHttps()
        {
            var settings = new JeevesSettings(true);
            var store = CreateStoreWithAdmin("my_api");

            var response = PerformHttpRequest(settings, store, "/get/json/my_app/my_key", "my_api");

            Assert.AreEqual(HttpStatusCode.SeeOther, response.StatusCode);
        }

        [TestMethod]
        public void GetJson_RequireHttps_UseHttps()
        {
            var settings = new JeevesSettings(true);
            var store = CreateStoreWithAdmin("my_api");
            store.RetrieveJson("my_app", "admin", "my_key").Returns("{ \"Data\" : \"Foo\" }");

            var response = PerformRequest(settings, store, "/get/json/my_app/my_key", with =>
            {
                with.Query("apikey", "my_api");
                with.HttpsRequest();
            });

            AssertOkResponse("{ \"Data\" : \"Foo\" }", response);
        }

        [TestMethod]
        public void GetJson_UnauthorizedWrongApikey()
        {
            var settings = new JeevesSettings(false);
            var store = Substitute.For<IDataStore>();

            var response = PerformHttpRequest(settings, store, "/get/json/my_app/my_key", "some_other_secret");

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public void GetJson_ThrowsExceptionIfNoData()
        {
            var settings = new JeevesSettings(false);
            var store = CreateStoreWithAdmin("my_api");

            var response = PerformHttpRequest(settings, store, "/get/json/my_app/my_key", "my_api");

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        private static void AssertOkResponse(string expectedJson, BrowserResponse response)
        {
            Assert.AreEqual("application/json", response.ContentType);
            Assert.AreEqual(expectedJson, response.Body.AsString());
        }

        private IDataStore CreateStoreWithAdmin(string apikey)
        {
            var store = Substitute.For<IDataStore>();
            store.RetrieveUser(apikey).Returns(JeevesUser.CreateAdmin("admin"));

            return store;
        }

        private BrowserResponse PerformHttpRequest(JeevesSettings settings, IDataStore store, string url, string apikey)
        {
            return PerformRequest(settings, store, url, with =>
            {
                with.Query("apikey", apikey);
                with.HttpRequest();
            });
        }

        private BrowserResponse PerformRequest(JeevesSettings settings, IDataStore store, string url, Action<BrowserContext> with)
        {
            using (var boot = new JeevesBootstrapper(settings, store))
            {
                var browser = new Browser(boot);

                return browser.Get(url, with);
            }
        }
    }
}
