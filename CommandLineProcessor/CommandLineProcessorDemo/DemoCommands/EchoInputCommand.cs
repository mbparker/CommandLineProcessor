namespace CommandLineProcessorDemo.DemoCommands
{
    using System.Collections.Generic;

    using CommandLineProcessorContracts;

    public class EchoInputCommand : BaseCommand, IInputCommand
    {
        public EchoInputCommand(IExecutableCommand nextCommand)
        {
            NextCommand = nextCommand;
            NextCommand.Parent = this;
        }

        public override IEnumerable<string> AliasSelectors => new[] { "E" };

        public override string HelpText => "Writes out the specified text to the history window.";

        public override string Name => "Echo";

        public ICommand NextCommand { get; }

        public override string PrimarySelector => "Echo";

        public string Prompt => "Enter Text";

        public void ApplyInput(ICommandContext context, string inputText)
        {
            context.DataStore.Add("TEXT", inputText);
        }
    }
}