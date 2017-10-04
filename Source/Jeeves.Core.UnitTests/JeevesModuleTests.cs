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
        private readonly Uri _uriHttp = new Uri("http://localhost:9042/jeeves/");
        private readonly Uri _uriHttps = new Uri("https://localhost:9042/jeeves/");

        [TestMethod]
        public void Bootstrapper_Exception_AuthenticatorWithoutHttps()
        {
            var store = Substitute.For<IDataStore>();
            var authenticator = Substitute.For<IUserAuthenticator>();

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                using (var boot = new JeevesBootstrapper(_uriHttp, store, authenticator, Substitute.For<IJeevesLog>()))
                {
                    // do nothing
                }
            });
        }

        [TestMethod]
        public void GetValue_Ok_WithHttp()
        {
            var store = Substitute.For<IDataStore>();
            store.RetrieveValue("admin", "my_app", "my_key").Returns("{ \"Data\" : \"Foo\" }");

            var response = PerformGetRequest(_uriHttp, store, null, "/get/admin/my_app/my_key", with =>
            {
                with.HttpRequest();
            });

            AssertOkResponse("{ \"Data\" : \"Foo\" }", response);
        }

        [TestMethod]
        public void GetValue_Ok_WithHttpsAndAuthentication()
        {
            var store = Substitute.For<IDataStore>();
            store.RetrieveValue("admin", "my_app", "my_key").Returns("{ \"Data\" : \"Foo\" }");

            var authenticator = CreateAuthenticatorWithUser();

            var response = PerformGetRequest(_uriHttps, store, authenticator, "/get/admin/my_app/my_key", with =>
            {
                with.Query("apikey", "my_api");
                with.HttpsRequest();
            });

            AssertOkResponse("{ \"Data\" : \"Foo\" }", response);
        }

        [TestMethod]
        public void GetValue_SeeOther_NoHttps()
        {
            var store = Substitute.For<IDataStore>();

            var response = PerformGetRequest(_uriHttps, store, null, "/get/admin/my_app/my_key", with =>
            {
                with.HttpRequest();
            });

            Assert.AreEqual(HttpStatusCode.SeeOther, response.StatusCode);
        }

        [TestMethod]
        public void GetValue_Unauthorized_NoApikey()
        {
            var store = Substitute.For<IDataStore>();
            var authenticator = Substitute.For<IUserAuthenticator>();

            var response = PerformGetRequest(_uriHttps, store, authenticator, "/get/admin/my_app/my_key", with => with.HttpsRequest());

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public void GetValue_Unauthorized_WrongApikey()
        {
            var store = Substitute.For<IDataStore>();
            var authenticator = Substitute.For<IUserAuthenticator>();

            var response = PerformGetRequest(_uriHttps, store, authenticator, "/get/admin/my_app/my_key", with =>
            {
                with.Query("apikey", "some_other_secret");
                with.HttpsRequest();
            });

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public void GetValue_NoContent_NoValueInDataStore()
        {
            var store = Substitute.For<IDataStore>();

            var response = PerformGetRequest(_uriHttp, store, null, "/get/admin/my_app/my_key", with =>
            {
                with.HttpRequest();
            });

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        [TestMethod]
        public void PostValue_Ok_WithHttpsAndAuthentication()
        {
            var store = Substitute.For<IDataStore>();
            var authenticator = CreateAuthenticatorWithUser();

            var response = PerformPostRequest(_uriHttps, store, authenticator, "/post/admin/my_app/my_key", with =>
            {
                with.Query("apikey", "my_api");
                with.FormValue("value", "barbarbar");
                with.HttpsRequest();
            });

            store.Received().StoreValue("admin", "my_app", "my_key", "barbarbar");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public void PostValue_Ok_WithHttp()
        {
            var store = Substitute.For<IDataStore>();

            var response = PerformPostRequest(_uriHttp, store, null, "/post/admin/my_app/my_key", with =>
            {
                with.FormValue("value", "barbarbar");
                with.HttpRequest();
            });

            store.Received().StoreValue("admin", "my_app", "my_key", "barbarbar");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public void PostValue_BadRequest_NoContent()
        {
            var store = Substitute.For<IDataStore>();
            var authenticator = CreateAuthenticatorWithUser();

            var response = PerformPostRequest(_uriHttps, store, authenticator, "/post/admin/my_app/my_key", with =>
            {
                with.Query("apikey", "my_api");
                with.HttpsRequest();
            });

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void PostValue_BadRequest_EmptyContent()
        {
            var store = Substitute.For<IDataStore>();
            var authenticator = CreateAuthenticatorWithUser();

            var response = PerformPostRequest(_uriHttps, store, authenticator, "/post/admin/my_app/my_key", with =>
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

        private IUserAuthenticator CreateAuthenticatorWithUser()
        {
            var authenticator = Substitute.For<IUserAuthenticator>();
            authenticator.RetrieveUser(Arg.Any<string>()).Returns(new JeevesUser("admin", "my_app"));

            return authenticator;
        }

        private BrowserResponse PerformPostRequest(Uri baseUrl, IDataStore store, IUserAuthenticator authenticator, string url, Action<BrowserContext> with)
        {
            return PerformRequest(baseUrl, store, authenticator, b => b.Post(url, with));
        }

        private BrowserResponse PerformGetRequest(Uri baseUrl, IDataStore store, IUserAuthenticator authenticator, string url, Action<BrowserContext> with)
        {
            return PerformRequest(baseUrl, store, authenticator, b => b.Get(url, with));
        }

        private BrowserResponse PerformRequest(Uri baseUrl, IDataStore store, IUserAuthenticator authenticator, Func<Browser, BrowserResponse> request)
        {
            using (var boot = new JeevesBootstrapper(baseUrl, store, authenticator, Substitute.For<IJeevesLog>()))
            {
                var browser = new Browser(boot);

                return request(browser);
            }
        }
    }
}
