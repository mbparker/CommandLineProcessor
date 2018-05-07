namespace CommandLineProcessorDemo
{
    using CommandLineProcessorContracts;
    using CommandLineProcessorContracts.Ioc;

    using CommandLineProcessorLib;

    public static class ContainerRegistration
    {
        public static void RegisterServices(IIocContainer container)
        {
            container.Register<ICommandLineProcessorService, CommandLineProcessorProvider>(ServiceLifestyle.Singleton);
            container.Register<ICommandRepositoryService, CommandRepositoryProvider>(ServiceLifestyle.Singleton);
            container.Register<ICommandPathCalculator, CommandPathCalculator>(ServiceLifestyle.Transient);
            container.Register<IInputHandlerService, InputHandlerProvider>(ServiceLifestyle.Transient);
        }
    }
}