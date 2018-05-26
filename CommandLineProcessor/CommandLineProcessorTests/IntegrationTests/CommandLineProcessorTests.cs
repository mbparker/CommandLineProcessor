namespace CommandLineProcessorTests.IntegrationTests
{
    using CommandLineProcessorCommon.Ioc.Windsor;

    using CommandLineProcessorContracts;

    using NUnit.Framework;

    [TestFixture]
    public class CommandLineProcessorTests
    {
        private ICommandLineProcessorService processor;

        [OneTimeSetUp]
        public void GlobalSetUp()
        {
            IocContainerHolder.CreateContainer();
            ContainerRegistration.RegisterServices(IocContainerHolder.Container);
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            IocContainerHolder.DisposeContainer();
        }

        [SetUp]
        public void SetUp()
        {
            processor = IocContainerHolder.Container.Resolve<ICommandLineProcessorService>();
        }

        [Test]
        public void ProcessInput_WhenInvoked_ExecutesAsExpected()
        {            
        }
    }
}