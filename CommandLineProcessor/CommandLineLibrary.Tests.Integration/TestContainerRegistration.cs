namespace CommandLineLibrary.Tests.Integration
{
    using CommandLineLibrary.Autofac;
    using CommandLineLibrary.Contracts;
    using CommandLineLibrary.Demo.Commands;
    using CommandLineLibrary.Tests.Integration.Services;

    using global::Autofac;

    public static class TestContainerRegistration
    {
        private static IContainer container;

        public static IContainer RegisterServices()
        {
            var builder = new ContainerBuilder();

            // Register Framework
            builder.RegisterModule<ContainerModule>();

            builder.RegisterType<EchoCommand>().InstancePerDependency();
            builder.RegisterType<ExitCommand>().InstancePerDependency();
            builder.RegisterType<MathCommand>().InstancePerDependency();
            builder.RegisterType<CommandDescriptors>().InstancePerDependency();
            builder.RegisterType<TestCommandHistoryWriter>().As<ICommandHistoryWriter, ITestCommandHistoryWriter>()
                .SingleInstance();
            builder.RegisterType<TestApplication>().As<IApplication, ITestApplication>().SingleInstance();

            builder.Register(x => container).SingleInstance();
            container = builder.Build();
            return container;
        }
    }
}