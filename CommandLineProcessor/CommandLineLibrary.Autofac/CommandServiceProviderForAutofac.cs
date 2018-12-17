namespace CommandLineLibrary.Autofac
{
    using CommandLineLibrary.Contracts;

    using global::Autofac;

    public class CommandServiceProviderForAutofac : ICommandServiceProvider
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