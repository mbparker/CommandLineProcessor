namespace CommandLineProcessorDemo.DemoCommands
{
    using System;
    using System.Collections.Generic;

    using CommandLineProcessorContracts;

    public class ExitCommand : BaseCommand, IExecutableCommand
    {
        private readonly Action exitAction;

        public ExitCommand(Action exitAction)
        {
            this.exitAction = exitAction;
        }

        public override IEnumerable<string> AliasSelectors => new[] { "EX" };

        public override string HelpText => "Terminates the program immediately";

        public override string Name => "Exit Application";

        public override string PrimarySelector => "EXit";

        public void Execute(ICommandContext context, params object[] args)
        {
            exitAction();
        }

        public object[] GetArguments(ICommandContext context)
        {
            return new object[0];
        }
    }
}