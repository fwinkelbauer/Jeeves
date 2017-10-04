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
            var user = new JeevesUser("user01", "app01");
            var expectedString = "User: user01, App: app01";

            var actualOutput = user.ToString();

            Assert.AreEqual(expectedString, actualOutput);
        }

        [TestMethod]
        public void Equals_SameObjects()
        {
            var user1 = new JeevesUser("user01", "app01");
            var user2 = new JeevesUser("user01", "app01");

            Assert.IsTrue(user1.Equals(user2));
            Assert.IsTrue(user1.GetHashCode() == user2.GetHashCode());
        }

        [TestMethod]
        public void Equals_DifferentObjects()
        {
            var user1 = new JeevesUser("user01", "app01");
            var user2 = new JeevesUser("user02", "app02");

            Assert.IsFalse(user1.Equals(user2));
            Assert.IsFalse(user1.GetHashCode() == user2.GetHashCode());
        }

        [TestMethod]
        public void Equals_CannotCast()
        {
            var user1 = new JeevesUser("user01", "app01");
            JeevesUser user2 = null;

            Assert.IsFalse(user1.Equals(user2));
        }

        [TestMethod]
        public void ToUserIdentity_ReturnsUserIdentity()
        {
            var user = new JeevesUser("user01", "app01");

            var identity = user.ToUserIdentity();

            Assert.AreEqual("user01", identity.UserName);
            CollectionAssert.AreEqual(new[] { "user: user01", "app: app01" }, identity.Claims.ToList());
        }
    }
}
