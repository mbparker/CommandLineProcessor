namespace CommandLineLibrary.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using CommandLineLibrary.Contracts.Commands;

    public interface ICommandMethodFactoryService
    {
        Action<ICommandContext, string> CreateApplyInputActionForInputCommand<TCommand>(
            Expression<Action<TCommand>> applyInputExpression,
            TCommand instance)
            where TCommand : class;

        Func<ICommandContext, IEnumerable<ICommand>, ICommand> CreateDefaultCommandFuncForContainerCommand<TCommand>(
            Expression<Action<TCommand>> getDefaultCommandExpression,
            TCommand instance)
            where TCommand : class;

        Func<ICommandContext, string> CreateDefaultFuncForInputCommand<TCommand>(
            Expression<Action<TCommand>> getDefaultExpression,
            TCommand instance)
            where TCommand : class;

        Action<ICommandContext> CreateExecuteActionForExecuteCommand<TCommand>(
            Expression<Action<TCommand>> executeExpression,
            TCommand instance)
            where TCommand : class;

        Func<ICommandContext, string> CreatePromptTextFuncForInputCommand<TCommand>(
            Expression<Action<TCommand>> getPromptTextExpression,
            TCommand instance)
            where TCommand : class;
    }
}