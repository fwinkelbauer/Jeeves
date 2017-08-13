using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jeeves.Core.UnitTests
{
    [TestClass]
    public class JeevesUserTests
    {
        [TestMethod]
        public void ToString_ReturnsString()
        {
            var user = CreateUser("user01", "app01", false);
            var expectedString = "User: user01, App: app01, Write: False";

            var actualOutput = user.ToString();

            Assert.AreEqual(expectedString, actualOutput);
        }

        [TestMethod]
        public void Equals_SameObjects()
        {
            var user1 = CreateUser("user01", "app01", false);
            var user2 = CreateUser("user01", "app01", false);

            Assert.IsTrue(user1.Equals(user2));
            Assert.IsTrue(user1.GetHashCode() == user2.GetHashCode());
        }

        [TestMethod]
        public void Equals_DifferentObjects()
        {
            var user1 = CreateUser("user01", "app01", false);
            var user2 = CreateUser("user02", "app02", true);

            Assert.IsFalse(user1.Equals(user2));
            Assert.IsFalse(user1.GetHashCode() == user2.GetHashCode());
        }

        [TestMethod]
        public void Equals_CannotCast()
        {
            var user1 = CreateUser("user01", "app01", false);
            JeevesUser user2 = null;

            Assert.IsFalse(user1.Equals(user2));
        }

        [DataTestMethod]
        [DataRow(false, "access: read")]
        [DataRow(true, "access: read/write")]
        public void ToUserIdentity_ReturnsUserIdentity(bool canWrite, string expectedClaim)
        {
            var user = CreateUser("user01", "app01", canWrite);

            var identity = user.ToUserIdentity();

            Assert.AreEqual("user01", identity.UserName);
            CollectionAssert.AreEqual(new[] { "user: user01", "app: app01", expectedClaim }, identity.Claims.ToList());
        }

        private JeevesUser CreateUser(string user, string app, bool canWrite)
        {
            return new JeevesUser()
            {
                UserName = user,
                Application = app,
                CanWrite = canWrite
            };
        }
    }
}
