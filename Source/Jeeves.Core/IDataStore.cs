namespace Jeeves.Core
{
    public interface IDataStore
    {
        string RetrieveValue(string userName, string application, string key);

        void PutValue(string userName, string application, string key, string value);

        JeevesUser RetrieveUser(string apikey);
    }
}
