namespace CommandLineProcessorTests.IntegrationTests
{
    using CommandLineProcessorCommon.Ioc;

    using CommandLineProcessorContracts;

    using global::CommandLineProcessorTests.IntegrationTests.Services;

    public static class TestContainerRegistration
    {
        public static void RegisterServices(IIocContainer container)
        {
            container.Register<ICommandHistoryWriter, ITestCommandHistoryWriter, TestCommandHistoryWriter>(
                ServiceLifestyle.Singleton);
            container.Register<IApplication, ITestApplication, TestApplication>(ServiceLifestyle.Singleton);
        }
    }
}