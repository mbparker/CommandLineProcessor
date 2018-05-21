namespace CommandLineProcessorLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

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
            string[] aliasSelectors,
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
            string[] aliasSelectors,
            string name,
            string helpText)
        {
            return AddContainerCommand(primarySelector, aliasSelectors, name, helpText, null);
        }

        public IContainerCommandRegistration AddContainerCommand(
            string primarySelector,
            string name,
            string helpText,
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc)
        {
            return AddContainerCommand(primarySelector, new string[0], name, helpText, getDefaultCommandFunc);
        }

        public IContainerCommandRegistration AddContainerCommand(string primarySelector, string name, string helpText)
        {
            return AddContainerCommand(primarySelector, new string[0], name, helpText, null);
        }

        public IExecutableCommandRegistration AddExecutableCommand(
            string primarySelector,
            string[] aliasSelectors,
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

        public IInputCommandRegistration AddInputCommand(
            string primarySelector,
            string[] aliasSelectors,
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
            string[] aliasSelectors,
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

        public IContainerCommandRegistration RegisterContainerCommand(
            string primarySelector,
            string[] aliasSelectors,
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

        public IExecutableCommandRegistration RegisterExecutableCommand(
            string primarySelector,
            string[] aliasSelectors,
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

        public IInputCommandRegistration RegisterInputCommand(
            string primarySelector,
            string[] aliasSelectors,
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

            if (getPromptTextFunc != null && applyInputAction != null)
            {
                var descriptorContainer = iocContainer.Resolve<TDescriptorContainer>();
                var descriptor = getDescriptorFunc(descriptorContainer);

                var command = new GenericInputCommand(
                    descriptor.PrimarySelector,
                    descriptor.AliasSelectors,
                    descriptor.Name,
                    descriptor.HelpText,
                    getPromptTextFunc,
                    applyInputAction,
                    getDefaultFunc);
                registeredCommands.Add(command);
                return new CommandRegistrations(registeredCommands, command, iocContainer);
            }

            throw new Exception(
                $"{typeof(TCommand).Name} is not compatible for implementing an {nameof(IInputCommand)}.");
        }

        public IContainerCommandRegistration SetChildToContainerCommand(
            string primarySelector,
            string[] aliasSelectors,
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
            string[] aliasSelectors,
            string name,
            string helpText)
        {
            return SetChildToContainerCommand(primarySelector, aliasSelectors, name, helpText, null);
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
            string primarySelector,
            string name,
            string helpText)
        {
            return SetChildToContainerCommand(primarySelector, new string[0], name, helpText);
        }

        public IContainerCommandRegistration SetChildToContainerCommand()
        {
            return SetChildToContainerCommand(null, null, null);
        }

        public IExecutableCommandRegistration SetChildToExecutableCommand(
            string primarySelector,
            string[] aliasSelectors,
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

        public IInputCommandRegistration SetChildToInputCommand(
            string primarySelector,
            string[] aliasSelectors,
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
            string[] aliasSelectors,
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

        private Action<ICommandContext, string> GetApplyInputActionForInputCommand<TCommand>(
            Expression<Action<TCommand>> applyInputExpression,
            TCommand instance)
            where TCommand : class
        {
            Action<ICommandContext, string> applyInputAction = null;
            if (applyInputExpression?.Body is MethodCallExpression applyInputMethodExpression)
            {
                if (applyInputMethodExpression.Arguments.Count == 2
                    && applyInputMethodExpression.Arguments[0].Type.IsAssignableFrom(typeof(ICommandContext))
                    && applyInputMethodExpression.Arguments[1].Type.IsAssignableFrom(typeof(string)))
                {
                    applyInputAction = (context, input) =>
                        {
                            var target = instance ?? context.GetService<TCommand>();
                            applyInputMethodExpression.Method.Invoke(target, new object[] { context, input });
                        };
                }
            }

            return applyInputAction;
        }

        private Func<ICommandContext, string> GetDefaultFuncForInputCommand<TCommand>(
            Expression<Action<TCommand>> getDefaultExpression,
            TCommand instance)
            where TCommand : class
        {
            Func<ICommandContext, string> getDefaultFunc = null;
            if (getDefaultExpression?.Body is MethodCallExpression getDefaultMethodExpression)
            {
                if (getDefaultMethodExpression.Arguments.Count == 1
                    && getDefaultMethodExpression.Arguments[0].Type.IsAssignableFrom(typeof(ICommandContext))
                    && getDefaultMethodExpression.Method.ReturnType.IsAssignableFrom(typeof(string)))
                {
                    getDefaultFunc = context =>
                        {
                            var target = instance ?? context.GetService<TCommand>();
                            return (string)getDefaultMethodExpression.Method.Invoke(target, new object[] { context });
                        };
                }
            }

            return getDefaultFunc;
        }

        private Func<ICommandContext, string> GetPromptTextFuncForInputCommand<TCommand>(
            Expression<Action<TCommand>> getDefaultExpression,
            TCommand instance)
            where TCommand : class
        {
            Func<ICommandContext, string> getPromptTextFunc = null;
            if (getDefaultExpression?.Body is MethodCallExpression getPromptTextMethodExpression)
            {
                if (getPromptTextMethodExpression.Arguments.Count == 1
                    && getPromptTextMethodExpression.Arguments[0].Type.IsAssignableFrom(typeof(ICommandContext))
                    && getPromptTextMethodExpression.Method.ReturnType.IsAssignableFrom(typeof(string)))
                {
                    getPromptTextFunc = context =>
                        {
                            var target = instance ?? context.GetService<TCommand>();
                            return (string)getPromptTextMethodExpression.Method.Invoke(
                                target,
                                new object[] { context });
                        };
                }
            }

            return getPromptTextFunc;
        }
    }
}