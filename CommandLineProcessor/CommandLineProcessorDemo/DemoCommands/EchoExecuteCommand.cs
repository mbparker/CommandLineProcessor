namespace CommandLineProcessorDemo.DemoCommands
{
    using System;
    using System.Collections.Generic;

    using CommandLineProcessorContracts;

    public class EchoExecuteCommand : BaseCommand, IExecutableCommand
    {
        private readonly Action<string> executeAction;

        public EchoExecuteCommand(Action<string> executeAction)
        {
            this.executeAction = executeAction;
        }

        public override IEnumerable<string> AliasSelectors => new string[0];

        public override string HelpText => string.Empty;

        public override string Name => string.Empty;

        public override ICommand Parent { get; set; }

        public override string Path => Parent?.PrimarySelector ?? string.Empty;

        public override string PrimarySelector => "EchoExecute";

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