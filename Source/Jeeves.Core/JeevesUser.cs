using System.Collections.Generic;
using Nancy.Security;

namespace Jeeves.Core
{
    public class JeevesUser
    {
        public JeevesUser(string userName, string application, bool canWrite)
        {
            UserName = userName;
            Application = application;
            CanWrite = canWrite;
        }

        public string UserName { get; }

        public string Application { get; }

        public bool CanWrite { get; }

        public override bool Equals(object obj)
        {
            var user = obj as JeevesUser;

            if (user == null)
            {
                return false;
            }

            return UserName.Equals(user.UserName)
                && Application.Equals(user.Application)
                && CanWrite == user.CanWrite;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"User: {UserName}, App: {Application}, Write: {CanWrite}";
        }

        internal IUserIdentity ToUserIdentity()
        {
            var userClaim = $"user: {UserName}";
            var appClaim = $"app: {Application}";
            var accessClaim = CanWrite ? "access: read/write" : "access: read";

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
