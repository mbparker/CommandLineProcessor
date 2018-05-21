namespace CommandLineProcessorContracts.Commands.Registration
{
    using System;
    using System.Collections.Generic;

    public interface IInputCommandRegistration : ICommandRegistration
    {
        IContainerCommandRegistration SetChildToContainerCommand(
            string primarySelector,
            string[] aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc);

        IContainerCommandRegistration SetChildToContainerCommand(
            string primarySelector,
            string[] aliasSelectors,
            string name,
            string helpText);

        IContainerCommandRegistration SetChildToContainerCommand(
            string primarySelector,
            string name,
            string helpText,
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc);

        IContainerCommandRegistration SetChildToContainerCommand(string primarySelector, string name, string helpText);

        IContainerCommandRegistration SetChildToContainerCommand();

        IExecutableCommandRegistration SetChildToExecutableCommand(
            string primarySelector,
            string[] aliasSelectors,
            string name,
            string helpText,
            Action<ICommandContext> executeAction);

        IExecutableCommandRegistration SetChildToExecutableCommand(
            string primarySelector,
            string name,
            string helpText,
            Action<ICommandContext> executeAction);

        IExecutableCommandRegistration SetChildToExecutableCommand(Action<ICommandContext> executeAction);

        IInputCommandRegistration SetChildToInputCommand(
            string primarySelector,
            string[] aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc);

        IInputCommandRegistration SetChildToInputCommand(
            string primarySelector,
            string[] aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction);

        IInputCommandRegistration SetChildToInputCommand(
            string primarySelector,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc);

        IInputCommandRegistration SetChildToInputCommand(
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc);

        IInputCommandRegistration SetChildToInputCommand(
            string primarySelector,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction);

        IInputCommandRegistration SetChildToInputCommand(
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction);
    }
}