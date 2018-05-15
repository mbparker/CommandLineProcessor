namespace CommandLineProcessorDemo.DemoCommands.Math.Add
{
    using CommandLineProcessorContracts;
    public class AddInputCommand2 : BaseCommand, IInputCommand
    {
        public AddInputCommand2(IExecutableCommand nextCommand)
        {
            NextCommand = nextCommand;
            NextCommand.Parent = this;
        }

        public ICommand NextCommand { get; }

        public override string PrimarySelector => string.Empty;

        public string Prompt => "Enter second number";

        public void ApplyInput(ICommandContext context, string inputText)
        {
            context.DataStore.Add("N2", double.Parse(inputText));
        }
    }
}