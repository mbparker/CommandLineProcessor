namespace CommandLineProcessorTests.IntegrationTests
{
    using CommandLineProcessorCommon.Ioc;

    using CommandLineProcessorContracts;
    using CommandLineProcessorContracts.Commands.Registration;

    using CommandLineProcessorLib;

    using global::CommandLineProcessorTests.IntegrationTests.Services;

    public static class ContainerRegistration
    {
        public static void RegisterServices(IIocContainer container)
        {
            container.Register<IRootCommandRegistration, CommandRegistrations>(ServiceLifestyle.Singleton);
            container.Register<ICommandLineProcessorService, CommandLineProcessorProvider>(ServiceLifestyle.Singleton);
            container.Register<ICommandRepositoryService, CommandRepositoryProvider>(ServiceLifestyle.Singleton);
            container.Register<ICommandHistoryService, CommandHistoryProvider>(ServiceLifestyle.Singleton);
            container.Register<ICommandPathCalculator, CommandPathCalculator>(ServiceLifestyle.Transient);
            container.Register<IInputHandlerService, InputHandlerProvider>(ServiceLifestyle.Transient);
            container.Register<ICommandContext, CommandContext>(ServiceLifestyle.Transient);
            container.Register<ICommandDataStore, CommandDataStore>(ServiceLifestyle.Transient);
            container.RegisterAsFactory<ICommandContextFactory>(ServiceLifestyle.Singleton);
            container.Register<IMethodCallValidatorService, MethodCallValidatorProvider>(ServiceLifestyle.Singleton);
            container.Register<ICommandMethodFactoryService, CommandMethodFactoryProvider>(ServiceLifestyle.Singleton);
            container.Register<ICommandHistoryWriter, ITestCommandHistoryWriter, TestCommandHistoryWriter>(ServiceLifestyle.Singleton);
        }
    }
}