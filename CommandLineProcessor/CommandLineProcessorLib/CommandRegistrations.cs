namespace CommandLineProcessorLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    using CommandLineProcessorCommon.Ioc;

    using CommandLineProcessorContracts;
    using CommandLineProcessorContracts.Commands;
    using CommandLineProcessorContracts.Commands.Registration;

    public class CommandRegistrations : IRootCommandRegistration,
                                        IContainerCommandRegistration,
                                        IExecutableCommandRegistration,
                                        IInputCommandRegistration
    {
        private readonly IIocContainer iocContainer;

        private readonly List<ICommand> registeredCommands;

        private readonly ICommand targetCommand;

        public CommandRegistrations(IIocContainer iocContainer)
            : this(new List<ICommand>(), null, iocContainer)
        {
        }

        protected CommandRegistrations(
            List<ICommand> registeredCommands,
            ICommand targetCommand,
            IIocContainer iocContainer)
        {
            this.registeredCommands = registeredCommands;
            this.targetCommand = targetCommand;
            this.iocContainer = iocContainer;
        }

        public IEnumerable<ICommand> RegisteredCommands => registeredCommands;

        public IContainerCommandRegistration AddContainerCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc)
        {
            var command = new GenericContainerCommand(
                primarySelector,
                aliasSelectors,
                name,
                helpText,
                getDefaultCommandFunc);
            command.Parent = targetCommand;
            (targetCommand as IContainerCommandEdit).AddChild(command);
            return new CommandRegistrations(registeredCommands, command, iocContainer);
        }

        public IContainerCommandRegistration AddContainerCommand(
            string primarySelector,
            string name,
            string helpText,
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc)
        {
            return AddContainerCommand(primarySelector, new string[0], name, helpText, getDefaultCommandFunc);
        }

        public IContainerCommandRegistration AddContainerCommand<TCommand, TDescriptorContainer>(
            Func<TDescriptorContainer, ICommandDescriptor> getDescriptorFunc,
            Expression<Action<TCommand>> getDefaultCommandExpression,
            TCommand instance = default(TCommand))
            where TCommand : class where TDescriptorContainer : class
        {
            var getDefaultCommandFunc = GetDefaultCommandFuncForContainerCommand(getDefaultCommandExpression, instance);

            var descriptorContainer = iocContainer.Resolve<TDescriptorContainer>();
            var descriptor = getDescriptorFunc(descriptorContainer);

            return AddContainerCommand(
                descriptor.PrimarySelector,
                descriptor.AliasSelectors,
                descriptor.Name,
                descriptor.Name,
                getDefaultCommandFunc);
        }

        public IExecutableCommandRegistration AddExecutableCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
            string name,
            string helpText,
            Action<ICommandContext> executeAction)
        {
            var command = new GenericExecutableCommand(primarySelector, aliasSelectors, name, helpText, executeAction);
            command.Parent = targetCommand;
            (targetCommand as IContainerCommandEdit).AddChild(command);
            return new CommandRegistrations(registeredCommands, command, iocContainer);
        }

        public IExecutableCommandRegistration AddExecutableCommand(
            string primarySelector,
            string name,
            string helpText,
            Action<ICommandContext> executeAction)
        {
            return AddExecutableCommand(primarySelector, new string[0], name, helpText, executeAction);
        }

        public IExecutableCommandRegistration AddExecutableCommand<TCommand, TDescriptorContainer>(
            Func<TDescriptorContainer, ICommandDescriptor> getDescriptorFunc,
            Expression<Action<TCommand>> executeExpression,
            TCommand instance = default(TCommand))
            where TCommand : class where TDescriptorContainer : class
        {
            var executeAction = GetExecuteActionForExecuteCommand(executeExpression, instance);

            var descriptorContainer = iocContainer.Resolve<TDescriptorContainer>();
            var descriptor = getDescriptorFunc(descriptorContainer);

            return AddExecutableCommand(
                descriptor.PrimarySelector,
                descriptor.AliasSelectors,
                descriptor.Name,
                descriptor.HelpText,
                executeAction);
        }

        public IInputCommandRegistration AddInputCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc)
        {
            var command = new GenericInputCommand(
                primarySelector,
                aliasSelectors,
                name,
                helpText,
                getPromptTextFunc,
                applyInputAction,
                getDefaultFunc);
            command.Parent = targetCommand;
            (targetCommand as IContainerCommandEdit).AddChild(command);
            return new CommandRegistrations(registeredCommands, command, iocContainer);
        }

        public IInputCommandRegistration AddInputCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction)
        {
            return AddInputCommand(
                primarySelector,
                aliasSelectors,
                name,
                helpText,
                getPromptTextFunc,
                applyInputAction,
                null);
        }

        public IInputCommandRegistration AddInputCommand(
            string primarySelector,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc)
        {
            return AddInputCommand(
                primarySelector,
                new string[0],
                name,
                helpText,
                getPromptTextFunc,
                applyInputAction,
                getDefaultFunc);
        }

        public IInputCommandRegistration AddInputCommand(
            string primarySelector,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction)
        {
            return AddInputCommand(
                primarySelector,
                new string[0],
                name,
                helpText,
                getPromptTextFunc,
                applyInputAction,
                null);
        }

        public IInputCommandRegistration AddInputCommand<TCommand, TDescriptorContainer>(
            Func<TDescriptorContainer, ICommandDescriptor> getDescriptorFunc,
            Expression<Action<TCommand>> getPromptTextExpression,
            Expression<Action<TCommand>> applyInputExpression,
            Expression<Action<TCommand>> getDefaultExpression,
            TCommand instance = default(TCommand))
            where TCommand : class where TDescriptorContainer : class
        {
            var getPromptTextFunc = GetPromptTextFuncForInputCommand(getDefaultExpression, instance);
            var applyInputAction = GetApplyInputActionForInputCommand(applyInputExpression, instance);
            var getDefaultFunc = GetDefaultFuncForInputCommand(getDefaultExpression, instance);

            var descriptorContainer = iocContainer.Resolve<TDescriptorContainer>();
            var descriptor = getDescriptorFunc(descriptorContainer);

            return AddInputCommand(
                descriptor.PrimarySelector,
                descriptor.AliasSelectors,
                descriptor.Name,
                descriptor.HelpText,
                getPromptTextFunc,
                applyInputAction,
                getDefaultFunc);
        }

        public IInputCommandRegistration AddInputCommand<TCommand, TDescriptorContainer>(
            Func<TDescriptorContainer, ICommandDescriptor> getDescriptorFunc,
            Expression<Action<TCommand>> getPromptTextExpression,
            Expression<Action<TCommand>> applyInputExpression,
            TCommand instance = default(TCommand))
            where TCommand : class where TDescriptorContainer : class
        {
            return AddInputCommand(getDescriptorFunc, getPromptTextExpression, applyInputExpression, null, instance);
        }

        public IContainerCommandRegistration RegisterContainerCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc)
        {
            var command = new GenericContainerCommand(
                primarySelector,
                aliasSelectors,
                name,
                helpText,
                getDefaultCommandFunc);
            registeredCommands.Add(command);
            return new CommandRegistrations(registeredCommands, command, iocContainer);
        }

        public IContainerCommandRegistration RegisterContainerCommand(
            string primarySelector,
            string name,
            string helpText,
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc)
        {
            return RegisterContainerCommand(primarySelector, new string[0], name, helpText, getDefaultCommandFunc);
        }

        public IContainerCommandRegistration RegisterContainerCommand<TCommand, TDescriptorContainer>(
            Func<TDescriptorContainer, ICommandDescriptor> getDescriptorFunc,
            Expression<Action<TCommand>> getDefaultCommandExpression,
            TCommand instance = default(TCommand))
            where TCommand : class where TDescriptorContainer : class
        {
            var getDefaultCommandFunc = GetDefaultCommandFuncForContainerCommand(getDefaultCommandExpression, instance);

            var descriptorContainer = iocContainer.Resolve<TDescriptorContainer>();
            var descriptor = getDescriptorFunc(descriptorContainer);

            return RegisterContainerCommand(
                descriptor.PrimarySelector,
                descriptor.AliasSelectors,
                descriptor.Name,
                descriptor.Name,
                getDefaultCommandFunc);
        }

        public IExecutableCommandRegistration RegisterExecutableCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
            string name,
            string helpText,
            Action<ICommandContext> executeAction)
        {
            var command = new GenericExecutableCommand(primarySelector, aliasSelectors, name, helpText, executeAction);
            registeredCommands.Add(command);
            return new CommandRegistrations(registeredCommands, command, iocContainer);
        }

        public IExecutableCommandRegistration RegisterExecutableCommand(
            string primarySelector,
            string name,
            string helpText,
            Action<ICommandContext> executeAction)
        {
            return RegisterExecutableCommand(primarySelector, new string[0], name, helpText, executeAction);
        }

        public IExecutableCommandRegistration RegisterExecutableCommand<TCommand, TDescriptorContainer>(
            Func<TDescriptorContainer, ICommandDescriptor> getDescriptorFunc,
            Expression<Action<TCommand>> executeExpression,
            TCommand instance = null)
            where TCommand : class where TDescriptorContainer : class
        {
            var executeAction = GetExecuteActionForExecuteCommand(executeExpression, instance);

            var descriptorContainer = iocContainer.Resolve<TDescriptorContainer>();
            var descriptor = getDescriptorFunc(descriptorContainer);

            return RegisterExecutableCommand(
                descriptor.PrimarySelector,
                descriptor.AliasSelectors,
                descriptor.Name,
                descriptor.HelpText,
                executeAction);
        }

        public IInputCommandRegistration RegisterInputCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc)
        {
            var command = new GenericInputCommand(
                primarySelector,
                aliasSelectors,
                name,
                helpText,
                getPromptTextFunc,
                applyInputAction,
                getDefaultFunc);
            registeredCommands.Add(command);
            return new CommandRegistrations(registeredCommands, command, iocContainer);
        }

        public IInputCommandRegistration RegisterInputCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction)
        {
            return RegisterInputCommand(
                primarySelector,
                aliasSelectors,
                name,
                helpText,
                getPromptTextFunc,
                applyInputAction,
                null);
        }

        public IInputCommandRegistration RegisterInputCommand(
            string primarySelector,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc)
        {
            return RegisterInputCommand(
                primarySelector,
                new string[0],
                name,
                helpText,
                getPromptTextFunc,
                applyInputAction,
                getDefaultFunc);
        }

        public IInputCommandRegistration RegisterInputCommand(
            string primarySelector,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction)
        {
            return RegisterInputCommand(primarySelector, name, helpText, getPromptTextFunc, applyInputAction, null);
        }

        public IInputCommandRegistration RegisterInputCommand<TCommand, TDescriptorContainer>(
            Func<TDescriptorContainer, ICommandDescriptor> getDescriptorFunc,
            Expression<Action<TCommand>> getPromptTextExpression,
            Expression<Action<TCommand>> applyInputExpression,
            Expression<Action<TCommand>> getDefaultExpression,
            TCommand instance = null)
            where TCommand : class where TDescriptorContainer : class
        {
            var getPromptTextFunc = GetPromptTextFuncForInputCommand(getDefaultExpression, instance);
            var applyInputAction = GetApplyInputActionForInputCommand(applyInputExpression, instance);
            var getDefaultFunc = GetDefaultFuncForInputCommand(getDefaultExpression, instance);

            var descriptorContainer = iocContainer.Resolve<TDescriptorContainer>();
            var descriptor = getDescriptorFunc(descriptorContainer);

            return RegisterInputCommand(
                descriptor.PrimarySelector,
                descriptor.AliasSelectors,
                descriptor.Name,
                descriptor.HelpText,
                getPromptTextFunc,
                applyInputAction,
                getDefaultFunc);
        }

        public IInputCommandRegistration RegisterInputCommand<TCommand, TDescriptorContainer>(
            Func<TDescriptorContainer, ICommandDescriptor> getDescriptorFunc,
            Expression<Action<TCommand>> getPromptTextExpression,
            Expression<Action<TCommand>> applyInputExpression,
            TCommand instance = default(TCommand))
            where TCommand : class where TDescriptorContainer : class
        {
            return RegisterInputCommand(
                getDescriptorFunc,
                getPromptTextExpression,
                applyInputExpression,
                null,
                instance);
        }

        public IContainerCommandRegistration SetChildToContainerCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc)
        {
            var command = new GenericContainerCommand(
                primarySelector,
                aliasSelectors,
                name,
                helpText,
                getDefaultCommandFunc);
            command.Parent = targetCommand;
            (targetCommand as IInputCommand).NextCommand = command;
            return new CommandRegistrations(registeredCommands, command, iocContainer);
        }

        public IContainerCommandRegistration SetChildToContainerCommand(
            string primarySelector,
            string name,
            string helpText,
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc)
        {
            return SetChildToContainerCommand(primarySelector, new string[0], name, helpText, getDefaultCommandFunc);
        }

        public IContainerCommandRegistration SetChildToContainerCommand(
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc)
        {
            return SetChildToContainerCommand(null, null, null, getDefaultCommandFunc);
        }

        public IContainerCommandRegistration SetChildToContainerCommand<TCommand, TDescriptorContainer>(
            Func<TDescriptorContainer, ICommandDescriptor> getDescriptorFunc,
            Expression<Action<TCommand>> getDefaultCommandExpression,
            TCommand instance = default(TCommand))
            where TCommand : class where TDescriptorContainer : class
        {
            var getDefaultCommandFunc = GetDefaultCommandFuncForContainerCommand(getDefaultCommandExpression, instance);

            ICommandDescriptor descriptor = null;
            if (getDescriptorFunc != null)
            {
                var descriptorContainer = iocContainer.Resolve<TDescriptorContainer>();
                descriptor = getDescriptorFunc(descriptorContainer);
            }

            return SetChildToContainerCommand(
                descriptor?.PrimarySelector,
                descriptor?.AliasSelectors,
                descriptor?.Name,
                descriptor?.HelpText,
                getDefaultCommandFunc);
        }

        public IContainerCommandRegistration SetChildToContainerCommand<TCommand>(
            Expression<Action<TCommand>> getDefaultCommandExpression,
            TCommand instance = default(TCommand))
            where TCommand : class
        {
            return SetChildToContainerCommand<TCommand, TCommand>(null, getDefaultCommandExpression, instance);
        }

        public IExecutableCommandRegistration SetChildToExecutableCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
            string name,
            string helpText,
            Action<ICommandContext> executeAction)
        {
            var command = new GenericExecutableCommand(primarySelector, aliasSelectors, name, helpText, executeAction);
            command.Parent = targetCommand;
            (targetCommand as IInputCommand).NextCommand = command;
            return new CommandRegistrations(registeredCommands, command, iocContainer);
        }

        public IExecutableCommandRegistration SetChildToExecutableCommand(
            string primarySelector,
            string name,
            string helpText,
            Action<ICommandContext> executeAction)
        {
            return SetChildToExecutableCommand(primarySelector, new string[0], name, helpText, executeAction);
        }

        public IExecutableCommandRegistration SetChildToExecutableCommand(Action<ICommandContext> executeAction)
        {
            return SetChildToExecutableCommand(null, null, null, executeAction);
        }

        public IExecutableCommandRegistration SetChildToExecutableCommand<TCommand, TDescriptorContainer>(
            Func<TDescriptorContainer, ICommandDescriptor> getDescriptorFunc,
            Expression<Action<TCommand>> executeExpression,
            TCommand instance = default(TCommand))
            where TCommand : class where TDescriptorContainer : class
        {
            var executeAction = GetExecuteActionForExecuteCommand(executeExpression, instance);

            ICommandDescriptor descriptor = null;
            if (getDescriptorFunc != null)
            {
                var descriptorContainer = iocContainer.Resolve<TDescriptorContainer>();
                descriptor = getDescriptorFunc(descriptorContainer);
            }

            return SetChildToExecutableCommand(
                descriptor?.PrimarySelector,
                descriptor?.AliasSelectors,
                descriptor?.Name,
                descriptor?.HelpText,
                executeAction);
        }

        public IExecutableCommandRegistration SetChildToExecutableCommand<TCommand>(
            Expression<Action<TCommand>> executeExpression,
            TCommand instance = default(TCommand))
            where TCommand : class
        {
            return SetChildToExecutableCommand<TCommand, TCommand>(null, executeExpression, instance);
        }

        public IInputCommandRegistration SetChildToInputCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc)
        {
            var command = new GenericInputCommand(
                primarySelector,
                aliasSelectors,
                name,
                helpText,
                getPromptTextFunc,
                applyInputAction,
                getDefaultFunc);
            command.Parent = targetCommand;
            (targetCommand as IInputCommand).NextCommand = command;
            return new CommandRegistrations(registeredCommands, command, iocContainer);
        }

        public IInputCommandRegistration SetChildToInputCommand(
            string primarySelector,
            IEnumerable<string> aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction)
        {
            return SetChildToInputCommand(
                primarySelector,
                aliasSelectors,
                name,
                helpText,
                getPromptTextFunc,
                applyInputAction,
                null);
        }

        public IInputCommandRegistration SetChildToInputCommand(
            string primarySelector,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc)
        {
            return SetChildToInputCommand(
                primarySelector,
                new string[0],
                name,
                helpText,
                getPromptTextFunc,
                applyInputAction,
                getDefaultFunc);
        }

        public IInputCommandRegistration SetChildToInputCommand(
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc)
        {
            return SetChildToInputCommand(null, null, null, getPromptTextFunc, applyInputAction, getDefaultFunc);
        }

        public IInputCommandRegistration SetChildToInputCommand(
            string primarySelector,
            string name,
            string helpText,
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction)
        {
            return SetChildToInputCommand(primarySelector, name, helpText, getPromptTextFunc, applyInputAction, null);
        }

        public IInputCommandRegistration SetChildToInputCommand(
            Func<ICommandContext, string> getPromptTextFunc,
            Action<ICommandContext, string> applyInputAction)
        {
            return SetChildToInputCommand(null, null, null, getPromptTextFunc, applyInputAction, null);
        }

        public IInputCommandRegistration SetChildToInputCommand<TCommand, TDescriptorContainer>(
            Func<TDescriptorContainer, ICommandDescriptor> getDescriptorFunc,
            Expression<Action<TCommand>> getPromptTextExpression,
            Expression<Action<TCommand>> applyInputExpression,
            Expression<Action<TCommand>> getDefaultExpression,
            TCommand instance = default(TCommand))
            where TCommand : class where TDescriptorContainer : class
        {
            var getPromptTextFunc = GetPromptTextFuncForInputCommand(getDefaultExpression, instance);
            var applyInputAction = GetApplyInputActionForInputCommand(applyInputExpression, instance);
            var getDefaultFunc = GetDefaultFuncForInputCommand(getDefaultExpression, instance);

            ICommandDescriptor descriptor = null;
            if (getDescriptorFunc != null)
            {
                var descriptorContainer = iocContainer.Resolve<TDescriptorContainer>();
                descriptor = getDescriptorFunc(descriptorContainer);
            }

            return SetChildToInputCommand(
                descriptor?.PrimarySelector,
                descriptor?.AliasSelectors,
                descriptor?.Name,
                descriptor?.HelpText,
                getPromptTextFunc,
                applyInputAction,
                getDefaultFunc);
        }

        public IInputCommandRegistration SetChildToInputCommand<TCommand>(
            Expression<Action<TCommand>> getPromptTextExpression,
            Expression<Action<TCommand>> applyInputExpression,
            Expression<Action<TCommand>> getDefaultExpression,
            TCommand instance = default(TCommand))
            where TCommand : class
        {
            return SetChildToInputCommand<TCommand, TCommand>(
                null,
                getPromptTextExpression,
                applyInputExpression,
                getDefaultExpression,
                instance);
        }

        public IInputCommandRegistration SetChildToInputCommand<TCommand>(
            Expression<Action<TCommand>> getPromptTextExpression,
            Expression<Action<TCommand>> applyInputExpression,
            TCommand instance = default(TCommand))
            where TCommand : class
        {
            return SetChildToInputCommand(getPromptTextExpression, applyInputExpression, null, instance);
        }

        private Action<ICommandContext, string> GetApplyInputActionForInputCommand<TCommand>(
            Expression<Action<TCommand>> applyInputExpression,
            TCommand instance)
            where TCommand : class
        {
            if (applyInputExpression == null)
            {
                throw new ArgumentNullException(nameof(applyInputExpression));
            }

            if (applyInputExpression.Body is MethodCallExpression applyInputMethodExpression)
            {
                if (!(applyInputMethodExpression.Arguments.Count == 2
                      && applyInputMethodExpression.Arguments[0].Type.IsAssignableFrom(typeof(ICommandContext))
                      && applyInputMethodExpression.Arguments[1].Type.IsAssignableFrom(typeof(string))))
                {
                    throw new ArgumentException(
                        $"Method selected in expression does not have a compatible signature. Expected: void Foo(ICommandContext context, string input)",
                        nameof(applyInputExpression));
                }

                return (context, input) =>
                    {
                        var target = instance ?? context.GetService<TCommand>();
                        applyInputMethodExpression.Method.Invoke(target, new object[] { context, input });
                    };
            }

            throw new ArgumentException(
                $"Expression must be a {nameof(MethodCallExpression)}",
                nameof(applyInputExpression));
        }

        private Func<ICommandContext, IEnumerable<ICommand>, ICommand>
            GetDefaultCommandFuncForContainerCommand<TCommand>(
                Expression<Action<TCommand>> getDefaultCommandExpression,
                TCommand instance)
            where TCommand : class
        {
            if (getDefaultCommandExpression == null)
            {
                throw new ArgumentNullException(nameof(getDefaultCommandExpression));
            }

            if (getDefaultCommandExpression.Body is MethodCallExpression getDefaultCommandMethodExpression)
            {
                if (!(getDefaultCommandMethodExpression.Arguments.Count == 2
                      && getDefaultCommandMethodExpression.Arguments[0].Type.IsAssignableFrom(typeof(ICommandContext))
                      && getDefaultCommandMethodExpression.Arguments[1].Type
                          .IsAssignableFrom(typeof(IEnumerable<ICommand>))
                      && getDefaultCommandMethodExpression.Method.ReturnType.IsAssignableFrom(typeof(ICommand))))
                {
                    throw new ArgumentException(
                        $"Method selected in expression does not have a compatible signature. Expected: ICommand Foo(ICommandContext context, IEnumerable<ICommand> commands)",
                        nameof(getDefaultCommandExpression));
                }

                return (context, commands) =>
                    {
                        var target = instance ?? context.GetService<TCommand>();
                        return (ICommand)getDefaultCommandMethodExpression.Method.Invoke(
                            target,
                            new object[] { context, commands });
                    };
            }

            throw new ArgumentException(
                $"Expression must be a {nameof(MethodCallExpression)}",
                nameof(getDefaultCommandExpression));
        }

        private Func<ICommandContext, string> GetDefaultFuncForInputCommand<TCommand>(
            Expression<Action<TCommand>> getDefaultExpression,
            TCommand instance)
            where TCommand : class
        {
            if (getDefaultExpression != null)
            {
                if (getDefaultExpression.Body is MethodCallExpression getDefaultMethodExpression)
                {
                    if (!(getDefaultMethodExpression.Arguments.Count == 1
                          && getDefaultMethodExpression.Arguments[0].Type.IsAssignableFrom(typeof(ICommandContext))
                          && getDefaultMethodExpression.Method.ReturnType.IsAssignableFrom(typeof(string))))
                    {
                        throw new ArgumentException(
                            $"Method selected in expression does not have a compatible signature. Expected: string Foo(ICommandContext context)",
                            nameof(getDefaultExpression));
                    }

                    return context =>
                        {
                            var target = instance ?? context.GetService<TCommand>();
                            return (string)getDefaultMethodExpression.Method.Invoke(target, new object[] { context });
                        };
                }

                throw new ArgumentException(
                    $"Expression must be a {nameof(MethodCallExpression)}",
                    nameof(getDefaultExpression));
            }

            return null;
        }

        private Action<ICommandContext> GetExecuteActionForExecuteCommand<TCommand>(
            Expression<Action<TCommand>> executeExpression,
            TCommand instance)
            where TCommand : class
        {
            if (executeExpression == null)
            {
                throw new ArgumentNullException(nameof(executeExpression));
            }

            if (executeExpression.Body is MethodCallExpression executeMethodExpression)
            {
                if (!(executeMethodExpression.Arguments.Count == 1 && executeMethodExpression.Arguments[0].Type
                          .IsAssignableFrom(typeof(ICommandContext))))
                {
                    throw new ArgumentException(
                        $"Method selected in expression does not have a compatible signature. Expected: void Foo(ICommandContext context)",
                        nameof(executeExpression));
                }

                return context =>
                    {
                        var target = instance ?? context.GetService<TCommand>();
                        executeMethodExpression.Method.Invoke(target, new object[] { context });
                    };
            }

            throw new ArgumentException(
                $"Expression must be a {nameof(MethodCallExpression)}",
                nameof(executeExpression));
        }

        private Func<ICommandContext, string> GetPromptTextFuncForInputCommand<TCommand>(
            Expression<Action<TCommand>> getPromptTextExpression,
            TCommand instance)
            where TCommand : class
        {
            if (getPromptTextExpression == null)
            {
                throw new ArgumentNullException(nameof(getPromptTextExpression));
            }

            if (getPromptTextExpression.Body is MethodCallExpression getPromptTextMethodExpression)
            {
                if (!(getPromptTextMethodExpression.Arguments.Count == 1
                      && getPromptTextMethodExpression.Arguments[0].Type.IsAssignableFrom(typeof(ICommandContext))
                      && getPromptTextMethodExpression.Method.ReturnType.IsAssignableFrom(typeof(string))))
                {
                    throw new ArgumentException(
                        $"Method selected in expression does not have a compatible signature. Expected: string Foo(ICommandContext context)",
                        nameof(getPromptTextExpression));
                }

                return context =>
                    {
                        var target = instance ?? context.GetService<TCommand>();
                        return (string)getPromptTextMethodExpression.Method.Invoke(target, new object[] { context });
                    };
            }

            throw new ArgumentException(
                $"Expression must be a {nameof(MethodCallExpression)}",
                nameof(getPromptTextExpression));
        }

        private MethodInfo ValidateMethodCallExpressionAndReturnMethodInfo<TValidate, TExpected>(
            Expression<Action<TValidate>> expressionToValidate,
            Expression<Action<TExpected>> knownCompatibleExpression)
        {
            if (expressionToValidate == null)
            {
                throw new ArgumentNullException(nameof(expressionToValidate));
            }

            if (knownCompatibleExpression == null)
            {
                throw new ArgumentNullException(nameof(knownCompatibleExpression));
            }

            if (!(expressionToValidate.Body is MethodCallExpression methodExpressionToValidate))
            {
                throw new ArgumentException(
                    $"Expression must be a {nameof(MethodCallExpression)}",
                    nameof(expressionToValidate));
            }

            if (!(knownCompatibleExpression.Body is MethodCallExpression knownGoodMethodExpression))
            {
                throw new ArgumentException(
                    $"Expression must be a {nameof(MethodCallExpression)}",
                    nameof(knownCompatibleExpression));
            }

            bool signatureMatches = true;            
            if (methodExpressionToValidate.Arguments.Count == knownGoodMethodExpression.Arguments.Count)
            {                
                for (int i = 0; i < knownGoodMethodExpression.Arguments.Count; i++)
                {
                    if (!methodExpressionToValidate.Arguments[i].Type
                            .IsAssignableFrom(knownGoodMethodExpression.Arguments[i].Type))
                    {
                        signatureMatches = false;
                        break;
                    }
                }
            }

            if (signatureMatches)
            {
                signatureMatches =
                    methodExpressionToValidate.Method.ReturnType.IsAssignableFrom(
                        knownGoodMethodExpression.Method.ReturnType);
            }

            if (signatureMatches)
            {
                return methodExpressionToValidate.Method;
            }

            throw new ArgumentException(
                $"Method selected in expression does not have a compatible signature. Expected: {GetMethodSignatureText(knownGoodMethodExpression)} but received {GetMethodSignatureText(methodExpressionToValidate)}",
                nameof(expressionToValidate));            
        }

        private string GetMethodSignatureText(MethodCallExpression methodExpression)
        {
            throw new NotImplementedException();
        }

        private class ExpectedMethodSignatures
        {
            public void ApplyInput(ICommandContext context, string input)
            {
            }

            public string GetPromptText(ICommandContext context)
            {
                return null;
            }

            public void Execute(ICommandContext context)
            {
            }

            public string GetDefaultText(ICommandContext context)
            {
                return null;
            }

            public ICommand GetDefaultCommand(ICommandContext context, IEnumerable<ICommand> commands)
            {
                return null;
            }
        }
    }
}