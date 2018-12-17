namespace CommandLineLibrary
{
    using CommandLineLibrary.Contracts;

    public class CommandContext : ICommandContext
    {
        private readonly ICommandServiceProvider commandServiceProvider;

        public CommandContext(ICommandServiceProvider commandServiceProvider, ICommandDataStore dataStore)
        {
            this.commandServiceProvider = commandServiceProvider;
            DataStore = dataStore;
        }

        public ICommandDataStore DataStore { get; }

        public T GetService<T>()
        {
            return commandServiceProvider.Resolve<T>();
        }
    }
}