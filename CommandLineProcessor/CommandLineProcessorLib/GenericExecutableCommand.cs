namespace CommandLineProcessorLib
{
    using System;

    using CommandLineProcessorContracts;

    public class GenericExecutableCommand : BaseCommand, IExecutableCommand
    {
        private readonly Action<ICommandContext, object[]> executeAction;

        private readonly Func<ICommandContext, object[]> getArgumentsFunc;

        public GenericExecutableCommand(
            ICommandDescriptor descriptor,
            Func<ICommandContext, object[]> getArgumentsFunc,
            Action<ICommandContext, object[]> executeAction)
            : base(descriptor)
        {
            this.getArgumentsFunc = getArgumentsFunc;
            this.executeAction = executeAction;
        }

        public void Execute(ICommandContext context, params object[] args)
        {
            executeAction?.Invoke(context, args);
        }

        public object[] GetArguments(ICommandContext context)
        {
            return getArgumentsFunc?.Invoke(context) ?? new object[0];
        }
    }
}