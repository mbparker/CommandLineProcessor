namespace CommandLineProcessorTests
{
    using System;
    using System.Collections.Generic;

    using CommandLineProcessorContracts;

    using CommandLineProcessorLib;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class CommandLineProcessorProviderTests
    {
        private ICommandRepositoryService commandRepositoryMock;

        private CommandLineProcessorProvider systemUnderTest;

        [Test]
        public void ProcessInput_WhenInvoked_SetsCurrentInputProperty()
        {
            systemUnderTest.ProcessInput("command text");

            Assert.That(systemUnderTest.CurrentInput, Is.EqualTo("command text"));
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
        public void RegisterCommands_WhenInvoked_InvokesLoadOnRepository()
        {
            var commands = new List<ICommand>();
            systemUnderTest.RegisterCommands(commands);
            commandRepositoryMock.Received(1).Load(commands);
        }

        [SetUp]
        public void SetUp()
        {
            commandRepositoryMock = Substitute.For<ICommandRepositoryService>();
            systemUnderTest = new CommandLineProcessorProvider(commandRepositoryMock);
        }
    }
}