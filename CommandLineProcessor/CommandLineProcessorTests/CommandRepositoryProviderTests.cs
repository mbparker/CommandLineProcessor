namespace CommandLineProcessorTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommandLineProcessorContracts;

    using CommandLineProcessorLib;

    using NSubstitute;
    using NSubstitute.ReturnsExtensions;

    using NUnit.Framework;

    [TestFixture]
    public class CommandRepositoryProviderTests
    {
        private List<ICommand> mockCommandList;

        private CommandRepositoryProvider systemUnderTest;

        [Test]
        public void Count_WhenReadAfterLoad_IsCorrect()
        {
            systemUnderTest.Load(mockCommandList);

            Assert.That(systemUnderTest.Count, Is.EqualTo(12));
        }

        [Test]
        public void Count_WhenReadBeforeLoad_IsCorrect()
        {
            Assert.That(systemUnderTest.Count, Is.EqualTo(0));
        }

        [Test]
        public void IntIndex_WhenIndexAboveHighBound_Throws()
        {
            systemUnderTest.Load(mockCommandList);

            Assert.That(
                () =>
                    {
                        var dummy = systemUnderTest[systemUnderTest.Count];
                    },
                Throws.InstanceOf<ArgumentOutOfRangeException>().With.Message.EqualTo(
                    $"Specified index is out of bounds.{Environment.NewLine}Index value: {systemUnderTest.Count}"));
        }

        [Test]
        public void IntIndex_WhenIndexBelowLowBound_Throws()
        {
            systemUnderTest.Load(mockCommandList);

            Assert.That(
                () =>
                    {
                        var dummy = systemUnderTest[-1];
                    },
                Throws.InstanceOf<ArgumentOutOfRangeException>().With.Message.EqualTo(
                    $"Specified index is out of bounds.{Environment.NewLine}Index value: {-1}"));
        }

        [Test]
        public void IntIndex_WhenIndexIsLowBound_ReturnsExpectedItem()
        {
            systemUnderTest.Load(mockCommandList);

            Assert.That(systemUnderTest[0], Is.SameAs(mockCommandList[0]));
        }

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
                    .EqualTo("Command Selector values must be unique."));
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

        [Test]
        public void StringIndex_WhenEmptySelector_Throws()
        {
            systemUnderTest.Load(mockCommandList);

            Assert.That(
                () =>
                    {
                        var dummy = systemUnderTest[string.Empty];
                    },
                Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(
                    $"Value is required.{Environment.NewLine}Parameter name: selector"));
        }

        [Test]
        public void StringIndex_WhenNullSelector_Throws()
        {
            systemUnderTest.Load(mockCommandList);

            Assert.That(
                () =>
                    {
                        var dummy = systemUnderTest[null];
                    },
                Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(
                    $"Value is required.{Environment.NewLine}Parameter name: selector"));
        }

        [Test]
        public void StringIndex_WhenSelectorDoesNotExist_Throws()
        {
            systemUnderTest.Load(mockCommandList);

            Assert.That(
                () =>
                    {
                        var dummy = systemUnderTest["i dont exist"];
                    },
                Throws.InstanceOf<CommandNotFoundException>().With.Message.EqualTo(
                    $"Command not found.{Environment.NewLine}Selector: 'i dont exist'"));
        }

        [Test]
        public void StringIndex_WhenSubCommand_ReturnsExpectedItem()
        {
            systemUnderTest.Load(mockCommandList);

            Assert.That(
                systemUnderTest[" test3|s"],
                Is.SameAs(
                    mockCommandList.Single(x => x.Selectors.Contains("TEst3")).Children
                        .Single(x => x.Selectors.Contains("S"))));
        }

        [Test]
        public void StringIndex_WhenTopLevelCommand_ReturnsExpectedItem()
        {
            systemUnderTest.Load(mockCommandList);

            Assert.That(
                systemUnderTest["TEst3 "],
                Is.SameAs(mockCommandList.Single(x => x.Selectors.Contains("TEst3"))));
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
            command.Selectors.Returns(new[] { "Test" });
            command.Parent.ReturnsNull();
            command.Path.Returns(string.Empty);

            var subCommand1 = Substitute.For<ICommand>();
            subCommand1.Selectors.Returns(new[] { "Sub", "S" });
            subCommand1.Parent.Returns(command);
            subCommand1.Path.Returns("Test");
            var subCommand2 = Substitute.For<ICommand>();
            subCommand2.Selectors.Returns(new[] { "Sub2", "S2" });
            subCommand2.Parent.Returns(command);
            subCommand2.Path.Returns("Test");

            command.Children.Returns(new[] { subCommand1, subCommand2 });
            result.Add(command);

            command = Substitute.For<ICommand>();
            command.Selectors.Returns(new[] { "Test2", "T2" });
            command.Parent.ReturnsNull();
            command.Path.Returns(string.Empty);
            command.Children.Returns(new ICommand[0]);
            result.Add(command);

            command = Substitute.For<ICommand>();
            command.Selectors.Returns(new[] { "TEst3", "T3", "TE" });
            command.Parent.ReturnsNull();
            command.Path.Returns(string.Empty);

            var subCommand3 = Substitute.For<ICommand>();
            subCommand3.Selectors.Returns(new[] { "Sub", "S" });
            subCommand3.Parent.Returns(command);
            subCommand3.Path.Returns("TEst3");

            command.Children.Returns(new[] { subCommand3 });
            result.Add(command);

            return result;
        }
    }
}