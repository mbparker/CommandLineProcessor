namespace CommandLineProcessorContracts
{
    using System;

    public interface IContainerCommandRegistration : ICommandRegistration
    {
        IContainerCommandRegistration AddContainerCommand(ICommandDescriptor descriptor);

        IExecutableCommandRegistration AddExecutableCommand(
            ICommandDescriptor descriptor,
            Func<ICommandContext, object[]> getArgumentsFunc,
            Action<ICommandContext, object[]> executeAction);

        IInputCommandRegistration AddInputCommand(
            ICommandDescriptor descriptor,
            string promptText,
            Action<ICommandContext, string> applyInputAction);
    }
}