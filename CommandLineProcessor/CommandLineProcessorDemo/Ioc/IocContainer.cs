namespace CommandLineProcessorDemo.Ioc
{
    using System;
    using System.Collections.Generic;

    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using CommandLineProcessorContracts.Ioc;

    public class IocContainer : IIocContainer
    {
        private readonly IWindsorContainer innerContainer;

        public IocContainer(IWindsorContainer innerContainer)
        {
            this.innerContainer = innerContainer;
        }

        public void Register<TService, TImplementation>(ServiceLifestyle lifestyle)
            where TService : class where TImplementation : TService
        {
            var registration = Component.For<TService>().ImplementedBy<TImplementation>();
            Register(registration, lifestyle);
        }

        public void Register<TService1, TService2, TImplementation>(ServiceLifestyle lifestyle)
            where TService1 : class where TService2 : class where TImplementation : TService1, TService2
        {
            var registration = Component.For<TService1, TService2>().ImplementedBy<TImplementation>();
            Register(registration, lifestyle);
        }

        public void Register<TService1, TService2, TService3, TImplementation>(ServiceLifestyle lifestyle)
            where TService1 : class
            where TService2 : class
            where TService3 : class
            where TImplementation : TService1, TService2, TService3
        {
            var registration = Component.For<TService1, TService2, TService3>().ImplementedBy<TImplementation>();
            Register(registration, lifestyle);
        }

        public void Register(Type service, Type implementation, ServiceLifestyle lifestyle)
        {
            var registration = Component.For(service).ImplementedBy(implementation);
            Register(registration, lifestyle);
        }

        public void RegisterAsFactory<TService>(ServiceLifestyle lifestyle)
            where TService : class
        {
            var registration = Component.For<TService>().AsFactory();
            Register(registration, lifestyle);
        }

        public void RegisterClassesInAssemblyContaining<TService, TLocator>(ServiceLifestyle lifestyle)
            where TService : class where TLocator : class
        {
            var descriptor = Classes.FromAssemblyContaining<TLocator>().BasedOn<TService>().WithServiceFromInterface();
            Register(descriptor, lifestyle);
        }

        public void RegisterUsingFactoryMethod<TService, TImplementation>(
            ServiceLifestyle lifestyle,
            Func<TService> factoryMethod)
            where TService : class where TImplementation : TService
        {
            var registration = Component.For<TService>().ImplementedBy<TImplementation>()
                .UsingFactoryMethod(factoryMethod);
            Register(registration, lifestyle);
        }

        public T Resolve<T>()
        {
            return innerContainer.Resolve<T>();
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return innerContainer.ResolveAll<T>();
        }

        private void Register<TService>(
            ComponentRegistration<TService> componentRegistration,
            ServiceLifestyle lifestyle)
            where TService : class
        {
            switch (lifestyle)
            {
                case ServiceLifestyle.Transient:
                    componentRegistration = componentRegistration.LifestyleTransient();
                    break;
                case ServiceLifestyle.PerThread:
                    componentRegistration = componentRegistration.LifestylePerThread();
                    break;
                case ServiceLifestyle.PerWebRequest:
                    componentRegistration = componentRegistration.LifestylePerWebRequest();
                    break;
                case ServiceLifestyle.Singleton:
                    componentRegistration = componentRegistration.LifestyleSingleton();
                    break;
            }

            innerContainer.Register(componentRegistration);
        }

        private void Register(BasedOnDescriptor basedOnDescriptor, ServiceLifestyle lifestyle)
        {
            switch (lifestyle)
            {
                case ServiceLifestyle.Transient:
                    basedOnDescriptor = basedOnDescriptor.LifestyleTransient();
                    break;
                case ServiceLifestyle.PerThread:
                    basedOnDescriptor = basedOnDescriptor.LifestylePerThread();
                    break;
                case ServiceLifestyle.PerWebRequest:
                    basedOnDescriptor = basedOnDescriptor.LifestylePerWebRequest();
                    break;
                case ServiceLifestyle.Singleton:
                    basedOnDescriptor = basedOnDescriptor.LifestyleSingleton();
                    break;
            }

            innerContainer.Register(basedOnDescriptor);
        }
    }
}