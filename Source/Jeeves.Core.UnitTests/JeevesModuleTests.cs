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
            var store = Substitute.For<IDataStore>();
            store.RetrieveJson("my_app", "me", "my_key").Returns("{ \"Revision\" : 0, \"Data\" : \"Foo\" }");
            store.RetrieveUser("my_api").Returns(JeevesUser.CreateAdmin("me"));

            var response = PerformRequest(settings, store, "/get/json/my_app/my_key", with =>
            {
                with.Query("apikey", "my_api");
                with.HttpRequest();
            });

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("application/json", response.ContentType);
            Assert.AreEqual("{ \"Revision\" : 0, \"Data\" : \"Foo\" }", response.Body.AsString());
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
        public void GetJson_RequireHttps_NoHttps()
        {
            var settings = new JeevesSettings(true);
            var store = Substitute.For<IDataStore>();
            store.RetrieveUser("my_api").Returns(JeevesUser.CreateAdmin("me"));

            var response = PerformRequest(settings, store, "/get/json/my_app/my_key", with =>
            {
                with.Query("apikey", "my_api");
                with.HttpRequest();
            });

            Assert.AreEqual(HttpStatusCode.SeeOther, response.StatusCode);
        }

        [TestMethod]
        public void GetJson_RequireHttps_UseHttps()
        {
            var settings = new JeevesSettings(true);
            var store = Substitute.For<IDataStore>();
            store.RetrieveJson("my_app", "me", "my_key").Returns("{ \"Revision\" : 0, \"Data\" : \"Foo\" }");
            store.RetrieveUser("my_api").Returns(JeevesUser.CreateAdmin("me"));

            var response = PerformRequest(settings, store, "/get/json/my_app/my_key", with =>
            {
                with.Query("apikey", "my_api");
                with.HttpsRequest();
            });

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public void GetJson_UnauthorizedWrongApikey()
        {
            var settings = new JeevesSettings(false);
            var store = Substitute.For<IDataStore>();

            var response = PerformRequest(settings, store, "/get/json/my_app/my_key", with =>
            {
                with.Query("apikey", "some_other_secret");
                with.HttpRequest();
            });

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public void GetJson_ThrowsExceptionIfNoData()
        {
            var settings = new JeevesSettings(false);
            var store = Substitute.For<IDataStore>();
            store.RetrieveUser("my_api").Returns(JeevesUser.CreateAdmin("me"));

            var response = PerformRequest(settings, store, "/get/json/my_app/my_key", with =>
            {
                with.Query("apikey", "my_api");
                with.HttpRequest();
            });

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
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
