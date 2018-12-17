namespace CommandLineLibrary.Tests.Integration.Services
{
    using CommandLineLibrary.Contracts;

    using NSubstitute;

    public class TestApplication : ITestApplication
    {
        public TestApplication()
        {
            Mock = Substitute.For<IApplication>();
        }

        public IApplication Mock { get; }

        public void Exit()
        {
            Mock.Exit();
        }
    }
}