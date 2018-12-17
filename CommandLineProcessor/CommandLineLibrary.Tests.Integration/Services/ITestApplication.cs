namespace CommandLineLibrary.Tests.Integration.Services
{
    using CommandLineLibrary.Contracts;

    public interface ITestApplication : IApplication
    {
        IApplication Mock { get; }
    }
}