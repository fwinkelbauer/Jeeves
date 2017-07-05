namespace Jeeves.Core
{
    public interface IDataStore
    {
        string RetrieveJson(string application, string host, string key);

        JeevesUser RetrieveUser(string apikey);
    }
}
