namespace CommandLineProcessorContracts
{
    using System;

    public interface IContainerCommandRegistration : ICommandRegistration
    {
        IContainerCommandRegistration AddContainerCommand(ICommandDescriptor descriptor);

        IExecutableCommandRegistration AddExecutableCommand(
            ICommandDescriptor descriptor,
            Action<ICommandContext> executeAction);

        IInputCommandRegistration AddInputCommand(
            ICommandDescriptor descriptor,
            string promptText,
            Action<ICommandContext, string> applyInputAction);
    }
}