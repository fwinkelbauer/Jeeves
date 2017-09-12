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
        public void GetValue_Ok_WithHttp()
        {
            var settings = CreateSettings(SecurityOption.Http);
            var store = CreateStoreWithUser();
            store.RetrieveValue("admin", "my_app", "my_key").Returns("{ \"Data\" : \"Foo\" }");

            var response = PerformGetRequest(settings, store, "/get/admin/my_app/my_key", with =>
            {
                with.HttpRequest();
            });

            AssertOkResponse("{ \"Data\" : \"Foo\" }", response);
        }

        [TestMethod]
        public void GetValue_Ok_WithHttpsAndAuthentication()
        {
            var settings = CreateSettings(SecurityOption.HttpsAndAuthentication);
            var store = CreateStoreWithUser();
            store.RetrieveValue("admin", "my_app", "my_key").Returns("{ \"Data\" : \"Foo\" }");

            var response = PerformGetRequest(settings, store, "/get/admin/my_app/my_key", with =>
            {
                with.Query("apikey", "my_api");
                with.HttpsRequest();
            });

            AssertOkResponse("{ \"Data\" : \"Foo\" }", response);
        }

        [TestMethod]
        public void GetValue_SeeOther_NoHttps()
        {
            var settings = CreateSettings(SecurityOption.HttpsAndAuthentication);
            var store = CreateStoreWithUser();

            var response = PerformGetRequest(settings, store, "/get/admin/my_app/my_key", with =>
            {
                with.HttpRequest();
            });

            Assert.AreEqual(HttpStatusCode.SeeOther, response.StatusCode);
        }

        [TestMethod]
        public void GetValue_Unauthorized_NoApikey()
        {
            var settings = CreateSettings(SecurityOption.HttpsAndAuthentication);
            var store = Substitute.For<IDataStore>();

            var response = PerformGetRequest(settings, store, "/get/admin/my_app/my_key", with => with.HttpsRequest());

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public void GetValue_Unauthorized_WrongApikey()
        {
            var settings = CreateSettings(SecurityOption.HttpsAndAuthentication);
            var store = Substitute.For<IDataStore>();

            var response = PerformGetRequest(settings, store, "/get/admin/my_app/my_key", with =>
            {
                with.Query("apikey", "some_other_secret");
                with.HttpsRequest();
            });

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public void GetValue_NoContent_NoValueInDataStore()
        {
            var settings = CreateSettings(SecurityOption.Http);
            var store = CreateStoreWithUser();

            var response = PerformGetRequest(settings, store, "/get/admin/my_app/my_key", with =>
            {
                with.HttpRequest();
            });

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        [TestMethod]
        public void PostValue_Ok_WithHttpsAndAuthentication()
        {
            var settings = CreateSettings(SecurityOption.HttpsAndAuthentication);
            var store = CreateStoreWithUser();

            var response = PerformPostRequest(settings, store, "/post/admin/my_app/my_key", with =>
            {
                with.Query("apikey", "my_api");
                with.FormValue("value", "barbarbar");
                with.HttpsRequest();
            });

            store.Received().PutValue("admin", "my_app", "my_key", "barbarbar");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public void PostValue_Ok_WithHttp()
        {
            var settings = CreateSettings(SecurityOption.Http);
            var store = Substitute.For<IDataStore>();

            var response = PerformPostRequest(settings, store, "/post/admin/my_app/my_key", with =>
            {
                with.FormValue("value", "barbarbar");
                with.HttpRequest();
            });

            store.Received().PutValue("admin", "my_app", "my_key", "barbarbar");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public void PostValue_BadRequest_NoContent()
        {
            var settings = CreateSettings(SecurityOption.HttpsAndAuthentication);
            var store = CreateStoreWithUser();

            var response = PerformPostRequest(settings, store, "/post/admin/my_app/my_key", with =>
            {
                with.Query("apikey", "my_api");
                with.HttpsRequest();
            });

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void PostValue_BadRequest_EmptyContent()
        {
            var settings = CreateSettings(SecurityOption.HttpsAndAuthentication);
            var store = CreateStoreWithUser();

            var response = PerformPostRequest(settings, store, "/post/admin/my_app/my_key", with =>
            {
                with.Query("apikey", "my_api");
                with.FormValue("value", string.Empty);
                with.HttpsRequest();
            });

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        private static void AssertOkResponse(string expectedValue, BrowserResponse response)
        {
            Assert.AreEqual("text/html", response.ContentType);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(expectedValue, response.Body.AsString());
        }

        private JeevesSettings CreateSettings(SecurityOption security)
        {
            return new JeevesSettings("some_uri", security);
        }

        private IDataStore CreateStoreWithUser()
        {
            var store = Substitute.For<IDataStore>();
            store.RetrieveUser("my_api").Returns(new JeevesUser("admin", "my_app", true));

            return store;
        }

        private BrowserResponse PerformPostRequest(JeevesSettings settings, IDataStore store, string url, Action<BrowserContext> with)
        {
            return PerformRequest(settings, store, b => b.Post(url, with));
        }

        private BrowserResponse PerformGetRequest(JeevesSettings settings, IDataStore store, string url, Action<BrowserContext> with)
        {
            return PerformRequest(settings, store, b => b.Get(url, with));
        }

        private BrowserResponse PerformRequest(JeevesSettings settings, IDataStore store, Func<Browser, BrowserResponse> request)
        {
            using (var boot = new JeevesBootstrapper(settings, store, Substitute.For<IJeevesLog>()))
            {
                var browser = new Browser(boot);

                return request(browser);
            }
        }
    }
}
