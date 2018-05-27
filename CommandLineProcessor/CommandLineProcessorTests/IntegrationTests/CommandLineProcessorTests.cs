﻿namespace CommandLineProcessorTests.IntegrationTests
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

        [TestCase("Echo|Hello World", "Hello World")]
        [TestCase("Echo| Hello World ", "Hello World")]
        [TestCase("Echo| Hello World ", "Hello World")]
        [TestCase("E| Hello  World ", "Hello  World")]
        public void ProcessInput_WhenEchoInvoked_WritesToHistory(string input, string expectedOutput)
        {
            RegisterCommands();

            processor.ProcessInput(input);

            commandHistoryWriterMock.Mock.Received(1).WriteLine($"You entered: {expectedOutput}");
        }

        [TestCase("echo|^C")]
        [TestCase("E| ^C ")]
        public void ProcessInput_WhenEchoInvokedAndCancel_DoesNotWriteToHistory(string input)
        {
            RegisterCommands();

            processor.ProcessInput(input);

            commandHistoryWriterMock.Mock.DidNotReceive().WriteLine(Arg.Any<string>());
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

        [TestCase("Math|Add|3.25|4.75", "3.25 + 4.75 = 8")]
        [TestCase("MATH|add|3|4", "3 + 4 = 7")]
        [TestCase("math|ADD|0.25 | 7", "0.25 + 7 = 7.25")]
        [TestCase("math|a|-1 | 10", "-1 + 10 = 9")]
        public void ProcessInput_WhenMathAddInvoked_WritesToHistory(string input, string expectedOutput)
        {
            RegisterCommands();

            processor.ProcessInput(input);

            commandHistoryWriterMock.Mock.Received(1).WriteLine(expectedOutput);
        }

        [TestCase("Math|Mult|3.25|4.75", "3.25 X 4.75 = 15.4375")]
        [TestCase("MATH|mult|3|4", "3 X 4 = 12")]
        [TestCase("MATH|M|3|4", "3 X 4 = 12")]
        [TestCase("math|MULT|0.25 | 7", "0.25 X 7 = 1.75")]
        public void ProcessInput_WhenMathMultInvoked_WritesToHistory(string input, string expectedOutput)
        {
            RegisterCommands();

            processor.ProcessInput(input);

            commandHistoryWriterMock.Mock.Received(1).WriteLine(expectedOutput);
        }

        [TestCase("Math|Mult|3.25|^C")]
        [TestCase("Math|Mult|^C")]
        [TestCase("Math|^C")]
        public void ProcessInput_WhenMathMultInvokedAndCancel_DoesNotWriteToHistory(string input)
        {
            RegisterCommands();

            processor.ProcessInput(input);

            commandHistoryWriterMock.Mock.DidNotReceive().WriteLine(Arg.Any<string>());
        }

        [TestCase("Math|Mult|3.25|`math|add|2|3|1.75")]
        public void ProcessInput_WhenOverlappingCommands_AllCommandsComplete(string input)
        {
            RegisterCommands();

            processor.ProcessInput(input);

            commandHistoryWriterMock.Mock.Received(1).WriteLine("2 + 3 = 5");
            commandHistoryWriterMock.Mock.Received(1).WriteLine("3.25 X 1.75 = 5.6875");
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