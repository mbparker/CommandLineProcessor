﻿namespace CommandLineProcessorTests.IntegrationTests.Services
{
    using CommandLineProcessorContracts;

    public interface ITestCommandHistoryWriter : ICommandHistoryWriter
    {
        ICommandHistoryWriter Mock { get; }
    }
}