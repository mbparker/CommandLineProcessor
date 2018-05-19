namespace CommandLineProcessorTests
{
    using System;
    using System.Collections.Generic;

    using CommandLineProcessorLib;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class CommandDataStoreTests
    {
        private IDictionary<string, object> innerStoreMock;

        private CommandDataStore systemUnderTest;

        [Test]
        public void Clear_WhenInvoked_ClearsInnerStore()
        {
            systemUnderTest.Clear();

            innerStoreMock.Received(1).Clear();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Get_WhenKeyInvalid_Throws(string key)
        {
            Assert.That(
                () => { systemUnderTest.Get<double>(key); },
                Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(
                    $"key is a required argument when getting a value from the data store.{Environment.NewLine}Parameter name: key"));
        }

        [Test]
        public void Get_WhenValueDoesNotExist_ReturnsDefaultValue()
        {
            innerStoreMock.ContainsKey("TEST").Returns(false);

            var actual = systemUnderTest.Get<double>("test");

            Assert.AreEqual(0.0, actual);
        }

        [Test]
        public void Get_WhenValueExists_ReturnsValue()
        {
            innerStoreMock.ContainsKey("TEST").Returns(true);
            innerStoreMock["TEST"].Returns(42.24);

            var actual = systemUnderTest.Get<double>("test");

            Assert.AreEqual(42.24, actual);
        }

        [Test]
        public void Get_WhenValueIsDifferentType_Throws()
        {
            innerStoreMock.ContainsKey("TEST").Returns(true);
            innerStoreMock["TEST"].Returns(42.24);

            Assert.That(
                () => { systemUnderTest.Get<string>("test"); },
                Throws.InstanceOf<InvalidCastException>().With.Message.EqualTo(
                    "Unable to cast object of type 'System.Double' to type 'System.String'."));
        }

        [Test]
        public void Set_WhenExistingValueIsDifferentType_DoesNotThrow()
        {
            innerStoreMock.ContainsKey("TEST").Returns(true);
            innerStoreMock["TEST"].Returns(42.24);

            Assert.DoesNotThrow(() => { systemUnderTest.Set("test", "Test Value"); });
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Set_WhenKeyInvalid_Throws(string key)
        {
            Assert.That(
                () => { systemUnderTest.Set(key, 42.24); },
                Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(
                    $"key is a required argument when setting a value to the data store.{Environment.NewLine}Parameter name: key"));
        }

        [Test]
        public void Set_WhenValueDoesNotExist_AddsValue()
        {
            systemUnderTest.Set("test", 42.24);

            innerStoreMock.Received(1).Add("TEST", 42.24);
        }

        [Test]
        public void Set_WhenValueExists_SetsExistingValue()
        {
            innerStoreMock.ContainsKey("TEST").Returns(true);

            systemUnderTest.Set("test", 42.24);

            innerStoreMock.Received(1)["TEST"] = 42.24;
        }

        [SetUp]
        public void SetUp()
        {
            innerStoreMock = Substitute.For<IDictionary<string, object>>();
            systemUnderTest = new CommandDataStore(innerStoreMock);
        }
    }
}