namespace CommandLineProcessorDemo.DemoCommands.Math.Multiply
{
    using CommandLineProcessorContracts;

    public class MultiplyInputCommand2 : BaseCommand, IInputCommand
    {
        public MultiplyInputCommand2(IExecutableCommand nextCommand)
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