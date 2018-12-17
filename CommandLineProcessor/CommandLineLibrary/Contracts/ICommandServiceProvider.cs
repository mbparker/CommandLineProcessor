namespace CommandLineLibrary.Contracts
{
    public interface ICommandServiceProvider
    {
        T Resolve<T>();
    }
}