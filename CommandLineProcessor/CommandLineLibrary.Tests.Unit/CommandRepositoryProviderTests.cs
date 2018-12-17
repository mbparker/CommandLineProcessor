namespace CommandLineLibrary.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommandLineLibrary.Contracts.Commands;
    using CommandLineLibrary.Models.Exceptions;
    using CommandLineLibrary.Tests.Unit.TestData;

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

            Assert.That(systemUnderTest.Count, Is.EqualTo(16));
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
                () => { systemUnderTest.Load(MockCommandGenerator.GenerateCommandCollectionWithDuplicateSelectors()); },
                Throws.InstanceOf<DuplicateCommandSelectorException>().With.Message
                    .EqualTo("Cannot add 'T'. Command Selector values must be unique."));
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
            mockCommandList = new List<ICommand>(MockCommandGenerator.GenerateValidCommandCollection());
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
                    "Command 'i dont exist' not found."));
        }

        [Test]
        public void StringIndex_WhenSubCommand_ReturnsExpectedItem()
        {
            systemUnderTest.Load(mockCommandList);

            Assert.That(
                systemUnderTest[" test3|s"],
                Is.SameAs(
                    (mockCommandList.Single(x => x.PrimarySelector == "TEst3") as IContainerCommand).Children
                        .Single(x => x.AliasSelectors.Contains("S"))));
        }

        [Test]
        public void StringIndex_WhenTopLevelCommand_ReturnsExpectedItem()
        {
            systemUnderTest.Load(mockCommandList);

            Assert.That(
                systemUnderTest["TEst3 "],
                Is.SameAs(mockCommandList.Single(x => x.PrimarySelector == "TEst3")));
        }
    }
}