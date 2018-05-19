namespace CommandLineProcessorContracts
{
    using System;
    using System.Collections.Generic;

    public interface IRootCommandRegistration : ICommandRegistration
    {
        IContainerCommandRegistration RegisterContainerCommand(
            ICommandDescriptor descriptor,
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc);

        IExecutableCommandRegistration RegisterExecutableCommand(
            ICommandDescriptor descriptor,
            Action<ICommandContext> executeAction);

        IInputCommandRegistration RegisterInputCommand(
            ICommandDescriptor descriptor,
            string promptText,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc);
    }
}