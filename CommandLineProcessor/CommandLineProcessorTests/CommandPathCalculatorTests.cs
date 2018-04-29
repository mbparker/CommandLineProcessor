namespace CommandLineProcessorTests
{
    using CommandLineProcessorContracts;

    using CommandLineProcessorLib;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class CommandPathCalculatorTests
    {
        private CommandPathCalculator systemUnderTest;

        [Test]
        public void CalculateFullyQualifiedPath_WhenInvokedWithNoActiveCommand_ReturnsExpectedPath()
        {
            var input = "command";
            var expected = input;
            var actual = systemUnderTest.CalculateFullyQualifiedPath(null, input);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CalculateFullyQualifiedPath_WhenInvokedWithSubCommand_ReturnsExpectedPath()
        {
            var commandMock = Substitute.For<ICommand>();
            commandMock.Selectors.Returns(new[] { "CS1", "CS2" });
            commandMock.Path.Returns("root|next");

            var input = "command";
            var expected = "root|next|CS1|command";
            var actual = systemUnderTest.CalculateFullyQualifiedPath(commandMock, input);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CalculateFullyQualifiedPath_WhenInvokedWithTopLevelCommand_ReturnsExpectedPath()
        {
            var commandMock = Substitute.For<ICommand>();
            commandMock.Selectors.Returns(new[] { "CS1", "CS2" });

            var input = "command";
            var expected = "CS1|command";
            var actual = systemUnderTest.CalculateFullyQualifiedPath(commandMock, input);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [SetUp]
        public void SetUp()
        {
            systemUnderTest = new CommandPathCalculator();
        }
    }
}