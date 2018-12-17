namespace CommandLineLibrary
{
    using CommandLineLibrary.Contracts;
    using CommandLineLibrary.Contracts.Commands.Registration;

    public static class ContainerModule
    {
        public static void RegisterServices(ICommandServiceRegistration registration)
        {
            registration.RegisterSingleton<CommandRegistrations, IRootCommandRegistration>();
            registration.RegisterSingleton<CommandLineProcessorProvider, ICommandLineProcessorService>();
            registration.RegisterSingleton<CommandRepositoryProvider, ICommandRepositoryService>();
            registration.RegisterSingleton<CommandHistoryProvider, ICommandHistoryService>();
            registration.RegisterTransient<CommandPathCalculator, ICommandPathCalculator>();
            registration.RegisterTransient<InputHandlerProvider, IInputHandlerService>();
            registration.RegisterTransient<CommandContext, ICommandContext>();
            registration.RegisterTransient<CommandDataStore, ICommandDataStore>();
            registration.RegisterSingleton<MethodCallValidatorProvider, IMethodCallValidatorService>();
            registration.RegisterSingleton<CommandMethodFactoryProvider, ICommandMethodFactoryService>();
            registration.RegisterSingleton<CommandLineInterface, ICommandLineInterface>();
        }
    }
}