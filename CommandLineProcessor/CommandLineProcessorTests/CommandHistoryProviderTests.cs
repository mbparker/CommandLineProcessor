namespace CommandLineProcessorTests
{
    using CommandLineProcessorContracts;

    using CommandLineProcessorLib;

    using NSubstitute;
    using NSubstitute.ReturnsExtensions;

    using NUnit.Framework;

    [TestFixture]
    public class CommandHistoryProviderTests
    {
        private ICommand command1Mock;

        private ICommand command2Mock;

        private ICommand command3Mock;

        private CommandHistoryProvider systemUnderTest;

        [Test]
        public void First_WhenEmpty_ReturnsNull()
        {
            Assert.IsNull(systemUnderTest.First());
        }

        [Test]
        public void First_WhenOneItem_ReturnsItem()
        {
            systemUnderTest.NotifyCommandExecuting(command1Mock);

            Assert.AreSame(command1Mock, systemUnderTest.First());
        }

        [Test]
        public void Last_WhenEmpty_ReturnsNull()
        {
            Assert.IsNull(systemUnderTest.Last());
        }

        [Test]
        public void Last_WhenOneItem_ReturnsItem()
        {
            systemUnderTest.NotifyCommandExecuting(command1Mock);

            Assert.AreSame(command1Mock, systemUnderTest.Last());
        }

        [Test]
        public void Next_WhenAtEnd_ReturnsNull()
        {
            systemUnderTest.NotifyCommandExecuting(command1Mock);
            systemUnderTest.Last();

            Assert.IsNull(systemUnderTest.Next());
        }

        [Test]
        public void Next_WhenEmpty_ReturnsNull()
        {
            Assert.IsNull(systemUnderTest.Next());
        }

        [Test]
        public void Next_WhenMultipleItemsAndAtBeginning_ReturnsItemsInOrder()
        {
            systemUnderTest.NotifyCommandExecuting(command1Mock);
            systemUnderTest.NotifyCommandExecuting(command2Mock);
            systemUnderTest.NotifyCommandExecuting(command3Mock);

            Assert.AreSame(command1Mock, systemUnderTest.First());
            Assert.AreSame(command2Mock, systemUnderTest.Next());
            Assert.AreSame(command3Mock, systemUnderTest.Next());
            Assert.IsNull(systemUnderTest.Next());
        }

        [Test]
        public void Next_WhenOneItemAndAtEnd_ReturnsNull()
        {
            systemUnderTest.NotifyCommandExecuting(command1Mock);

            Assert.IsNull(systemUnderTest.Next());
        }

        [Test]
        public void NotifyCommandExecuting_WhenCommandNull_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => systemUnderTest.NotifyCommandExecuting(null));
        }

        [Test]
        public void NotifyCommandExecuting_WhenCommandParented_DoesNotAddToHistory()
        {
            command2Mock.Parent.Returns(command1Mock);

            systemUnderTest.NotifyCommandExecuting(command2Mock);

            Assert.IsNull(systemUnderTest.First());
        }

        [Test]
        public void Previous_WhenAtBeginning_ReturnsNull()
        {
            systemUnderTest.NotifyCommandExecuting(command1Mock);
            systemUnderTest.First();

            Assert.IsNull(systemUnderTest.Previous());
        }

        [Test]
        public void Previous_WhenEmpty_ReturnsNull()
        {
            Assert.IsNull(systemUnderTest.Previous());
        }

        [Test]
        public void Previous_WhenMultipleItems_ReturnsItemsInOrder()
        {
            systemUnderTest.NotifyCommandExecuting(command1Mock);
            systemUnderTest.NotifyCommandExecuting(command2Mock);
            systemUnderTest.NotifyCommandExecuting(command3Mock);

            Assert.AreSame(command3Mock, systemUnderTest.Previous());
            Assert.AreSame(command2Mock, systemUnderTest.Previous());
            Assert.AreSame(command1Mock, systemUnderTest.Previous());
            Assert.IsNull(systemUnderTest.Previous());
        }

        [Test]
        public void Previous_WhenOneItem_ReturnsItem()
        {
            systemUnderTest.NotifyCommandExecuting(command1Mock);

            Assert.AreSame(command1Mock, systemUnderTest.Previous());
        }

        [SetUp]
        public void SetUp()
        {
            systemUnderTest = new CommandHistoryProvider();
            command1Mock = Substitute.For<ICommand>();
            command1Mock.Parent.ReturnsNull();
            command2Mock = Substitute.For<ICommand>();
            command2Mock.Parent.ReturnsNull();
            command3Mock = Substitute.For<ICommand>();
            command3Mock.Parent.ReturnsNull();
        }
    }
}