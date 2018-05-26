namespace CommandLineProcessorTests.IntegrationTests.Services
{
    using System.Collections.Generic;

    public class TestCommandHistoryWriter : ITestCommandHistoryWriter
    {
        public TestCommandHistoryWriter()
        {
            Entries = new List<string>();
        }

        public List<string> Entries { get; }

        public void WriteLine(string text)
        {
            Entries.Add(text);
        }
    }
}