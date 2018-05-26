namespace CommandLineProcessorTests.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;

    using CommandLineProcessorCommon;

    using CommandLineProcessorContracts;
    using CommandLineProcessorContracts.Commands;

    using CommandLineProcessorEntity;

    using CommandLineProcessorLib;

    using CommandLineProcessorTests.TestData;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class InputHandlerProviderTests
    {
        private const string CommandRoot = "cmd";

        private ICommand activeCommandMock;

        private ICommandLineProcessorService processorMock;

        private InputHandlerProvider systemUnderTest;

        private List<ICommand> validCommands;

        [TestCase(Constants.Keys.Back)]
        [TestCase(Constants.Keys.Left)]
        public void AllowKeyPress_WhenMovingBackwardsAndIndexEqualToMin_ReturnsFalse(int key)
        {
            var actual = systemUnderTest.AllowKeyPress(key, CommandRoot.Length + 2);
            Assert.IsFalse(actual);
        }

        [TestCase(Constants.Keys.Back)]
        [TestCase(Constants.Keys.Left)]
        public void AllowKeyPress_WhenMovingBackwardsAndIndexGreaterThanMin_ReturnsTrue(int key)
        {
            var actual = systemUnderTest.AllowKeyPress(key, CommandRoot.Length + 3);
            Assert.IsTrue(actual);
        }

        [Test]
        public void AllowKeyPress_WhenTextCharAndIndexEqualToMin_ReturnsTrue()
        {
            var actual = systemUnderTest.AllowKeyPress('A', CommandRoot.Length + 2);
            Assert.IsTrue(actual);
        }

        [Test]
        public void AllowKeyPress_WhenTextCharAndIndexGreaterThanMin_ReturnsTrue()
        {
            var actual = systemUnderTest.AllowKeyPress('A', CommandRoot.Length + 3);
            Assert.IsTrue(actual);
        }

        [Test]
        public void AllowKeyPress_WhenTextCharAndIndexLessThanMin_ReturnsFalse()
        {
            var actual = systemUnderTest.AllowKeyPress('A', CommandRoot.Length + 1);
            Assert.IsFalse(actual);
        }

        [Test]
        public void GetPrompt_ActiveContainerCommandWithoutDefault_ReturnsCorrectText()
        {
            SetUpActiveCommandForContainerCommand();
            Assert.That(systemUnderTest.GetPrompt(), Is.EqualTo($"{CommandRoot}: Test Command 3 (Sub,SubInput,SubInput2): "));
        }

        [Test]
        public void GetPrompt_ActiveContainerCommandWithDefault_ReturnsCorrectText()
        {
            SetUpActiveCommandForContainerCommandWithDefault();
            Assert.That(systemUnderTest.GetPrompt(), Is.EqualTo($"{CommandRoot}: Test Command (Sub,Sub2) [Sub2]: "));
        }

        [Test]
        public void GetPrompt_ActiveInputCommandWithoutDefault_ReturnsCorrectText()
        {
            SetUpActiveCommandForInputCommand();
            Assert.That(systemUnderTest.GetPrompt(), Is.EqualTo($"{CommandRoot}: Test Command 3 (Prompt Text): "));
        }

        [Test]
        public void GetPrompt_ActiveInputCommandWithDefault_ReturnsCorrectText()
        {
            SetUpActiveCommandForInputCommandWithDefault();
            Assert.That(systemUnderTest.GetPrompt(), Is.EqualTo($"{CommandRoot}: Test Command 3 (Prompt Text) [ABC]: "));
        }

        [Test]
        public void GetPrompt_WhenNoActiveCommand_ReturnsCorrectText()
        {
            Assert.That(systemUnderTest.GetPrompt(), Is.EqualTo($"{CommandRoot}: "));
        }

        [Test]
        public void GetPrompt_WhenStackDepthIsOne_ReturnsCorrectText()
        {
            SetUpActiveCommandForInputCommand();
            processorMock.StackDepth.Returns(1);
            Assert.That(
                systemUnderTest.GetPrompt(),
                Is.EqualTo(
                    $"{processorMock.Settings.CommandLevelIndicator} {CommandRoot}: Test Command 3 (Prompt Text): "));
        }

        [Test]
        public void GetPrompt_WhenStackDepthIsTwo_ReturnsCorrectText()
        {
            SetUpActiveCommandForInputCommand();
            processorMock.StackDepth.Returns(2);
            Assert.That(
                systemUnderTest.GetPrompt(),
                Is.EqualTo(
                    $"{processorMock.Settings.CommandLevelIndicator}{processorMock.Settings.CommandLevelIndicator} {CommandRoot}: Test Command 3 (Prompt Text): "));
        }

        [Test]
        public void MinimumSelectionStart_ActiveContainerCommand_ReturnsLengthOfPrompt()
        {
            SetUpActiveCommandForContainerCommand();
            var prompt = systemUnderTest.GetPrompt();
            Assert.That(systemUnderTest.MinimumSelectionStart, Is.EqualTo(prompt.Length));
        }

        [Test]
        public void MinimumSelectionStart_ActiveInputCommand_ReturnsLengthOfPrompt()
        {
            SetUpActiveCommandForInputCommand();
            var prompt = systemUnderTest.GetPrompt();
            Assert.That(systemUnderTest.MinimumSelectionStart, Is.EqualTo(prompt.Length));
        }

        [Test]
        public void MinimumSelectionStart_WhenNoActiveCommand_ReturnsLengthOfPrompt()
        {
            var prompt = systemUnderTest.GetPrompt();
            Assert.That(systemUnderTest.MinimumSelectionStart, Is.EqualTo(prompt.Length));
        }

        [SetUp]
        public void SetUp()
        {
            validCommands = MockCommandGenerator.GenerateValidCommandCollection().ToList();
            processorMock = Substitute.For<ICommandLineProcessorService>();
            processorMock.ActiveCommand.Returns(x => activeCommandMock);
            processorMock.Settings = new CommandLineSettings { CommandPromptRoot = CommandRoot };
            activeCommandMock = null;
            systemUnderTest = new InputHandlerProvider();
            systemUnderTest.Processor = processorMock;
        }

        private void SetUpActiveCommandForContainerCommand()
        {
            activeCommandMock = validCommands.Single(x => x.PrimarySelector.ToUpper() == "TEST3");
        }

        private void SetUpActiveCommandForContainerCommandWithDefault()
        {
            activeCommandMock = validCommands.Single(x => x.PrimarySelector.ToUpper() == "TEST");
        }

        private void SetUpActiveCommandForInputCommand()
        {
            var containerCmd = validCommands.Single(x => x.PrimarySelector.ToUpper() == "TEST3") as IContainerCommand;
            activeCommandMock = containerCmd?.Children.First(x => x.PrimarySelector.ToUpper() == "SUBINPUT");
        }

        private void SetUpActiveCommandForInputCommandWithDefault()
        {
            var containerCmd = validCommands.Single(x => x.PrimarySelector.ToUpper() == "TEST3") as IContainerCommand;
            activeCommandMock = containerCmd?.Children.First(x => x.PrimarySelector.ToUpper() == "SUBINPUT2");
        }
    }
}