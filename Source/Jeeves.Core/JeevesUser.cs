using System.Collections.Generic;
using Jeeves.Common;
using Nancy.Security;

namespace Jeeves.Core
{
    public class JeevesUser
    {
        public JeevesUser(string userName, string applicationName, bool canWrite)
        {
            UserName = userName.ThrowIfNull(nameof(userName));
            ApplicationName = applicationName.ThrowIfNull(nameof(applicationName));
            CanWrite = canWrite;
        }

        public string UserName { get; }

        public string ApplicationName { get; }

        public bool CanWrite { get; }

        public override bool Equals(object obj)
        {
            var user = obj as JeevesUser;

            if (user == null)
            {
                return false;
            }

            return UserName.Equals(user.UserName)
                && ApplicationName.Equals(user.ApplicationName)
                && CanWrite == user.CanWrite;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"User: {UserName}, App: {ApplicationName}, Write: {CanWrite}";
        }

        internal IUserIdentity ToUserIdentity()
        {
            var userClaim = $"user: {UserName}";
            var appClaim = $"app: {ApplicationName}";
            var accessClaim = CanWrite ? "access: read/write" : "access: read";

            return new UserIdentity(UserName, userClaim, appClaim, accessClaim);
        }

        internal class UserIdentity : IUserIdentity
        {
            public UserIdentity(string userName, params string[] claims)
            {
                UserName = userName;
                Claims = claims;
            }

            public string UserName { get; }

            public IEnumerable<string> Claims { get; }
        }
    }
}
