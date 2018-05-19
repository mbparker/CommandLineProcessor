namespace CommandLineProcessorContracts
{
    using System;
    using System.Collections.Generic;

    public interface IInputCommandRegistration : ICommandRegistration
    {
        IContainerCommandRegistration SetChildToContainerCommand(
            ICommandDescriptor descriptor,
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc);

        IExecutableCommandRegistration SetChildToExecutableCommand(
            ICommandDescriptor descriptor,
            Action<ICommandContext> executeAction);

        IInputCommandRegistration SetChildToInputCommand(
            ICommandDescriptor descriptor,
            string promptText,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc);
    }
}