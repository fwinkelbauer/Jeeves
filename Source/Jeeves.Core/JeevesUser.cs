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

        public string UserName { get; set; }

        public string ApplicationName { get; set; }

        public bool CanWrite { get; set; }

        public override bool Equals(object obj)
        {
            // TODO fw test me
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
            // TODO fw test me
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            // TODO fw test me
            return $"User: {UserName}, App: {ApplicationName}, Write: {CanWrite}";
        }

        internal IUserIdentity ToUserIdentity()
        {
            // TODO fw test me
            var userClaim = $"user: {UserName}";
            var appClaim = $"app: {ApplicationName}";
            var accessClaim = CanWrite ? "access: read" : "access: read/write";

            return new UserIdentity(UserName, userClaim, appClaim, accessClaim);
        }

        private class UserIdentity : IUserIdentity
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
