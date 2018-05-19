namespace CommandLineProcessorContracts.Commands.Registration
{
    using System;
    using System.Collections.Generic;

    using CommandLineProcessorContracts.Commands;

    public interface IContainerCommandRegistration : ICommandRegistration
    {
        IContainerCommandRegistration AddContainerCommand(
            ICommandDescriptor descriptor,
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc);

        IExecutableCommandRegistration AddExecutableCommand(
            ICommandDescriptor descriptor,
            Action<ICommandContext> executeAction);

        IInputCommandRegistration AddInputCommand(
            ICommandDescriptor descriptor,
            string promptText,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc);
    }
}