using System.Collections.Generic;
using Jeeves.Common;
using Nancy.Security;

namespace Jeeves.Core
{
    public class JeevesUser
    {
        public string UserName { get; set; }

        public string ApplicationName { get; set; }

        public bool CanWrite { get; set; }

        public bool IsAdmin { get; set; }

        public static JeevesUser CreateAdmin(string userName)
        {
            // TODO fw test me
            return new JeevesUser()
            {
                UserName = userName.ThrowIfNull(nameof(userName)),
                ApplicationName = string.Empty,
                CanWrite = true,
                IsAdmin = true
            };
        }

        public static JeevesUser CreateUser(string userName, string applicationName, bool canWrite)
        {
            // TODO fw test me
            return new JeevesUser()
            {
                UserName = userName.ThrowIfNull(nameof(userName)),
                ApplicationName = applicationName.ThrowIfNull(nameof(applicationName)),
                CanWrite = canWrite,
                IsAdmin = false
            };
        }

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
                && CanWrite == user.CanWrite
                && IsAdmin == user.IsAdmin;
        }

        public override int GetHashCode()
        {
            // TODO fw test me
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            // TODO fw test me
            return $"User: {UserName}, App: {ApplicationName}, Write: {CanWrite}, Admin: {IsAdmin}";
        }

        internal IUserIdentity ToUserIdentity()
        {
            // TODO fw test me
            if (IsAdmin)
            {
                return new UserIdentity(UserName, "Admin");
            }
            else
            {
                return new UserIdentity(UserName, $"({ApplicationName})", CanWrite ? "Read/Write" : "Read");
            }
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
