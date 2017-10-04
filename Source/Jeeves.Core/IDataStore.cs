namespace Jeeves.Core
{
    public interface IDataStore
    {
        string RetrieveValue(string userName, string application, string key);

        void StoreValue(string userName, string application, string key, string value);
    }
}
