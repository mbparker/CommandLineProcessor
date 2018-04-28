namespace CommandLineProcessorDemo.Ioc
{
    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using CommandLineProcessorContracts.Ioc;

    public static class IocContainerHolder
    {
        private static IWindsorContainer container;

        public static IIocContainer Container => container?.Resolve<IIocContainer>();

        public static void CreateContainer()
        {
            if (container == null)
            {
                container = new WindsorContainer();
                container.AddFacility<TypedFactoryFacility>();
                container.Register(
                    Component.For<IIocContainer>().ImplementedBy<IocContainer>().LifestyleTransient()
                        .UsingFactoryMethod(() => new IocContainer(container)));
            }
        }

        public static void DisposeContainer()
        {
            if (container != null)
            {
                container.Dispose();
                container = null;
            }
        }
    }
}