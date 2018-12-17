namespace CommandLineLibrary.Contracts
{
    public interface ICommandDataStore
    {
        void Clear();

        T Get<T>(string key);

        void Set<T>(string key, T value);
    }
}