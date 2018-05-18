namespace CommandLineProcessorContracts
{
    using System;

    public interface IInputCommandRegistration : ICommandRegistration
    {
        IContainerCommandRegistration SetChildToContainerCommand(ICommandDescriptor descriptor);

        IExecutableCommandRegistration SetChildToExecutableCommand(
            ICommandDescriptor descriptor,
            Func<ICommandContext, object[]> getArgumentsFunc,
            Action<ICommandContext, object[]> executeAction);

        IInputCommandRegistration SetChildToInputCommand(
            ICommandDescriptor descriptor,
            string promptText,
            Action<ICommandContext, string> applyInputAction);
    }
}