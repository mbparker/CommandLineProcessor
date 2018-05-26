namespace CommandLineProcessorTests.IntegrationTests.Services
{
    using CommandLineProcessorContracts;

    public interface ITestApplication : IApplication
    {
        IApplication Mock { get; }
    }
}