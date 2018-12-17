namespace CommandLineLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using CommandLineLibrary.Contracts;
    using CommandLineLibrary.Contracts.Commands;

    public class CommandMethodFactoryProvider : ICommandMethodFactoryService
    {
        private readonly IMethodCallValidatorService methodValidator;

        public CommandMethodFactoryProvider(IMethodCallValidatorService methodValidator)
        {
            this.methodValidator = methodValidator;
        }

        public interface ICommandDelegateMethodSignatures
        {
            void ApplyInput(ICommandContext context, string inputText);

            void Execute(ICommandContext context);

            ICommand GetDefaultCommand(ICommandContext context, IEnumerable<ICommand> commands);

            string GetDefaultValue(ICommandContext context);

            string GetPromptText(ICommandContext context);
        }

        public Action<ICommandContext, string> CreateApplyInputActionForInputCommand<TCommand>(
            Expression<Action<TCommand>> applyInputExpression,
            TCommand instance)
            where TCommand : class
        {
            var method =
                methodValidator
                    .ValidateMethodCallExpressionAndReturnMethodInfo<TCommand, ICommandDelegateMethodSignatures>(
                        applyInputExpression,
                        x => x.ApplyInput(null, null));

            return (context, input) =>
                {
                    var target = instance ?? context.GetService<TCommand>();
                    method.Invoke(target, new object[] { context, input });
                };
        }

        public Func<ICommandContext, IEnumerable<ICommand>, ICommand>
            CreateDefaultCommandFuncForContainerCommand<TCommand>(
                Expression<Action<TCommand>> getDefaultCommandExpression,
                TCommand instance)
            where TCommand : class
        {
            var method =
                methodValidator
                    .ValidateMethodCallExpressionAndReturnMethodInfo<TCommand, ICommandDelegateMethodSignatures>(
                        getDefaultCommandExpression,
                        x => x.GetDefaultCommand(null, null));

            return (context, commands) =>
                {
                    var target = instance ?? context.GetService<TCommand>();
                    return (ICommand)method.Invoke(target, new object[] { context, commands });
                };
        }

        public Func<ICommandContext, string> CreateDefaultFuncForInputCommand<TCommand>(
            Expression<Action<TCommand>> getDefaultExpression,
            TCommand instance)
            where TCommand : class
        {
            if (getDefaultExpression != null)
            {
                var method =
                    methodValidator
                        .ValidateMethodCallExpressionAndReturnMethodInfo<TCommand, ICommandDelegateMethodSignatures>(
                            getDefaultExpression,
                            x => x.GetDefaultValue(null));

                return context =>
                    {
                        var target = instance ?? context.GetService<TCommand>();
                        return (string)method.Invoke(target, new object[] { context });
                    };
            }

            return null;
        }

        public Action<ICommandContext> CreateExecuteActionForExecuteCommand<TCommand>(
            Expression<Action<TCommand>> executeExpression,
            TCommand instance)
            where TCommand : class
        {
            var method =
                methodValidator
                    .ValidateMethodCallExpressionAndReturnMethodInfo<TCommand, ICommandDelegateMethodSignatures>(
                        executeExpression,
                        x => x.Execute(null));

            return context =>
                {
                    var target = instance ?? context.GetService<TCommand>();
                    method.Invoke(target, new object[] { context });
                };
        }

        public Func<ICommandContext, string> CreatePromptTextFuncForInputCommand<TCommand>(
            Expression<Action<TCommand>> getPromptTextExpression,
            TCommand instance)
            where TCommand : class
        {
            var method =
                methodValidator
                    .ValidateMethodCallExpressionAndReturnMethodInfo<TCommand, ICommandDelegateMethodSignatures>(
                        getPromptTextExpression,
                        x => x.GetPromptText(null));

            return context =>
                {
                    var target = instance ?? context.GetService<TCommand>();
                    return (string)method.Invoke(target, new object[] { context });
                };
        }
    }
}