namespace CommandLineLibrary.Demo
{
    using CommandLineLibrary.Autofac;
    using CommandLineLibrary.Contracts;
    using CommandLineLibrary.Demo.Commands;

    using global::Autofac;

    public static class ContainerRegistration
    {
        private static IContainer container;

        public static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            // Register Framework
            builder.RegisterModule<ContainerModule>();

            // Register Demo-Specific Types
            builder.RegisterType<EchoCommand>().InstancePerDependency();
            builder.RegisterType<ExitCommand>().InstancePerDependency();
            builder.RegisterType<MathCommand>().InstancePerDependency();
            builder.RegisterType<ConfigCommand>().InstancePerDependency();
            builder.RegisterType<CommandDescriptors>().InstancePerDependency();
            builder.RegisterType<ApplicationInstance>().As<IApplication, ICommandHistoryWriter>().SingleInstance();

            builder.Register(x => container).SingleInstance();
            container = builder.Build();
            return container;
        }
    }
}