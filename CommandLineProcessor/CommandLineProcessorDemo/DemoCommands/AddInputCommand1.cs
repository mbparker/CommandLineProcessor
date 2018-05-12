namespace CommandLineProcessorDemo.DemoCommands
{
    using System.Collections.Generic;

    using CommandLineProcessorContracts;

    public class AddInputCommand1 : BaseCommand, IInputCommand
    {
        public AddInputCommand1(IInputCommand nextCommand)
        {
            NextCommand = nextCommand;
            NextCommand.Parent = this;
        }

        public override IEnumerable<string> AliasSelectors => new[] { "A" };

        public override string HelpText => "Adds two decimal numbers";

        public override string Name => "Add";

        public ICommand NextCommand { get; }

        public override string PrimarySelector => "Add";

        public string Prompt => "Enter first number";

        public void ApplyInput(ICommandContext context, string inputText)
        {
            context.DataStore.Add("N1", double.Parse(inputText));
        }
    }
}