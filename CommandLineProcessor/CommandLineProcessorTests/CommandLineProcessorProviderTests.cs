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
            var firstCommand = validCommandCollection.First();
            var firstCommandFirstSelector = firstCommand.Selectors.First();
            commandRepositoryMock[firstCommandFirstSelector].Returns(firstCommand);
            commandPathCalculatorMock.CalculateFullyQualifiedPath(null, firstCommandFirstSelector)
                .Returns(firstCommandFirstSelector);

            systemUnderTest.ProcessInput(firstCommandFirstSelector);

            Assert.That(systemUnderTest.ActiveCommand, Is.SameAs(firstCommand));
        }

        [Test]
        public void ProcessInput_WhenMatchingSubCommand_SetsActiveCommand()
        {
            systemUnderTest.RegisterCommands(validCommandCollection);
            var firstCommand = validCommandCollection.First();
            var firstCommandFirstSelector = firstCommand.Selectors.First();
            commandRepositoryMock[firstCommandFirstSelector].Returns(firstCommand);
            commandPathCalculatorMock.CalculateFullyQualifiedPath(null, firstCommandFirstSelector)
                .Returns(firstCommandFirstSelector);
            var firstSubCommand = firstCommand.Children.First();
            var firstSubCommandFirstSelector = firstSubCommand.Selectors.First();
            commandRepositoryMock[firstCommandFirstSelector + "|" + firstSubCommandFirstSelector]
                .Returns(firstSubCommand);
            commandPathCalculatorMock.CalculateFullyQualifiedPath(firstCommand, firstSubCommandFirstSelector)
                .Returns(firstCommandFirstSelector + "|" + firstSubCommandFirstSelector);

            systemUnderTest.ProcessInput(firstCommandFirstSelector);
            systemUnderTest.ProcessInput(firstSubCommandFirstSelector);

            Assert.That(systemUnderTest.ActiveCommand, Is.SameAs(firstSubCommand));
        }

        [Test]
        public void ProcessInput_WhenWaitingForCommand_TriesToGetCommand()
        {
            systemUnderTest.RegisterCommands(validCommandCollection);
            var firstCommand = validCommandCollection.First();
            var firstCommandFirstSelector = firstCommand.Selectors.First();
            commandPathCalculatorMock.CalculateFullyQualifiedPath(null, firstCommandFirstSelector)
                .Returns(firstCommandFirstSelector);

            systemUnderTest.ProcessInput(firstCommandFirstSelector);

            var dummy = commandRepositoryMock.Received(1)[firstCommandFirstSelector];
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
    }
}