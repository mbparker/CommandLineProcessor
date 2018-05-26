namespace CommandLineProcessorTests.IntegrationTests
{
    using System.Collections.Generic;

    using CommandLineProcessorCommon.Ioc.Windsor;

    using CommandLineProcessorContracts;
    using CommandLineProcessorContracts.Commands;
    using CommandLineProcessorContracts.Commands.Registration;

    using CommandLineProcessorDemo;
    using CommandLineProcessorDemo.DemoCommands;

    using global::CommandLineProcessorTests.IntegrationTests.Services;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class CommandLineProcessorTests
    {
        private ITestApplication applicationMock;

        private ITestCommandHistoryWriter commandHistoryWriterMock;

        private IEnumerable<ICommand> commands;

        private ICommandLineProcessorService processor;

        [OneTimeSetUp]
        public void GlobalSetUp()
        {
            IocContainerHolder.CreateContainer();
            TestContainerRegistration.RegisterServices(IocContainerHolder.Container);
            ContainerRegistration.RegisterServices(IocContainerHolder.Container);
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            IocContainerHolder.DisposeContainer();
        }

        [TestCase("Exit", "Y")]
        [TestCase("Exit", "Yes")]
        [TestCase("exit", "y")]
        [TestCase("exit", " yessir ")]
        [TestCase(" EXIT ", "Y")]
        public void ProcessInput_WhenExitInvokedWithConfirm_InvokesExitOnApplication(string input1, string input2)
        {
            RegisterCommands();

            processor.ProcessInput(input1);
            processor.ProcessInput(input2);

            applicationMock.Mock.Received(1).Exit();
        }

        [TestCase("Exit", "N")]
        [TestCase("Exit", "No")]
        [TestCase("exit", "n")]
        [TestCase("exit", " nope ")]
        [TestCase(" EXIT ", "N")]
        public void ProcessInput_WhenExitInvokedWithNoConfirm_DoesNotInvokeExitOnApplication(
            string input1,
            string input2)
        {
            RegisterCommands();

            processor.ProcessInput(input1);
            processor.ProcessInput(input2);

            applicationMock.Mock.DidNotReceive().Exit();
        }

        [SetUp]
        public void SetUp()
        {
            GetServices();
            ResetMocks();
        }

        private void GetServices()
        {
            processor = IocContainerHolder.Container.Resolve<ICommandLineProcessorService>();
            applicationMock = IocContainerHolder.Container.Resolve<ITestApplication>();
            commandHistoryWriterMock = IocContainerHolder.Container.Resolve<ITestCommandHistoryWriter>();
        }

        private void RegisterCommands()
        {
            var rootRegistration = IocContainerHolder.Container.Resolve<IRootCommandRegistration>();
            rootRegistration.Clear();
            commands = CommandRegistration.Register(rootRegistration);
            processor.RegisterCommands(commands);
        }

        private void ResetMocks()
        {
            applicationMock.Mock.ClearReceivedCalls();
            commandHistoryWriterMock.Mock.ClearReceivedCalls();
        }
    }
}