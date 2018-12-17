namespace CommandLineLibrary.Tests.Integration.Services
{
    using CommandLineLibrary.Contracts;

    using NSubstitute;

    public class TestCommandHistoryWriter : ITestCommandHistoryWriter
    {
        public TestCommandHistoryWriter()
        {
            Mock = Substitute.For<ICommandHistoryWriter>();
        }

        public ICommandHistoryWriter Mock { get; }

        public void Write(string text)
        {
            Mock.Write(text);
        }

        public void WriteLine(string text)
        {
            Mock.WriteLine(text);
        }
    }
}