using System.Collections.Generic;
using Nancy.Security;

namespace Jeeves.Core
{
    public class JeevesUser
    {
        public JeevesUser(string userName, string application)
        {
            UserName = userName;
            Application = application;
        }

        public string UserName { get; }

        public string Application { get; }

        public override bool Equals(object obj)
        {
            var user = obj as JeevesUser;

            if (user == null)
            {
                return false;
            }

            return UserName.Equals(user.UserName)
                && Application.Equals(user.Application);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"User: {UserName}, App: {Application}";
        }

        internal IUserIdentity ToUserIdentity()
        {
            return new UserIdentity(UserName, $"user: {UserName}", $"app: {Application}");
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
