namespace CommandLineLibrary.Autofac
{
    using CommandLineLibrary.Contracts;

    using global::Autofac;

    public class CommandServiceRegistrationForAutofac : ICommandServiceRegistration
    {
        private readonly ContainerBuilder builder;

        public CommandServiceRegistrationForAutofac(ContainerBuilder builder)
        {
            this.builder = builder;
        }

        public void RegisterSingleton<TProvider, TService>()
            where TProvider : TService
        {
            builder.RegisterType<TProvider>().As<TService>().SingleInstance();
        }

        public void RegisterTransient<TProvider, TService>()
            where TProvider : TService
        {
            builder.RegisterType<TProvider>().As<TService>().InstancePerDependency();
        }
    }
}