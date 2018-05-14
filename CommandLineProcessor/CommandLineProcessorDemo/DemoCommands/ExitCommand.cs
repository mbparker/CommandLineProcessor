namespace CommandLineProcessorDemo.DemoCommands
{
    using System;
    using System.Collections.Generic;

    using CommandLineProcessorContracts;

    public class ExitCommand : BaseCommand, IInputCommand
    {
        private readonly Action exitAction;

        public ExitCommand(Action exitAction)
        {
            this.exitAction = exitAction;
        }

        public override IEnumerable<string> AliasSelectors => new[] { "EX" };

        public override string HelpText => "Terminates the program, after confirmation.";

        public override string Name => "Exit Application";

        public override string PrimarySelector => "EXit";

        public ICommand NextCommand => null;

        public string Prompt => "Are you sure you want to exit? (Y/N)";

        public void ApplyInput(ICommandContext context, string inputText)
        {
            if (inputText.ToUpper().StartsWith("Y"))
            {
                exitAction();
            }
        }
    }
}