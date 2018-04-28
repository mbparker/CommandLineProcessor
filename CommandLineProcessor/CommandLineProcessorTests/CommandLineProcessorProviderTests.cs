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
        private List<ICommand> mockCommandList;

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
        public void RegisterCommands_WhenInvoked_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => { systemUnderTest.RegisterCommands(mockCommandList); });
        }

        [Test]
        public void RegisterCommands_WhenInvokedWithDuplicateSelectors_Throws()
        {
            Assert.That(
                () => { systemUnderTest.RegisterCommands(CreateCommandCollectionWithDuplicateSelectors()); },
                Throws.InstanceOf<DuplicateCommandSelectorException>().With.Message.EqualTo(
                    $"Command Selector values must be unique."));
        }

        [Test]
        public void RegisterCommands_WhenInvokedWithEmpty_Throws()
        {
            Assert.That(
                () => { systemUnderTest.RegisterCommands(new List<ICommand>()); },
                Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(
                    $"Collection cannot be empty.{Environment.NewLine}Parameter name: commands"));
        }

        [Test]
        public void RegisterCommands_WhenInvokedWithNull_Throws()
        {
            Assert.That(
                () => { systemUnderTest.RegisterCommands(null); },
                Throws.InstanceOf<ArgumentNullException>().With.Message.EqualTo(
                    $"Value cannot be null.{Environment.NewLine}Parameter name: commands"));
        }

        [SetUp]
        public void SetUp()
        {
            systemUnderTest = new CommandLineProcessorProvider();
            mockCommandList = new List<ICommand>(CreateValidCommandCollection());
        }

        private IEnumerable<ICommand> CreateCommandCollectionWithDuplicateSelectors()
        {
            var result = new List<ICommand>();
            var command = Substitute.For<ICommand>();
            command.Selectors.Returns(new[] { "Test", "T" });
            command.Parent.Returns((ICommand)null);
            result.Add(command);
            command = Substitute.For<ICommand>();
            command.Selectors.Returns(new[] { "Test2", "T" });
            command.Parent.Returns((ICommand)null);
            result.Add(command);
            return result;
        }

        private IEnumerable<ICommand> CreateValidCommandCollection()
        {
            var result = new List<ICommand>();
            var command = Substitute.For<ICommand>();
            command.Selectors.Returns(new[] { "Test", "T" });
            command.Parent.Returns((ICommand)null);
            result.Add(command);
            command = Substitute.For<ICommand>();
            command.Selectors.Returns(new[] { "TEst2", "T2" });
            command.Parent.Returns((ICommand)null);
            result.Add(command);
            return result;
        }
    }
}