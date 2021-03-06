﻿namespace CommandLineLibrary.Tests.Integration
{
    using System;
    using System.Collections.Generic;

    using CommandLineLibrary.Contracts;
    using CommandLineLibrary.Contracts.Commands;
    using CommandLineLibrary.Contracts.Commands.Registration;
    using CommandLineLibrary.Contracts.Events;
    using CommandLineLibrary.Demo.Commands;
    using CommandLineLibrary.Tests.Integration.Services;

    using global::Autofac;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class CommandLineProcessorTests
    {
        private ITestApplication applicationMock;

        private ITestCommandHistoryWriter commandHistoryWriterMock;

        private IEnumerable<ICommand> commands;

        private IContainer container;

        private List<CommandLineErrorEventArgs> processingErrors;

        private ICommandLineProcessorService processor;

        private List<CommandLineErrorEventArgs> registrationErrors;

        [OneTimeSetUp]
        public void GlobalSetUp()
        {
            container = TestContainerRegistration.RegisterServices();
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            container.Dispose();
            container = null;
        }

        [TestCase("Echo|Hello World", "You entered: Hello World")]
        [TestCase("Echo| Hello World ", "You entered: Hello World")]
        [TestCase("Echo| Hello World ", "You entered: Hello World")]
        [TestCase("E| Hello  World ", "You entered: Hello  World")]
        public void ProcessInput_WhenEchoInvoked_WritesToHistory(string input, string expectedOutput)
        {
            RegisterCommands();

            InvokeProcessInput(input);

            AssertMultipleWriteLine(expectedOutput);
        }

        [TestCase("echo|^C")]
        [TestCase("E| ^C ")]
        public void ProcessInput_WhenEchoInvokedAndCancel_DoesNotWriteToHistory(string input)
        {
            RegisterCommands();

            InvokeProcessInput(input);

            commandHistoryWriterMock.Mock.DidNotReceive().WriteLine(Arg.Any<string>());
        }

        [TestCase("Exit;Y")]
        [TestCase("Exit;Yes")]
        [TestCase("exit;y")]
        [TestCase("exit; yessir ")]
        [TestCase(" EXIT ;Y")]
        public void ProcessInput_WhenExitInvokedWithConfirm_InvokesExitOnApplication(string input)
        {
            RegisterCommands();

            InvokeProcessInput(input);

            applicationMock.Mock.Received(1).Exit();
        }

        [TestCase("Exit;N")]
        [TestCase("Exit;No")]
        [TestCase("exit;n")]
        [TestCase("exit; nope ")]
        [TestCase(" EXIT ;N")]
        [TestCase("exit;")]
        [TestCase("exit|")]
        public void ProcessInput_WhenExitInvokedWithNoConfirm_DoesNotInvokeExitOnApplication(string input)
        {
            RegisterCommands();

            InvokeProcessInput(input);

            applicationMock.Mock.DidNotReceive().Exit();
        }

        [TestCase("Math|Add|3.25|4.75", "3.25 + 4.75 = 8")]
        [TestCase("MATH|add|3|4", "3 + 4 = 7")]
        [TestCase("math|ADD|0.25 | 7", "0.25 + 7 = 7.25")]
        [TestCase("math|a|-1 | 10", "-1 + 10 = 9")]
        public void ProcessInput_WhenMathAddInvoked_WritesToHistory(string input, string expectedOutput)
        {
            RegisterCommands();

            InvokeProcessInput(input);

            AssertMultipleWriteLine(expectedOutput);
        }

        [TestCase("Math|Mult|3.25|4.75", "3.25 X 4.75 = 15.4375")]
        [TestCase("MATH|mult|3|4", "3 X 4 = 12")]
        [TestCase("MATH|M|3|4", "3 X 4 = 12")]
        [TestCase("MATH||3|4", "3 X 4 = 12")]
        [TestCase("MATH;;3|4", "3 X 4 = 12")]
        [TestCase("math|MULT|0.25 | 7", "0.25 X 7 = 1.75")]
        public void ProcessInput_WhenMathMultInvoked_WritesToHistory(string input, string expectedOutput)
        {
            RegisterCommands();

            InvokeProcessInput(input);

            AssertMultipleWriteLine(expectedOutput);
        }

        [TestCase("Math|Mult|3.25|^C|^C|^C")]
        [TestCase("Math|Mult|^C|^C")]
        [TestCase("Math|^C")]
        [TestCase("Math;^C")]
        public void ProcessInput_WhenMathMultInvokedAndCancel_DoesNotWriteToHistory(string input)
        {
            RegisterCommands();

            InvokeProcessInput(input);

            commandHistoryWriterMock.Mock.DidNotReceive().WriteLine(Arg.Any<string>());
        }

        [TestCase("Math|Add|~|3.25;1.75", "1.75 + 3.25 = 5")]
        [TestCase("Math|Add|~|~;3.25;1.75", "3.25 + 1.75 = 5")]
        [TestCase("Math|~|~|~;a;3.25;1.75", "3.25 + 1.75 = 5")]
        public void ProcessInput_WhenMPromptForInput_MustInvokeProcessInputAgain(
            string macroInput,
            string expectedOutputs)
        {
            RegisterCommands();

            InvokeProcessInput(macroInput);

            AssertMultipleWriteLine(expectedOutputs);
        }

        [TestCase("Math|Add|`math|add|2|3|~|3.25;1.75", "2 + 3 = 5;1.75 + 3.25 = 5")]
        [TestCase("Math|Add|~|`math|add|2|3|3.25;1.75", "2 + 3 = 5;1.75 + 3.25 = 5")]
        [TestCase("Math|Add|~|3.25;`math|add|2|3;1.75", "2 + 3 = 5;1.75 + 3.25 = 5")]
        [TestCase("Math|Add|~|3.25;`math|add|~|3;2;1.75", "2 + 3 = 5;1.75 + 3.25 = 5")]
        public void ProcessInput_WhenMPromptForInputAndTranparent_MustInvokeProcessInputAgain(
            string macroInput,
            string expectedOutputs)
        {
            RegisterCommands();

            InvokeProcessInput(macroInput);

            AssertMultipleWriteLine(expectedOutputs);
        }

        [Test]
        public void ProcessInput_WhenOverlappingCommands_AllCommandsComplete()
        {
            RegisterCommands();

            InvokeProcessInput("Math|Mult|3.25|`math|add|2|3|1.75");

            AssertMultipleWriteLine("2 + 3 = 5;3.25 X 1.75 = 5.6875");
        }

        [Test]
        public void ProcessInput_WhenOverlappingCommandsAndTopCancelled_FirstCommandCompletes()
        {
            RegisterCommands();

            InvokeProcessInput("Math|Mult|3.25|`math|add|2|^C|^C|^C|1.75");

            commandHistoryWriterMock.Mock.Received(1).WriteLine(Arg.Any<string>());
            commandHistoryWriterMock.Mock.Received(1).WriteLine("3.25 X 1.75 = 5.6875");
        }

        [Test]
        public void ProcessInput_WhenUnknownCommand_FiresErrorEvent()
        {
            RegisterCommands();

            InvokeProcessInput("DoesntExist");

            Assert.That(processingErrors.Count, Is.EqualTo(1));
            Assert.That(processingErrors[0].Exception.Message, Is.EqualTo("Command 'DoesntExist' not found."));
            processingErrors.Clear();
        }

        [Test]
        public void ProcessInput_WhenCommandsNotRegistered_FiresErrorEvent()
        {
            InvokeProcessInput("Math");

            Assert.That(processingErrors.Count, Is.EqualTo(1));
            Assert.That(processingErrors[0].Exception.Message, Is.EqualTo("Processor is not ready to accept commands. Did you register your commands?"));
            processingErrors.Clear();
            RegisterCommands();
        }

        [SetUp]
        public void SetUp()
        {
            GetServices();
            ResetMocks();
        }

        [TearDown]
        public void TearDown()
        {
            UnhookEvents();
            AssertNoRegistrationErrors();
            AssertNoProcessingErrors();
            AssertProcessorActiveCommandIsNull();
            AssertProcessorStateIsWaitingForCommand();
            AssertProcessorStackDepthIsZero();
        }

        private void UnhookEvents()
        {
            processor.CommandRegistrationError -= Processor_CommandRegistrationError;
            processor.ProcessInputError -= Processor_ProcessInputError;
        }

        private void AssertMultipleWriteLine(string expectedOutputs)
        {
            var outputs = expectedOutputs.Split(new[] { ";" }, StringSplitOptions.None);
            commandHistoryWriterMock.Mock.Received(outputs.Length).WriteLine(Arg.Any<string>());
            Received.InOrder(
                () =>
                    {
                        foreach (var output in outputs)
                        {
                            commandHistoryWriterMock.Mock.WriteLine(output);
                        }
                    });
        }

        private void AssertNoProcessingErrors()
        {
            Assert.That(processingErrors, Is.Empty);
        }

        private void AssertNoRegistrationErrors()
        {
            Assert.That(registrationErrors, Is.Empty);
        }

        private void AssertProcessorActiveCommandIsNull()
        {
            Assert.That(processor.ActiveCommand, Is.Null);
        }

        private void AssertProcessorStackDepthIsZero()
        {
            Assert.That(processor.StackDepth, Is.EqualTo(0));
        }

        private void AssertProcessorStateIsWaitingForCommand()
        {
            Assert.That(processor.Status, Is.EqualTo(CommandLineStatus.WaitingForCommand));
        }

        private void GetServices()
        {
            processingErrors = new List<CommandLineErrorEventArgs>();
            registrationErrors = new List<CommandLineErrorEventArgs>();
            processor = container.Resolve<ICommandLineProcessorService>();
            processor.CommandRegistrationError += Processor_CommandRegistrationError;
            processor.ProcessInputError += Processor_ProcessInputError;
            applicationMock = container.Resolve<ITestApplication>();
            commandHistoryWriterMock = container.Resolve<ITestCommandHistoryWriter>();
        }

        private void InvokeProcessInput(string macroInput)
        {
            var inputs = macroInput.Split(new[] { ";" }, StringSplitOptions.None);
            foreach (var input in inputs)
            {
                processor.ProcessInput(input);
            }
        }

        private void Processor_CommandRegistrationError(object sender, CommandLineErrorEventArgs e)
        {
            registrationErrors.Add(e);
        }

        private void Processor_ProcessInputError(object sender, CommandLineErrorEventArgs e)
        {
            processingErrors.Add(e);
        }

        private void RegisterCommands()
        {
            var rootRegistration = container.Resolve<IRootCommandRegistration>();
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