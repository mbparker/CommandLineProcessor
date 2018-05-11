namespace CommandLineProcessorDemo.DemoCommands
{
    using System.Collections.Generic;

    using CommandLineProcessorContracts;

    public class EchoInputCommand : BaseCommand, IInputCommand
    {
        public EchoInputCommand(IExecutableCommand nextCommand)
        {
            NextCommand = nextCommand;
            Inputs = new Dictionary<string, object>();
        }

        public override IEnumerable<string> AliasSelectors => new[] { "E" };

        public override string HelpText => "Writes out the specified text to the history window.";

        public IDictionary<string, object> Inputs { get; }

        public override string Name => "Echo";

        public ICommand NextCommand { get; }

        public override ICommand Parent { get; set; }

        public override string Path => string.Empty;

        public override string PrimarySelector => "Echo";

        public string Prompt => "Enter Text";

        public void ApplyInput(string inputText)
        {
            if (Inputs.ContainsKey("TEXT"))
            {
                Inputs.Remove("TEXT");
            }

            Inputs.Add("TEXT", inputText);
        }
    }
}