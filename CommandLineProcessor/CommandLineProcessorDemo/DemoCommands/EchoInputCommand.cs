namespace CommandLineProcessorDemo.DemoCommands
{
    using System.Collections.Generic;

    using CommandLineProcessorContracts;

    public class EchoInputCommand : IInputCommand
    {
        public EchoInputCommand(IExecutableCommand nextCommand)
        {
            NextCommand = nextCommand;
            Inputs = new Dictionary<string, object>();
        }

        public IEnumerable<string> AliasSelectors => new[] { "E" };

        public string HelpText => "Writes out the specified text to the history window.";

        public IDictionary<string, object> Inputs { get; }

        public string Name => "Echo";

        public ICommand NextCommand { get; }

        public ICommand Parent { get; set; }

        public string Path => string.Empty;

        public string PrimarySelector => "Echo";

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