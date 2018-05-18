namespace CommandLineProcessorLib
{
    using CommandLineProcessorCommon.Ioc;

    using CommandLineProcessorContracts;

    public class CommandContext : ICommandContext
    {
        private readonly IIocContainer container;

        public CommandContext(IIocContainer container, ICommandDataStore dataStore)
        {
            this.container = container;
            DataStore = dataStore;
        }

        public ICommandDataStore DataStore { get; }

        public T GetService<T>()
        {
            return container.Resolve<T>();
        }
    }
}