namespace CommandLineProcessorTests
{
    using System;
    using System.Collections.Generic;

    using CommandLineProcessorContracts;

    using CommandLineProcessorLib;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class CommandRepositoryProviderTests
    {
        private List<ICommand> mockCommandList;

        private CommandRepositoryProvider systemUnderTest;

        [Test]
        public void Load_WhenInvoked_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => { systemUnderTest.Load(mockCommandList); });
        }

        [Test]
        public void Load_WhenInvokedWithDuplicateSelectors_Throws()
        {
            Assert.That(
                () => { systemUnderTest.Load(CreateCommandCollectionWithDuplicateSelectors()); },
                Throws.InstanceOf<DuplicateCommandSelectorException>().With.Message
                    .EqualTo($"Command Selector values must be unique."));
        }

        [Test]
        public void Load_WhenInvokedWithEmpty_Throws()
        {
            Assert.That(
                () => { systemUnderTest.Load(new List<ICommand>()); },
                Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(
                    $"Collection cannot be empty.{Environment.NewLine}Parameter name: commands"));
        }

        [Test]
        public void Load_WhenInvokedWithNull_Throws()
        {
            Assert.That(
                () => { systemUnderTest.Load(null); },
                Throws.InstanceOf<ArgumentNullException>().With.Message.EqualTo(
                    $"Value cannot be null.{Environment.NewLine}Parameter name: commands"));
        }

        [SetUp]
        public void SetUp()
        {
            systemUnderTest = new CommandRepositoryProvider();
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