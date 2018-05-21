namespace CommandLineProcessorContracts.Commands.Registration
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public interface IInputCommandRegistration : ICommandRegistration
    {
        IContainerCommandRegistration SetChildToContainerCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc);

        IContainerCommandRegistration SetChildToContainerCommand(
            string primarySelector,
            string name,
            string helpText,
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc);

        IContainerCommandRegistration SetChildToContainerCommand(
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc);

        IContainerCommandRegistration SetChildToContainerCommand<TCommand, TDescriptorContainer>(
            Func<TDescriptorContainer, ICommandDescriptor> getDescriptorFunc,
            Expression<Action<TCommand>> getDefaultCommandExpression,
            TCommand instance = null)
            where TCommand : class where TDescriptorContainer : class;

        IContainerCommandRegistration SetChildToContainerCommand<TCommand>(
            Expression<Action<TCommand>> getDefaultCommandExpression,
            TCommand instance = null)
            where TCommand : class;

        IExecutableCommandRegistration SetChildToExecutableCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
            string name,
            string helpText,
            Action<ICommandContext> executeAction);

        IExecutableCommandRegistration SetChildToExecutableCommand(
            string primarySelector,
            string name,
            string helpText,
            Action<ICommandContext> executeAction);

        IExecutableCommandRegistration SetChildToExecutableCommand(Action<ICommandContext> executeAction);

        IExecutableCommandRegistration SetChildToExecutableCommand<TCommand, TDescriptorContainer>(
            Func<TDescriptorContainer, ICommandDescriptor> getDescriptorFunc,
            Expression<Action<TCommand>> executeExpression,
            TCommand instance = null)
            where TCommand : class where TDescriptorContainer : class;

        IExecutableCommandRegistration SetChildToExecutableCommand<TCommand>(
            Expression<Action<TCommand>> executeExpression,
            TCommand instance = null)
            where TCommand : class;

        IInputCommandRegistration SetChildToInputCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc);

        IInputCommandRegistration SetChildToInputCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
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

        IInputCommandRegistration SetChildToInputCommand<TCommand, TDescriptorContainer>(
            Func<TDescriptorContainer, ICommandDescriptor> getDescriptorFunc,
            Expression<Action<TCommand>> getPromptTextExpression,
            Expression<Action<TCommand>> applyInputExpression,
            Expression<Action<TCommand>> getDefaultExpression,
            TCommand instance = null)
            where TCommand : class where TDescriptorContainer : class;

        IInputCommandRegistration SetChildToInputCommand<TCommand>(
            Expression<Action<TCommand>> getPromptTextExpression,
            Expression<Action<TCommand>> applyInputExpression,
            Expression<Action<TCommand>> getDefaultExpression,
            TCommand instance = null)
            where TCommand : class;

        IInputCommandRegistration SetChildToInputCommand<TCommand>(
            Expression<Action<TCommand>> getPromptTextExpression,
            Expression<Action<TCommand>> applyInputExpression,
            TCommand instance = null)
            where TCommand : class;
    }
}