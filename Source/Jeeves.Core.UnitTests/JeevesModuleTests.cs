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
        public void GetValue_Ok_UseHttp()
        {
            var settings = new JeevesSettings(false);
            var store = CreateStoreWithAdmin("my_api");
            store.RetrieveValue("my_app", "admin", "my_key").Returns("{ \"Data\" : \"Foo\" }");

            var response = PerformHttpRequest(settings, store, "/get/my_app/my_key", "my_api");

            AssertOkResponse("{ \"Data\" : \"Foo\" }", response);
        }

        [TestMethod]
        public void GetValue_Ok_UseHttps()
        {
            var settings = new JeevesSettings(true);
            var store = CreateStoreWithAdmin("my_api");
            store.RetrieveValue("my_app", "admin", "my_key").Returns("{ \"Data\" : \"Foo\" }");

            var response = PerformRequest(settings, store, "/get/my_app/my_key", with =>
            {
                with.Query("apikey", "my_api");
                with.HttpsRequest();
            });

            AssertOkResponse("{ \"Data\" : \"Foo\" }", response);
        }

        [TestMethod]
        public void GetValue_SeeOther_NoHttps()
        {
            var settings = new JeevesSettings(true);
            var store = CreateStoreWithAdmin("my_api");

            var response = PerformHttpRequest(settings, store, "/get/my_app/my_key", "my_api");

            Assert.AreEqual(HttpStatusCode.SeeOther, response.StatusCode);
        }

        [TestMethod]
        public void GetValue_Unauthorized_NoApikey()
        {
            var settings = new JeevesSettings(false);
            var store = Substitute.For<IDataStore>();

            var response = PerformRequest(settings, store, "/get/my_app/my_key", with => with.HttpRequest());

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public void GetValue_Unauthorized_WrongApikey()
        {
            var settings = new JeevesSettings(false);
            var store = Substitute.For<IDataStore>();

            var response = PerformHttpRequest(settings, store, "/get/my_app/my_key", "some_other_secret");

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public void GetValue_NoContent_NoValueInDataStore()
        {
            var settings = new JeevesSettings(false);
            var store = CreateStoreWithAdmin("my_api");

            var response = PerformHttpRequest(settings, store, "/get/my_app/my_key", "my_api");

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        private static void AssertOkResponse(string expectedValue, BrowserResponse response)
        {
            Assert.AreEqual("text/html", response.ContentType);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(expectedValue, response.Body.AsString());
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
