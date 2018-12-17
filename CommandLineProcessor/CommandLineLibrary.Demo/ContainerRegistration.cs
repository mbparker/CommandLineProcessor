namespace CommandLineLibrary.Demo
{
    using Autofac;

    using CommandLineLibrary.Contracts;
    using CommandLineLibrary.Contracts.Commands.Registration;
    using CommandLineLibrary.Demo.Commands;

    public static class ContainerRegistration
    {
        private static IContainer container;

        public static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            // Register Framework
            builder.RegisterType<CommandRegistrations>().As<IRootCommandRegistration>().SingleInstance();
            builder.RegisterType<CommandLineProcessorProvider>().As<ICommandLineProcessorService>().SingleInstance();
            builder.RegisterType<CommandRepositoryProvider>().As<ICommandRepositoryService>().SingleInstance();
            builder.RegisterType<CommandHistoryProvider>().As<ICommandHistoryService>().SingleInstance();
            builder.RegisterType<CommandPathCalculator>().As<ICommandPathCalculator>().InstancePerDependency();
            builder.RegisterType<InputHandlerProvider>().As<IInputHandlerService>().InstancePerDependency();
            builder.RegisterType<CommandContext>().As<ICommandContext>().InstancePerDependency();
            builder.RegisterType<CommandDataStore>().As<ICommandDataStore>().InstancePerDependency();
            builder.RegisterType<MethodCallValidatorProvider>().As<IMethodCallValidatorService>().SingleInstance();
            builder.RegisterType<CommandMethodFactoryProvider>().As<ICommandMethodFactoryService>().SingleInstance();
            builder.RegisterType<CommandLineInterface>().As<ICommandLineInterface>().SingleInstance();
            

            // Register Demo-Specific Types
            builder.RegisterType<EchoCommand>().InstancePerDependency();
            builder.RegisterType<ExitCommand>().InstancePerDependency();
            builder.RegisterType<MathCommand>().InstancePerDependency();
            builder.RegisterType<CommandDescriptors>().InstancePerDependency();
            builder.RegisterType<ApplicationInstance>().As<IApplication, ICommandHistoryWriter>().SingleInstance();
            builder.RegisterType<CommandServiceProviderForAutofac>().As<ICommandServiceProvider>().SingleInstance();

            builder.Register<IContainer>(x => container).SingleInstance();
            container = builder.Build();
            return container;
        }

        private class CommandServiceProviderForAutofac : ICommandServiceProvider
        {
            private readonly IContainer container;

            public CommandServiceProviderForAutofac(IContainer container)
            {
                this.container = container;
            }

            public T Resolve<T>()
            {
                return container.Resolve<T>();
            }
        }
    }
}