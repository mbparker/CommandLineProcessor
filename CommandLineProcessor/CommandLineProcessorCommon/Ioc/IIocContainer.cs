namespace CommandLineProcessorCommon.Ioc
{
    using System;
    using System.Collections.Generic;

    public interface IIocContainer
    {
        void Register<TService>(ServiceLifestyle lifestyle)
            where TService : class;

        void Register<TService, TImplementation>(ServiceLifestyle lifestyle)
            where TService : class where TImplementation : TService;

        void Register(Type service, Type implementation, ServiceLifestyle lifestyle);

        void Register<TService1, TService2, TImplementation>(ServiceLifestyle lifestyle)
            where TService1 : class where TService2 : class where TImplementation : TService1, TService2;

        void Register<TService1, TService2, TService3, TImplementation>(ServiceLifestyle lifestyle)
            where TService1 : class
            where TService2 : class
            where TService3 : class
            where TImplementation : TService1, TService2, TService3;

        void RegisterAsFactory<TService>(ServiceLifestyle lifestyle)
            where TService : class;

        void RegisterClassesInAssemblyContaining<TService, TLocator>(ServiceLifestyle lifestyle)
            where TService : class where TLocator : class;

        void RegisterUsingFactoryMethod<TService, TImplementation>(
            ServiceLifestyle lifestyle,
            Func<TService> factoryMethod)
            where TService : class where TImplementation : TService;

        T Resolve<T>();

        IEnumerable<T> ResolveAll<T>();
    }
}