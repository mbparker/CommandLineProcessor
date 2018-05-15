namespace CommandLineProcessorDemo.DemoCommands.Math.Multiply
{
    using System;

    using CommandLineProcessorContracts;

    public class MultiplyExecuteCommand : BaseCommand, IExecutableCommand
    {
        private readonly Action<double> writeAction;

        public MultiplyExecuteCommand(Action<double> writeAction)
        {
            this.writeAction = writeAction;
        }

        public override string PrimarySelector => string.Empty;

        public void Execute(ICommandContext context, params object[] args)
        {
            var n1 = (double)args[0];
            var n2 = (double)args[1];
            writeAction(n1 * n2);
        }

        public object[] GetArguments(ICommandContext context)
        {
            return new[] { context.DataStore["N1"], context.DataStore["N2"] };
        }
    }
}