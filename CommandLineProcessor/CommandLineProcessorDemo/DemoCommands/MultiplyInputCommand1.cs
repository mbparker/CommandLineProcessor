namespace CommandLineProcessorDemo.DemoCommands
{
    using System.Collections.Generic;

    using CommandLineProcessorContracts;

    public class MultiplyInputCommand1 : BaseCommand, IInputCommand
    {
        public MultiplyInputCommand1(IInputCommand nextCommand)
        {
            NextCommand = nextCommand;
            NextCommand.Parent = this;
        }

        public override IEnumerable<string> AliasSelectors => new[] { "M" };

        public override string HelpText => "Multiplies two decimal numbers";

        public override string Name => "Multiply";

        public ICommand NextCommand { get; }

        public override string PrimarySelector => "Mult";

        public string Prompt => "Enter first number";

        public void ApplyInput(ICommandContext context, string inputText)
        {
            context.DataStore.Add("N1", double.Parse(inputText));
        }
    }
}