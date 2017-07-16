namespace Jeeves.Core
{
    public interface IDataStore
    {
        string RetrieveValue(string application, string userName, string key);

        void PutValue(string application, string userName, string key, string value);

        JeevesUser RetrieveUser(string apikey);
    }
}
