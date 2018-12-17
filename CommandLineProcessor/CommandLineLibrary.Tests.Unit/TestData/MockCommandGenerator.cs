namespace CommandLineLibrary.Tests.Unit.TestData
{
    using System.Collections.Generic;

    using CommandLineLibrary.Contracts;
    using CommandLineLibrary.Contracts.Commands;

    using NSubstitute;
    using NSubstitute.ReturnsExtensions;

    public static class MockCommandGenerator
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
            command.Name.Returns("Test Command");
            command.CommandIs<IContainerCommand>().Returns(true);

            var subCommand1 = Substitute.For<IExecutableCommand>();
            subCommand1.PrimarySelector.Returns("Sub");
            subCommand1.AliasSelectors.Returns(new[] { "S" });
            subCommand1.Parent.Returns(command);
            subCommand1.Path.Returns("Test");
            subCommand1.CommandIs<IExecutableCommand>().Returns(true);
            var subCommand2 = Substitute.For<IExecutableCommand>();
            subCommand2.PrimarySelector.Returns("Sub2");
            subCommand2.AliasSelectors.Returns(new[] { "S2" });
            subCommand2.Parent.Returns(command);
            subCommand2.Path.Returns("Test");
            subCommand2.CommandIs<IExecutableCommand>().Returns(true);

            command.Children.Returns(new[] { subCommand1, subCommand2 });
            command.GetDefaultCommandSelector(Arg.Any<ICommandContext>()).Returns("Sub2");
            result.Add(command);

            var executableCommand = Substitute.For<IExecutableCommand>();
            executableCommand.PrimarySelector.Returns("Test2");
            executableCommand.AliasSelectors.Returns(new[] { "T2" });
            executableCommand.Parent.ReturnsNull();
            executableCommand.Path.Returns(string.Empty);
            executableCommand.CommandIs<IExecutableCommand>().Returns(true);
            result.Add(executableCommand);

            command = Substitute.For<IContainerCommand>();
            command.PrimarySelector.Returns("TEst3");
            command.AliasSelectors.Returns(new[] { "T3", "TE" });
            command.Parent.ReturnsNull();
            command.Path.Returns(string.Empty);
            command.Name.Returns("Test Command 3");
            command.CommandIs<IContainerCommand>().Returns(true);

            var subCommand3 = Substitute.For<IExecutableCommand>();
            subCommand3.PrimarySelector.Returns("Sub");
            subCommand3.AliasSelectors.Returns(new[] { "S" });
            subCommand3.Parent.Returns(command);
            subCommand3.Path.Returns("TEst3");
            subCommand3.CommandIs<IExecutableCommand>().Returns(true);

            var subCommand4 = Substitute.For<IInputCommand>();
            subCommand4.PrimarySelector.Returns("SubInput");
            subCommand4.AliasSelectors.Returns(new[] { "SI" });
            subCommand4.Parent.Returns(command);
            subCommand4.Path.Returns("TEst3");
            subCommand4.GetPromptText(Arg.Any<ICommandContext>()).Returns("Prompt Text");
            subCommand4.Name.Returns(x => subCommand4.Parent.Name);
            subCommand4.CommandIs<IInputCommand>().Returns(true);

            var subCommand5 = Substitute.For<IExecutableCommand>();
            subCommand5.PrimarySelector.ReturnsNull();
            subCommand5.AliasSelectors.Returns(new string[0]);
            subCommand5.Parent.Returns(subCommand4);
            subCommand5.Path.ReturnsNull();
            subCommand5.CommandIs<IExecutableCommand>().Returns(true);

            subCommand4.NextCommand.Returns(subCommand5);

            var subCommand6 = Substitute.For<IInputCommand>();
            subCommand6.PrimarySelector.Returns("SubInput2");
            subCommand6.AliasSelectors.Returns(new[] { "SI2" });
            subCommand6.Parent.Returns(command);
            subCommand6.Path.Returns("TEst3");
            subCommand6.GetPromptText(Arg.Any<ICommandContext>()).Returns("Prompt Text");
            subCommand6.Name.Returns(x => subCommand4.Parent.Name);
            subCommand6.CommandIs<IInputCommand>().Returns(true);
            subCommand6.GetDefaultValue(Arg.Any<ICommandContext>()).Returns("ABC");

            command.Children.Returns(new ICommand[] { subCommand3, subCommand4, subCommand6 });
            result.Add(command);

            return result;
        }
    }
}