namespace CommandLineProcessorLib
{
    using System;
    using System.Collections.Generic;

    using CommandLineProcessorContracts;
    using CommandLineProcessorContracts.Commands;

    public class GenericExecutableCommand : BaseCommand, IExecutableCommand
    {
        private readonly Action<ICommandContext> executeAction;

        public GenericExecutableCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
            string name,
            string helpText,
            Action<ICommandContext> executeAction)
            : base(primarySelector, aliasSelectors, name, helpText)
        {
            this.executeAction = executeAction;
        }

        public void Execute(ICommandContext context)
        {
            executeAction?.Invoke(context);
        }
    }
}