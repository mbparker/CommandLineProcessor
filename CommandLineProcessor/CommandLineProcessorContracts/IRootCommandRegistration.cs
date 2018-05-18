namespace CommandLineProcessorContracts
{
    using System;

    public interface IRootCommandRegistration : ICommandRegistration
    {
        IContainerCommandRegistration RegisterContainerCommand(ICommandDescriptor descriptor);

        IExecutableCommandRegistration RegisterExecutableCommand(
            ICommandDescriptor descriptor,
            Func<ICommandContext, object[]> getArgumentsFunc,
            Action<ICommandContext, object[]> executeAction);

        IInputCommandRegistration RegisterInputCommand(
            ICommandDescriptor descriptor,
            string promptText,
            Action<ICommandContext, string> applyInputAction);
    }
}