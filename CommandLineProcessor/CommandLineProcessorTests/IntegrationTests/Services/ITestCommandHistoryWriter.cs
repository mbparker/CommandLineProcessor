namespace CommandLineProcessorTests.IntegrationTests.Services
{
    using System.Collections.Generic;

    using CommandLineProcessorContracts;

    public interface ITestCommandHistoryWriter : ICommandHistoryWriter
    {
        List<string> Entries { get; }
    }
}