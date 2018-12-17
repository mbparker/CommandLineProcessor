namespace CommandLineLibrary.Contracts
{
    public interface ICommandServiceRegistration
    {
        void RegisterSingleton<TProvider, TService>()
            where TProvider : TService;

        void RegisterTransient<TProvider, TService>()
            where TProvider : TService;
    }
}