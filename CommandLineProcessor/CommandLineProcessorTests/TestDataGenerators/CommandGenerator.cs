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
            command.Selectors.Returns(new[] { "Test", "T" });
            command.Parent.Returns((ICommand)null);
            result.Add(command);
            command = Substitute.For<ICommand>();
            command.Selectors.Returns(new[] { "Test2", "T" });
            command.Parent.Returns((ICommand)null);
            result.Add(command);
            return result;
        }

        public static IEnumerable<ICommand> GenerateValidCommandCollection()
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