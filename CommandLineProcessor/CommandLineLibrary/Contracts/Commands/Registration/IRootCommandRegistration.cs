﻿namespace CommandLineLibrary.Contracts.Commands.Registration
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public interface IRootCommandRegistration : ICommandRegistration
    {
        IContainerCommandRegistration RegisterContainerCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc);

        IContainerCommandRegistration RegisterContainerCommand(
            string primarySelector,
            string name,
            string helpText,
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc);

        IContainerCommandRegistration RegisterContainerCommand<TCommand, TDescriptorContainer>(
            Func<TDescriptorContainer, ICommandDescriptor> getDescriptorFunc,
            Expression<Action<TCommand>> getDefaultCommandExpression,
            TCommand instance = null)
            where TCommand : class;

        IExecutableCommandRegistration RegisterExecutableCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
            string name,
            string helpText,
            Action<ICommandContext> executeAction);

        IExecutableCommandRegistration RegisterExecutableCommand(
            string primarySelector,
            string name,
            string helpText,
            Action<ICommandContext> executeAction);

        IExecutableCommandRegistration RegisterExecutableCommand<TCommand, TDescriptorContainer>(
            Func<TDescriptorContainer, ICommandDescriptor> getDescriptorFunc,
            Expression<Action<TCommand>> executeExpression,
            TCommand instance = null)
            where TCommand : class;

        IInputCommandRegistration RegisterInputCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc);

        IInputCommandRegistration RegisterInputCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction);

        IInputCommandRegistration RegisterInputCommand(
            string primarySelector,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc);

        IInputCommandRegistration RegisterInputCommand(
            string primarySelector,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction);

        IInputCommandRegistration RegisterInputCommand<TCommand, TDescriptorContainer>(
            Func<TDescriptorContainer, ICommandDescriptor> getDescriptorFunc,
            Expression<Action<TCommand>> getPromptTextExpression,
            Expression<Action<TCommand>> applyInputExpression,
            Expression<Action<TCommand>> getDefaultExpression,
            TCommand instance = null)
            where TCommand : class;

        IInputCommandRegistration RegisterInputCommand<TCommand, TDescriptorContainer>(
            Func<TDescriptorContainer, ICommandDescriptor> getDescriptorFunc,
            Expression<Action<TCommand>> getPromptTextExpression,
            Expression<Action<TCommand>> applyInputExpression,
            TCommand instance = null)
            where TCommand : class;
    }
}