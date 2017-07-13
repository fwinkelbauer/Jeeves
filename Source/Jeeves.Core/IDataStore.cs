namespace Jeeves.Core
{
    public interface IDataStore
    {
        string RetrieveValue(string application, string host, string key);

        JeevesUser RetrieveUser(string apikey);
    }
}
