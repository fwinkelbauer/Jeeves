namespace Jeeves.Core
{
    public interface IUserAuthenticator
    {
        JeevesUser RetrieveUser(string apikey);
    }
}
