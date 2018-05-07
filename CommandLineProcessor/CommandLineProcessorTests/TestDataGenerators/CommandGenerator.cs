namespace CommandLineProcessorTests.TestDataGenerators
{
    using System.Collections.Generic;

    using CommandLineProcessorContracts;

    using NSubstitute;
    using NSubstitute.ReturnsExtensions;

    public static class CommandGenerator
    {
        public static IEnumerable<ICommand> GenerateCommandCollectionWithDuplicateSelectors()
        {
            var result = new List<ICommand>();
            var command = Substitute.For<ICommand>();
            command.PrimarySelector.Returns("Test");
            command.AliasSelectors.Returns(new[] { "TAlias", "T" });
            command.Parent.Returns((ICommand)null);
            result.Add(command);
            command = Substitute.For<ICommand>();
            command.PrimarySelector.Returns("Test2");
            command.AliasSelectors.Returns(new[] { "T", "TAlias2" });
            command.Parent.Returns((ICommand)null);
            result.Add(command);
            return result;
        }

        public static IEnumerable<ICommand> GenerateValidCommandCollection()
        {
            var result = new List<ICommand>();

            var command = Substitute.For<IContainerCommand>();
            command.PrimarySelector.Returns("Test");
            command.AliasSelectors.Returns(new string[0]);
            command.Parent.ReturnsNull();
            command.Path.Returns(string.Empty);

            var subCommand1 = Substitute.For<IExecutableCommand>();
            subCommand1.PrimarySelector.Returns("Sub");
            subCommand1.AliasSelectors.Returns(new[] { "S" });
            subCommand1.Parent.Returns(command);
            subCommand1.Path.Returns("Test");
            var subCommand2 = Substitute.For<IExecutableCommand>();
            subCommand2.PrimarySelector.Returns("Sub2");
            subCommand2.AliasSelectors.Returns(new[] { "S2" });
            subCommand2.Parent.Returns(command);
            subCommand2.Path.Returns("Test");

            command.Children.Returns(new[] { subCommand1, subCommand2 });
            result.Add(command);

            var executableCommand = Substitute.For<IExecutableCommand>();
            executableCommand.PrimarySelector.Returns("Test2");
            executableCommand.AliasSelectors.Returns(new[] { "T2" });
            executableCommand.Parent.ReturnsNull();
            executableCommand.Path.Returns(string.Empty);
            result.Add(executableCommand);

            command = Substitute.For<IContainerCommand>();
            command.PrimarySelector.Returns("TEst3");
            command.AliasSelectors.Returns(new[] { "T3", "TE" });
            command.Parent.ReturnsNull();
            command.Path.Returns(string.Empty);
            command.Name.Returns("Test Command 3");

            var subCommand3 = Substitute.For<IExecutableCommand>();
            subCommand3.PrimarySelector.Returns("Sub");
            subCommand3.AliasSelectors.Returns(new[] { "S" });
            subCommand3.Parent.Returns(command);
            subCommand3.Path.Returns("TEst3");

            var subCommand4 = Substitute.For<IInputCommand>();
            subCommand4.PrimarySelector.Returns("SubInput");
            subCommand4.AliasSelectors.Returns(new[] { "SI" });
            subCommand4.Parent.Returns(command);
            subCommand4.Path.Returns("TEst3");
            subCommand4.Prompt.Returns("Prompt Text");
            subCommand4.Name.Returns(x => subCommand4.Parent.Name);

            var subCommand5 = Substitute.For<IExecutableCommand>();
            subCommand5.PrimarySelector.ReturnsNull();
            subCommand5.AliasSelectors.Returns(new string[0]);
            subCommand5.Parent.Returns(subCommand4);
            subCommand5.Path.ReturnsNull();

            subCommand4.NextCommand.Returns(subCommand5);

            command.Children.Returns(new ICommand[] { subCommand3, subCommand4 });
            result.Add(command);

            return result;
        }
    }
}