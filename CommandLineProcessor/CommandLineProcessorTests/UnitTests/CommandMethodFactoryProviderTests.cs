namespace CommandLineProcessorTests.UnitTests
{
    using System.Collections.Generic;

    using CommandLineProcessorCommon.Ioc;

    using CommandLineProcessorContracts;
    using CommandLineProcessorContracts.Commands;

    using CommandLineProcessorLib;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class CommandMethodFactoryProviderTests
    {
        private const string DefaultValueText = "default-value";

        private const string InputText = "input-text";

        private const string PromptText = "prompt-text";

        private ICommandContext commandContextMock;

        private IIocContainer iocContainerMock;

        private IMethodCallValidatorService methodCallValidatorServiceMock;

        private CommandMethodFactoryProvider systemUnderTest;

        private ITestingCommandImplementor testingCommandImplementorMock;

        private TestingCommandImplementor testingCommandImplementorProxy;

        [Test]
        public void CreateApplyInputActionForInputCommand_WhenInvokedWithInstance_CreatesCorrectAction()
        {
            SetUpApplyInput();

            var action = systemUnderTest.CreateApplyInputActionForInputCommand(
                x => x.ApplyInput(null, null),
                testingCommandImplementorProxy);

            action(commandContextMock, InputText);

            commandContextMock.DidNotReceive().GetService<TestingCommandImplementor>();
            testingCommandImplementorMock.Received(1).ApplyInput(commandContextMock, InputText);
        }

        [Test]
        public void CreateApplyInputActionForInputCommand_WhenInvokedWithoutInstance_CreatesCorrectAction()
        {
            SetUpApplyInput();

            var action =
                systemUnderTest.CreateApplyInputActionForInputCommand<TestingCommandImplementor>(
                    x => x.ApplyInput(null, null),
                    null);

            action(commandContextMock, InputText);

            commandContextMock.Received(1).GetService<TestingCommandImplementor>();
            testingCommandImplementorMock.Received(1).ApplyInput(commandContextMock, InputText);
        }

        [Test]
        public void CreateDefaultCommandFuncForContainerCommand_WhenInvokedWithInstance_CreatesCorrectAction()
        {
            SetUpGetDefaultCommand();
            var commandList = Substitute.For<IEnumerable<ICommand>>();
            var expectedCommand = Substitute.For<ICommand>();
            testingCommandImplementorMock.GetDefaultCommand(commandContextMock, commandList).Returns(expectedCommand);

            var func = systemUnderTest.CreateDefaultCommandFuncForContainerCommand(
                x => x.GetDefaultCommand(null, null),
                testingCommandImplementorProxy);

            var actualCommand = func(commandContextMock, commandList);

            Assert.AreSame(expectedCommand, actualCommand);
            commandContextMock.DidNotReceive().GetService<TestingCommandImplementor>();
            testingCommandImplementorMock.Received(1).GetDefaultCommand(commandContextMock, commandList);
        }

        [Test]
        public void CreateDefaultCommandFuncForContainerCommand_WhenInvokedWithoutInstance_CreatesCorrectAction()
        {
            SetUpGetDefaultCommand();
            var commandList = Substitute.For<IEnumerable<ICommand>>();
            var expectedCommand = Substitute.For<ICommand>();
            testingCommandImplementorMock.GetDefaultCommand(commandContextMock, commandList).Returns(expectedCommand);

            var func = systemUnderTest.CreateDefaultCommandFuncForContainerCommand<TestingCommandImplementor>(
                x => x.GetDefaultCommand(null, null),
                null);

            var actualCommand = func(commandContextMock, commandList);

            Assert.AreSame(expectedCommand, actualCommand);
            commandContextMock.Received(1).GetService<TestingCommandImplementor>();
            testingCommandImplementorMock.Received(1).GetDefaultCommand(commandContextMock, commandList);
        }

        [Test]
        public void CreateDefaultFuncForInputCommand_WhenInvokedWithInstance_CreatesCorrectAction()
        {
            SetUpGetDefaultValue();
            var expected = DefaultValueText;
            testingCommandImplementorMock.GetDefaultValue(commandContextMock).Returns(DefaultValueText);

            var func = systemUnderTest.CreateDefaultFuncForInputCommand(
                x => x.GetDefaultValue(null),
                testingCommandImplementorProxy);

            var actual = func(commandContextMock);

            Assert.AreEqual(expected, actual);
            commandContextMock.DidNotReceive().GetService<TestingCommandImplementor>();
            testingCommandImplementorMock.Received(1).GetDefaultValue(commandContextMock);
        }

        [Test]
        public void CreateDefaultFuncForInputCommand_WhenInvokedWithoutInstance_CreatesCorrectAction()
        {
            SetUpGetDefaultValue();
            var expected = DefaultValueText;
            testingCommandImplementorMock.GetDefaultValue(commandContextMock).Returns(DefaultValueText);

            var func = systemUnderTest.CreateDefaultFuncForInputCommand<TestingCommandImplementor>(
                x => x.GetDefaultValue(null),
                null);

            var actual = func(commandContextMock);

            Assert.AreEqual(expected, actual);
            commandContextMock.Received(1).GetService<TestingCommandImplementor>();
            testingCommandImplementorMock.Received(1).GetDefaultValue(commandContextMock);
        }

        [Test]
        public void CreateExecuteActionForExecuteCommand_WhenInvokedWithInstance_CreatesCorrectAction()
        {
            SetUpExecute();

            var action = systemUnderTest.CreateExecuteActionForExecuteCommand(
                x => x.Execute(null),
                testingCommandImplementorProxy);

            action(commandContextMock);

            commandContextMock.DidNotReceive().GetService<TestingCommandImplementor>();
            testingCommandImplementorMock.Received(1).Execute(commandContextMock);
        }

        [Test]
        public void CreateExecuteActionForExecuteCommand_WhenInvokedWithoutInstance_CreatesCorrectAction()
        {
            SetUpExecute();

            var action =
                systemUnderTest.CreateExecuteActionForExecuteCommand<TestingCommandImplementor>(
                    x => x.Execute(null),
                    null);

            action(commandContextMock);

            commandContextMock.Received(1).GetService<TestingCommandImplementor>();
            testingCommandImplementorMock.Received(1).Execute(commandContextMock);
        }

        [Test]
        public void CreatePromptTextFuncForInputCommand_WhenInvokedWithInstance_CreatesCorrectAction()
        {
            SetUpGetPromptText();
            var expected = PromptText;
            testingCommandImplementorMock.GetPromptText(commandContextMock).Returns(PromptText);

            var func = systemUnderTest.CreatePromptTextFuncForInputCommand(
                x => x.GetPromptText(null),
                testingCommandImplementorProxy);

            var actual = func(commandContextMock);

            Assert.AreEqual(expected, actual);
            commandContextMock.DidNotReceive().GetService<TestingCommandImplementor>();
            testingCommandImplementorMock.Received(1).GetPromptText(commandContextMock);
        }

        [Test]
        public void CreatePromptTextFuncForInputCommand_WhenInvokedWithoutInstance_CreatesCorrectAction()
        {
            SetUpGetPromptText();
            var expected = PromptText;
            testingCommandImplementorMock.GetPromptText(commandContextMock).Returns(PromptText);

            var func = systemUnderTest.CreatePromptTextFuncForInputCommand<TestingCommandImplementor>(
                x => x.GetPromptText(null),
                null);

            var actual = func(commandContextMock);

            Assert.AreEqual(expected, actual);
            commandContextMock.Received(1).GetService<TestingCommandImplementor>();
            testingCommandImplementorMock.Received(1).GetPromptText(commandContextMock);
        }

        [SetUp]
        public void SetUp()
        {
            methodCallValidatorServiceMock = Substitute.For<IMethodCallValidatorService>();
            systemUnderTest = new CommandMethodFactoryProvider(methodCallValidatorServiceMock);
            commandContextMock = Substitute.For<ICommandContext>();
            iocContainerMock = Substitute.For<IIocContainer>();
            testingCommandImplementorMock = Substitute.For<ITestingCommandImplementor>();
            testingCommandImplementorProxy = new TestingCommandImplementor(testingCommandImplementorMock);
            iocContainerMock.Resolve<TestingCommandImplementor>().Returns(testingCommandImplementorProxy);
            commandContextMock.GetService<TestingCommandImplementor>().Returns(testingCommandImplementorProxy);
        }

        private void SetUpApplyInput()
        {
            var methodInfo = typeof(TestingCommandImplementor).GetMethod(nameof(TestingCommandImplementor.ApplyInput));
            methodCallValidatorServiceMock
                .ValidateMethodCallExpressionAndReturnMethodInfo<TestingCommandImplementor,
                    CommandMethodFactoryProvider.ICommandDelegateMethodSignatures>(
                    x => x.ApplyInput(null, null),
                    y => y.ApplyInput(null, null)).ReturnsForAnyArgs(methodInfo);
        }

        private void SetUpExecute()
        {
            var methodInfo = typeof(TestingCommandImplementor).GetMethod(nameof(TestingCommandImplementor.Execute));
            methodCallValidatorServiceMock
                .ValidateMethodCallExpressionAndReturnMethodInfo<TestingCommandImplementor,
                    CommandMethodFactoryProvider.ICommandDelegateMethodSignatures>(
                    x => x.Execute(null),
                    y => y.Execute(null)).ReturnsForAnyArgs(methodInfo);
        }

        private void SetUpGetDefaultCommand()
        {
            var methodInfo =
                typeof(TestingCommandImplementor).GetMethod(nameof(TestingCommandImplementor.GetDefaultCommand));
            methodCallValidatorServiceMock
                .ValidateMethodCallExpressionAndReturnMethodInfo<TestingCommandImplementor,
                    CommandMethodFactoryProvider.ICommandDelegateMethodSignatures>(
                    x => x.GetDefaultCommand(null, null),
                    y => y.GetDefaultCommand(null, null)).ReturnsForAnyArgs(methodInfo);
        }

        private void SetUpGetDefaultValue()
        {
            var methodInfo =
                typeof(TestingCommandImplementor).GetMethod(nameof(TestingCommandImplementor.GetDefaultValue));
            methodCallValidatorServiceMock
                .ValidateMethodCallExpressionAndReturnMethodInfo<TestingCommandImplementor,
                    CommandMethodFactoryProvider.ICommandDelegateMethodSignatures>(
                    x => x.GetDefaultValue(null),
                    y => y.GetDefaultValue(null)).ReturnsForAnyArgs(methodInfo);
        }

        private void SetUpGetPromptText()
        {
            var methodInfo =
                typeof(TestingCommandImplementor).GetMethod(nameof(TestingCommandImplementor.GetPromptText));
            methodCallValidatorServiceMock
                .ValidateMethodCallExpressionAndReturnMethodInfo<TestingCommandImplementor,
                    CommandMethodFactoryProvider.ICommandDelegateMethodSignatures>(
                    x => x.GetPromptText(null),
                    y => y.GetPromptText(null)).ReturnsForAnyArgs(methodInfo);
        }
    }

    public interface ITestingCommandImplementor
    {
        void ApplyInput(ICommandContext context, string inputText);

        void Execute(ICommandContext context);

        ICommand GetDefaultCommand(ICommandContext context, IEnumerable<ICommand> commands);

        string GetDefaultValue(ICommandContext context);

        string GetPromptText(ICommandContext context);
    }

    public class TestingCommandImplementor : ITestingCommandImplementor
    {
        private readonly ITestingCommandImplementor mock;

        public TestingCommandImplementor(ITestingCommandImplementor mock)
        {
            this.mock = mock;
        }

        public void ApplyInput(ICommandContext context, string inputText)
        {
            mock.ApplyInput(context, inputText);
        }

        public void Execute(ICommandContext context)
        {
            mock.Execute(context);
        }

        public ICommand GetDefaultCommand(ICommandContext context, IEnumerable<ICommand> commands)
        {
            return mock.GetDefaultCommand(context, commands);
        }

        public string GetDefaultValue(ICommandContext context)
        {
            return mock.GetDefaultValue(context);
        }

        public string GetPromptText(ICommandContext context)
        {
            return mock.GetPromptText(context);
        }
    }
}