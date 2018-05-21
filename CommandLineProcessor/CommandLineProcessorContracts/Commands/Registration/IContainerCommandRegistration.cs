namespace CommandLineProcessorContracts.Commands.Registration
{
    using System;
    using System.Collections.Generic;

    public interface IContainerCommandRegistration : ICommandRegistration
    {
        IContainerCommandRegistration AddContainerCommand(
            string primarySelector,
            string[] aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc);

        IContainerCommandRegistration AddContainerCommand(
            string primarySelector,
            string[] aliasSelectors,
            string name,
            string helpText);

        IContainerCommandRegistration AddContainerCommand(
            string primarySelector,
            string name,
            string helpText,
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc);

        IContainerCommandRegistration AddContainerCommand(string primarySelector, string name, string helpText);

        IExecutableCommandRegistration AddExecutableCommand(
            string primarySelector,
            string[] aliasSelectors,
            string name,
            string helpText,
            Action<ICommandContext> executeAction);

        IExecutableCommandRegistration AddExecutableCommand(
            string primarySelector,
            string name,
            string helpText,
            Action<ICommandContext> executeAction);

        IInputCommandRegistration AddInputCommand(
            string primarySelector,
            string[] aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc);

        IInputCommandRegistration AddInputCommand(
            string primarySelector,
            string[] aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction);

        IInputCommandRegistration AddInputCommand(
            string primarySelector,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc);

        IInputCommandRegistration AddInputCommand(
            string primarySelector,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction);
    }
}