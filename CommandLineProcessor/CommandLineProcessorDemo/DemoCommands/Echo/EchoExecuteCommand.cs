namespace CommandLineProcessorDemo.DemoCommands.Echo
{
    using System;

    using CommandLineProcessorContracts;

    public class EchoExecuteCommand : BaseCommand, IExecutableCommand
    {
        private readonly Action<string> executeAction;

        public EchoExecuteCommand(Action<string> executeAction)
        {
            this.executeAction = executeAction;
        }

        public override string PrimarySelector => string.Empty;

        public void Execute(ICommandContext context, params object[] args)
        {
            executeAction?.Invoke((string)args[0]);
        }

        public object[] GetArguments(ICommandContext context)
        {
            return new[] { context.DataStore["TEXT"] };
        }
    }
}