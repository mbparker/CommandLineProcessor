namespace CommandLineProcessorLib
{
    using System;

    using CommandLineProcessorContracts;

    public class GenericExecutableCommand : BaseCommand, IExecutableCommand
    {
        private readonly Action<ICommandContext> executeAction;

        public GenericExecutableCommand(
            ICommandDescriptor descriptor,
            Action<ICommandContext> executeAction)
            : base(descriptor)
        {
            this.executeAction = executeAction;
        }

        public void Execute(ICommandContext context)
        {
            executeAction?.Invoke(context);
        }
    }
}