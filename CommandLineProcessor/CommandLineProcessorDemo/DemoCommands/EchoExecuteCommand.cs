namespace CommandLineProcessorDemo.DemoCommands
{
    using System;
    using System.Collections.Generic;

    using CommandLineProcessorContracts;

    public class EchoExecuteCommand : IExecutableCommand
    {
        private readonly Action<string> executeAction;

        public EchoExecuteCommand(Action<string> executeAction)
        {
            this.executeAction = executeAction;
        }

        public IEnumerable<string> AliasSelectors => new string[0];

        public string HelpText => string.Empty;

        public string Name => string.Empty;

        public ICommand Parent { get; set; }

        public string Path => Parent?.PrimarySelector ?? string.Empty;

        public string PrimarySelector => "EchoExecute";

        public void Execute(params object[] args)
        {
            executeAction?.Invoke((string)args[0]);
        }

        public object[] GetArguments()
        {
            var parentInput = Parent as IInputCommand;
            return new[] { parentInput?.Inputs["TEXT"] };
        }
    }
}