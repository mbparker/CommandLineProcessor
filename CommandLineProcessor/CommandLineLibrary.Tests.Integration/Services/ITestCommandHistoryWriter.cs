namespace CommandLineLibrary.Tests.Integration.Services
{
    using CommandLineLibrary.Contracts;

    public interface ITestCommandHistoryWriter : ICommandHistoryWriter
    {
        ICommandHistoryWriter Mock { get; }
    }
}