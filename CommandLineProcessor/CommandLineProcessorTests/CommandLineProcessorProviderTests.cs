namespace CommandLineProcessorTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommandLineProcessorContracts;
    using CommandLineProcessorContracts.Events;

    using CommandLineProcessorLib;

    using CommandLineProcessorTests.TestDataGenerators;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class CommandLineProcessorProviderTests
    {
        private ICommandContextFactory commandContextFactoryMock;

        private ICommandContext commandContextMock;

        private ICommandHistoryService commandHistoryServiceMock;

        private ICommandPathCalculator commandPathCalculatorMock;

        private ICommandRepositoryService commandRepositoryMock;

        private CommandLineProcessorProvider systemUnderTest;

        private List<ICommand> validCommandCollection;

        [Test]
        public void ProcessInput_WhenCancel_MakesParentInputOrContainerActive()
        {
            systemUnderTest.RegisterCommands(validCommandCollection);

            var rootCommand = validCommandCollection.Single(x => x.PrimarySelector.ToUpper() == "TEST3");
            var command = rootCommand;
            SetUpCommand(command);
            command = (command as IContainerCommand).Children.First(x => x.PrimarySelector.ToUpper() == "SUBINPUT");
            SetUpCommand(command);
            command = (command as IInputCommand).NextCommand;

            systemUnderTest.ProcessInput("test3");
            systemUnderTest.ProcessInput("subinput");
            systemUnderTest.ProcessInput("^c");

            (command as IExecutableCommand).DidNotReceiveWithAnyArgs().Execute(
                Arg.Any<ICommandContext>(),
                Arg.Any<object[]>());
            Assert.AreSame(systemUnderTest.ActiveCommand, rootCommand);
            Assert.That(systemUnderTest.Status, Is.EqualTo(CommandLineStatus.WaitingForCommand));
        }

        [Test]
        public void ProcessInput_WhenCommandExecutes_NotifiesHistoryService()
        {
            systemUnderTest.RegisterCommands(validCommandCollection);
            var command = SetUpCommand();

            systemUnderTest.ProcessInput(command.PrimarySelector);

            commandHistoryServiceMock.Received(1).NotifyCommandExecuting(command);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void ProcessInput_WhenInvalid_RaisesErrorEvent(string input)
        {
            Exception ex = null;
            systemUnderTest.ProcessInputError += (sender, args) => { ex = args.Exception; };

            systemUnderTest.ProcessInput(input);

            Assert.That(ex, Is.Not.Null);
            Assert.That(
                ex,
                Is.InstanceOf<ArgumentException>().With.Message
                    .EqualTo($"Value is required.{Environment.NewLine}Parameter name: input"));
        }

        [Test]
        public void ProcessInput_WhenInvoked_SetsLastInputProperty()
        {
            systemUnderTest.ProcessInput("command text");

            Assert.That(systemUnderTest.LastInput, Is.EqualTo("command text"));
        }

        [Test]
        public void ProcessInput_WhenMacroWithPause_PausesForUserInput()
        {
            systemUnderTest.RegisterCommands(validCommandCollection);

            var rootCommand = validCommandCollection.Single(x => x.PrimarySelector.ToUpper() == "TEST3");
            var command = rootCommand;
            SetUpCommand(command);
            var inputCommand =
                (rootCommand as IContainerCommand).Children.First(x => x.PrimarySelector.ToUpper() == "SUBINPUT");
            command = inputCommand;
            SetUpCommand(command);
            var exeCommand =
                (rootCommand as IContainerCommand).Children.First(x => x.PrimarySelector.ToUpper() == "SUB");
            SetUpCommand(exeCommand);
            command = (command as IInputCommand).NextCommand;

            systemUnderTest.ProcessInput("test3||subinput||~||test3||sub");

            (command as IExecutableCommand).DidNotReceiveWithAnyArgs().Execute(
                Arg.Any<ICommandContext>(),
                Arg.Any<object[]>());
            Assert.AreSame(systemUnderTest.ActiveCommand, inputCommand);
            Assert.That(systemUnderTest.Status, Is.EqualTo(CommandLineStatus.WaitingForInput));

            systemUnderTest.ProcessInput("Hello World");

            (command as IExecutableCommand).Received(1).Execute(Arg.Any<ICommandContext>(), Arg.Any<object[]>());
            (exeCommand as IExecutableCommand).Received(1).Execute(Arg.Any<ICommandContext>(), Arg.Any<object[]>());
            Assert.IsNull(systemUnderTest.ActiveCommand);
            Assert.That(systemUnderTest.Status, Is.EqualTo(CommandLineStatus.WaitingForCommand));
        }

        [Test]
        public void ProcessInput_WhenMacroWithPauseAndTransparentCommandWithPause_PausesForUserInput()
        {
            systemUnderTest.RegisterCommands(validCommandCollection);

            var rootCommand = validCommandCollection.Single(x => x.PrimarySelector.ToUpper() == "TEST3");
            var command = rootCommand;
            SetUpCommand(command);
            var inputCommand =
                (rootCommand as IContainerCommand).Children.First(x => x.PrimarySelector.ToUpper() == "SUBINPUT");
            command = inputCommand;
            SetUpCommand(command);
            var exeCommand =
                (rootCommand as IContainerCommand).Children.First(x => x.PrimarySelector.ToUpper() == "SUB");
            SetUpCommand(exeCommand);
            command = (command as IInputCommand).NextCommand;

            systemUnderTest.ProcessInput("test3||`test3||~||subinput||~||test3||sub");

            Assert.AreSame(systemUnderTest.ActiveCommand, rootCommand);
            Assert.That(systemUnderTest.Status, Is.EqualTo(CommandLineStatus.WaitingForCommand));

            systemUnderTest.ProcessInput("sub");

            (command as IExecutableCommand).DidNotReceiveWithAnyArgs().Execute(
                Arg.Any<ICommandContext>(),
                Arg.Any<object[]>());
            Assert.AreSame(systemUnderTest.ActiveCommand, inputCommand);
            Assert.That(systemUnderTest.Status, Is.EqualTo(CommandLineStatus.WaitingForInput));

            systemUnderTest.ProcessInput("Hello World");

            (command as IExecutableCommand).Received(1).Execute(Arg.Any<ICommandContext>(), Arg.Any<object[]>());
            (exeCommand as IExecutableCommand).Received(2).Execute(Arg.Any<ICommandContext>(), Arg.Any<object[]>());
            Assert.IsNull(systemUnderTest.ActiveCommand);
            Assert.That(systemUnderTest.Status, Is.EqualTo(CommandLineStatus.WaitingForCommand));
        }

        [Test]
        public void ProcessInput_WhenMatchingCommand_SetsActiveCommand()
        {
            systemUnderTest.RegisterCommands(validCommandCollection);
            var command = SetUpCommand();

            systemUnderTest.ProcessInput(command.PrimarySelector);

            Assert.That(systemUnderTest.ActiveCommand, Is.SameAs(command));
        }

        [Test]
        public void ProcessInput_WhenMatchingInputSubCommand_InvokexExecuteOnInputsChildAndUpdatesStatus()
        {
            var expectedStatusChangedEvents = new List<CommandLineStatusChangedEventArgs>();
            expectedStatusChangedEvents.Add(
                new CommandLineStatusChangedEventArgs(
                    CommandLineStatus.WaitingForCommandRegistration,
                    CommandLineStatus.WaitingForCommand));
            expectedStatusChangedEvents.Add(
                new CommandLineStatusChangedEventArgs(
                    CommandLineStatus.WaitingForCommand,
                    CommandLineStatus.WaitingForInput));
            expectedStatusChangedEvents.Add(
                new CommandLineStatusChangedEventArgs(
                    CommandLineStatus.WaitingForInput,
                    CommandLineStatus.WaitingForCommand));

            var actualStatusChangedEvents = new List<CommandLineStatusChangedEventArgs>();
            systemUnderTest.StatusChanged += (sender, args) => { actualStatusChangedEvents.Add(args); };

            systemUnderTest.RegisterCommands(validCommandCollection);

            ICommand command = validCommandCollection.Single(x => x.PrimarySelector.ToUpper() == "TEST3");
            SetUpCommand(command);
            command = (command as IContainerCommand).Children.First(x => x.PrimarySelector.ToUpper() == "SUBINPUT");
            SetUpCommand(command);
            command = (command as IInputCommand).NextCommand;

            systemUnderTest.ProcessInput("test3");
            systemUnderTest.ProcessInput("subinput");

            Assert.That(systemUnderTest.Status, Is.EqualTo(CommandLineStatus.WaitingForInput));

            systemUnderTest.ProcessInput("Hello World");

            (command as IExecutableCommand).Received(1).Execute(Arg.Any<ICommandContext>(), Arg.Any<object[]>());
            Assert.IsNull(systemUnderTest.ActiveCommand);
            Assert.That(systemUnderTest.Status, Is.EqualTo(CommandLineStatus.WaitingForCommand));

            Assert.That(actualStatusChangedEvents.Count, Is.EqualTo(expectedStatusChangedEvents.Count));
            for (int i = 0; i < expectedStatusChangedEvents.Count; i++)
            {
                Assert.That(
                    actualStatusChangedEvents[i].PriorStatus,
                    Is.EqualTo(expectedStatusChangedEvents[i].PriorStatus));
                Assert.That(actualStatusChangedEvents[i].Status, Is.EqualTo(expectedStatusChangedEvents[i].Status));
            }
        }

        [Test]
        public void ProcessInput_WhenMultipleCancel_GoesBackToNoActiveCommand()
        {
            systemUnderTest.RegisterCommands(validCommandCollection);

            var rootCommand = validCommandCollection.Single(x => x.PrimarySelector.ToUpper() == "TEST3");
            var command = rootCommand;
            SetUpCommand(command);
            command = (command as IContainerCommand).Children.First(x => x.PrimarySelector.ToUpper() == "SUBINPUT");
            SetUpCommand(command);
            command = (command as IInputCommand).NextCommand;

            systemUnderTest.ProcessInput("test3");
            systemUnderTest.ProcessInput("subinput");
            systemUnderTest.ProcessInput("^c||^c||^c");

            (command as IExecutableCommand).DidNotReceiveWithAnyArgs().Execute(
                Arg.Any<ICommandContext>(),
                Arg.Any<object[]>());
            Assert.IsNull(systemUnderTest.ActiveCommand);
            Assert.That(systemUnderTest.Status, Is.EqualTo(CommandLineStatus.WaitingForCommand));
        }

        [Test]
        public void ProcessInput_WhenTransparentCommand_SuspendsAndResumeActiveCommandAfterExecutingTransparentCommand()
        {
            systemUnderTest.RegisterCommands(validCommandCollection);

            var rootCommand = validCommandCollection.Single(x => x.PrimarySelector.ToUpper() == "TEST3");
            var command = rootCommand;
            SetUpCommand(command);
            var inputCommand =
                (command as IContainerCommand).Children.First(x => x.PrimarySelector.ToUpper() == "SUBINPUT");
            command = inputCommand;
            SetUpCommand(command);
            command = (command as IInputCommand).NextCommand;

            systemUnderTest.ProcessInput("test3");
            systemUnderTest.ProcessInput("subinput");
            systemUnderTest.ProcessInput("`test3||subinput||hello world");

            (command as IExecutableCommand).Received(1).Execute(Arg.Any<ICommandContext>(), Arg.Any<object[]>());
            Assert.AreSame(systemUnderTest.ActiveCommand, inputCommand);
            Assert.That(systemUnderTest.Status, Is.EqualTo(CommandLineStatus.WaitingForInput));

            systemUnderTest.ProcessInput("Hello World");

            (command as IExecutableCommand).Received(2).Execute(Arg.Any<ICommandContext>(), Arg.Any<object[]>());
            Assert.IsNull(systemUnderTest.ActiveCommand);
            Assert.That(systemUnderTest.Status, Is.EqualTo(CommandLineStatus.WaitingForCommand));
        }

        [Test]
        public void ProcessInput_WhenWaitingForCommand_TriesToGetCommand()
        {
            systemUnderTest.RegisterCommands(validCommandCollection);
            var commandFirstSelector = SetUpCommand().PrimarySelector;

            systemUnderTest.ProcessInput(commandFirstSelector);

            var dummy = commandRepositoryMock.Received(1)[commandFirstSelector];
        }

        [Test]
        public void ProcessInput_WhenWaitingForCommandRegistration_DoesNotTryToGetCommand()
        {
            systemUnderTest.ProcessInput("command text");

            var dummy = commandRepositoryMock.DidNotReceive()[Arg.Any<string>()];
        }

        [Test]
        public void RegisterCommands_WhenInvoked_InvokesLoadOnRepository()
        {
            systemUnderTest.RegisterCommands(validCommandCollection);
            commandRepositoryMock.Received(1).Load(validCommandCollection);
        }

        [Test]
        public void RegisterCommands_WhenInvokedWithInvalidData_DoesNotThrow()
        {
            Assert.DoesNotThrow(
                () =>
                    {
                        systemUnderTest.RegisterCommands(
                            CommandGenerator.GenerateCommandCollectionWithDuplicateSelectors());
                    });
        }

        [Test]
        public void RegisterCommands_WhenInvokedWithInvalidData_FiresErrorEventResetsStatus()
        {
            var invalidCommands = CommandGenerator.GenerateCommandCollectionWithDuplicateSelectors();
            var sourceError = new Exception("Test Exception");
            Exception actualError = null;
            systemUnderTest.CommandRegistrationError += (sender, args) => { actualError = args.Exception; };
            commandRepositoryMock.When(x => x.Load(invalidCommands)).Do(x => throw sourceError);

            systemUnderTest.RegisterCommands(invalidCommands);

            Assert.AreSame(actualError, sourceError);
            Assert.That(systemUnderTest.Status, Is.EqualTo(CommandLineStatus.WaitingForCommandRegistration));
        }

        [SetUp]
        public void SetUp()
        {
            validCommandCollection = CommandGenerator.GenerateValidCommandCollection().ToList();
            commandRepositoryMock = Substitute.For<ICommandRepositoryService>();
            commandPathCalculatorMock = Substitute.For<ICommandPathCalculator>();
            commandContextMock = Substitute.For<ICommandContext>();
            commandContextFactoryMock = Substitute.For<ICommandContextFactory>();
            commandContextFactoryMock.Create().Returns(commandContextMock);
            commandHistoryServiceMock = Substitute.For<ICommandHistoryService>();
            systemUnderTest = new CommandLineProcessorProvider(
                commandRepositoryMock,
                commandPathCalculatorMock,
                commandContextFactoryMock,
                commandHistoryServiceMock);
        }

        [Test]
        public void Status_AfterConstruction_IsWaitingForCommandRegistration()
        {
            Assert.That(systemUnderTest.Status, Is.EqualTo(CommandLineStatus.WaitingForCommandRegistration));
        }

        [Test]
        public void Status_AfterRegistration_IsWaitingForCommand()
        {
            systemUnderTest.RegisterCommands(validCommandCollection);

            Assert.That(systemUnderTest.Status, Is.EqualTo(CommandLineStatus.WaitingForCommand));
        }

        private ICommand SetUpCommand()
        {
            var command = validCommandCollection.First();
            var commandFirstSelector = command.PrimarySelector;
            commandRepositoryMock[commandFirstSelector].Returns(command);
            commandPathCalculatorMock.CalculateFullyQualifiedPath(null, commandFirstSelector)
                .Returns(commandFirstSelector);
            return command;
        }

        private void SetUpCommand(ICommand command)
        {
            var commandFirstSelector = command.PrimarySelector.ToLower();
            commandRepositoryMock[commandFirstSelector].Returns(command);
            commandPathCalculatorMock.CalculateFullyQualifiedPath(command.Parent, commandFirstSelector)
                .Returns(commandFirstSelector);
        }
    }
}