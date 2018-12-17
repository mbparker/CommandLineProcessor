namespace CommandLineLibrary.Autofac
{
    using CommandLineLibrary.Contracts;

    using global::Autofac;

    public class ContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var autofacRegistration = new CommandServiceRegistrationForAutofac(builder);
            CommandLineLibrary.ContainerModule.RegisterServices(autofacRegistration);
            builder.RegisterType<CommandServiceProviderForAutofac>().As<ICommandServiceProvider>().SingleInstance();
        }
    }
}