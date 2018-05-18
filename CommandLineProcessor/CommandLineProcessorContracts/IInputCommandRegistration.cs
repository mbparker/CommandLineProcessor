namespace CommandLineProcessorContracts
{
    using System;

    public interface IInputCommandRegistration : ICommandRegistration
    {
        IContainerCommandRegistration SetChildToContainerCommand(ICommandDescriptor descriptor);

        IExecutableCommandRegistration SetChildToExecutableCommand(
            ICommandDescriptor descriptor,
            Action<ICommandContext> executeAction);

        IInputCommandRegistration SetChildToInputCommand(
            ICommandDescriptor descriptor,
            string promptText,
            Action<ICommandContext, string> applyInputAction);
    }
}