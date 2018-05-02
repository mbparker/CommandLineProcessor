namespace CommandLineProcessorTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommandLineProcessorContracts;

    using CommandLineProcessorLib;

    using CommandLineProcessorTests.TestDataGenerators;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class CommandLineProcessorProviderTests
    {
        private ICommandPathCalculator commandPathCalculatorMock;

        private ICommandRepositoryService commandRepositoryMock;

        private CommandLineProcessorProvider systemUnderTest;

        private List<ICommand> validCommandCollection;

        [Test]
        public void ProcessInput_WhenInvoked_SetsLastInputProperty()
        {
            systemUnderTest.ProcessInput("command text");

            Assert.That(systemUnderTest.LastInput, Is.EqualTo("command text"));
        }

        [Test]
        public void ProcessInput_WhenInvokedWithEmpty_Throws()
        {
            Assert.That(
                () => { systemUnderTest.ProcessInput(string.Empty); },
                Throws.InstanceOf<ArgumentException>().With.Message
                    .EqualTo($"Value is required.{Environment.NewLine}Parameter name: input"));
        }

        [Test]
        public void ProcessInput_WhenInvokedWithNull_Throws()
        {
            Assert.That(
                () => { systemUnderTest.ProcessInput(null); },
                Throws.InstanceOf<ArgumentException>().With.Message
                    .EqualTo($"Value is required.{Environment.NewLine}Parameter name: input"));
        }

        [Test]
        public void ProcessInput_WhenInvokedWithOnlyWhitespace_Throws()
        {
            Assert.That(
                () => { systemUnderTest.ProcessInput(" "); },
                Throws.InstanceOf<ArgumentException>().With.Message
                    .EqualTo($"Value is required.{Environment.NewLine}Parameter name: input"));
        }

        [Test]
        public void ProcessInput_WhenMatchingCommand_SetsActiveCommand()
        {
            systemUnderTest.RegisterCommands(validCommandCollection);
            var command = SetUpCommand();

            systemUnderTest.ProcessInput(command.Selectors.First());

            Assert.That(systemUnderTest.ActiveCommand, Is.SameAs(command));
        }

        [Test]
        public void ProcessInput_WhenMatchingSubCommand_SetsActiveCommand()
        {
            systemUnderTest.RegisterCommands(validCommandCollection);
            var command = SetUpCommand();
            var subCommand = SetUpSubCommand(command);

            systemUnderTest.ProcessInput(command.Selectors.First());
            systemUnderTest.ProcessInput(subCommand.Selectors.First());

            Assert.That(systemUnderTest.ActiveCommand, Is.SameAs(subCommand));
        }

        [Test]
        public void ProcessInput_WhenWaitingForCommand_TriesToGetCommand()
        {
            systemUnderTest.RegisterCommands(validCommandCollection);
            var commandFirstSelector = SetUpCommand().Selectors.First();

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

        [SetUp]
        public void SetUp()
        {
            validCommandCollection = CommandGenerator.GenerateValidCommandCollection().ToList();
            commandRepositoryMock = Substitute.For<ICommandRepositoryService>();
            commandPathCalculatorMock = Substitute.For<ICommandPathCalculator>();
            systemUnderTest = new CommandLineProcessorProvider(commandRepositoryMock, commandPathCalculatorMock);
        }

        [Test]
        public void State_AfterConstruction_IsWaitingForCommandRegistration()
        {
            Assert.That(systemUnderTest.State, Is.EqualTo(CommandLineState.WaitingForCommandRegistration));
        }

        [Test]
        public void State_AfterRegistration_IsWaitingForCommand()
        {
            systemUnderTest.RegisterCommands(validCommandCollection);

            Assert.That(systemUnderTest.State, Is.EqualTo(CommandLineState.WaitingForCommand));
        }

        private ICommand SetUpCommand()
        {
            var command = validCommandCollection.First();
            var commandFirstSelector = command.Selectors.First();
            commandRepositoryMock[commandFirstSelector].Returns(command);
            commandPathCalculatorMock.CalculateFullyQualifiedPath(null, commandFirstSelector)
                .Returns(commandFirstSelector);
            return command;
        }

        private ICommand SetUpSubCommand(ICommand command)
        {
            var commandFirstSelector = command.Selectors.First();
            var subCommand = command.Children.First();
            var subCommandFirstSelector = subCommand.Selectors.First();
            commandRepositoryMock[commandFirstSelector + "|" + subCommandFirstSelector].Returns(subCommand);
            commandPathCalculatorMock.CalculateFullyQualifiedPath(command, subCommandFirstSelector)
                .Returns(commandFirstSelector + "|" + subCommandFirstSelector);
            return subCommand;
        }
    }
}