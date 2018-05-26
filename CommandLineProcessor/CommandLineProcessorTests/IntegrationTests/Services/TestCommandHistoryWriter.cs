namespace CommandLineProcessorTests.IntegrationTests.Services
{
    using CommandLineProcessorContracts;

    using NSubstitute;

    public class TestCommandHistoryWriter : ITestCommandHistoryWriter
    {
        public TestCommandHistoryWriter()
        {
            Mock = Substitute.For<ICommandHistoryWriter>();
        }

        public ICommandHistoryWriter Mock { get; }

        public void WriteLine(string text)
        {
            Mock.WriteLine(text);
        }
    }
}