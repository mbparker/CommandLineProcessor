namespace CommandLineLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using CommandLineLibrary.Contracts;
    using CommandLineLibrary.Contracts.Commands;
    using CommandLineLibrary.Contracts.Commands.Registration;

    public class CommandRegistrations : IRootCommandRegistration,
                                        IContainerCommandRegistration,
                                        IExecutableCommandRegistration,
                                        IInputCommandRegistration
    {
        private readonly ICommandServiceProvider commandServiceProvider;

        private readonly ICommandMethodFactoryService methodFactoryService;

        private readonly List<ICommand> registeredCommands;

        private readonly ICommand targetCommand;

        public CommandRegistrations(ICommandServiceProvider commandServiceProvider, ICommandMethodFactoryService methodFactoryService)
            : this(new List<ICommand>(), null, commandServiceProvider, methodFactoryService)
        {
        }

        protected CommandRegistrations(
            List<ICommand> registeredCommands,
            ICommand targetCommand,
            ICommandServiceProvider commandServiceProvider,
            ICommandMethodFactoryService methodFactoryService)
        {
            this.registeredCommands = registeredCommands;
            this.targetCommand = targetCommand;
            this.commandServiceProvider = commandServiceProvider;
            this.methodFactoryService = methodFactoryService;
        }

        private interface ICommandDelegateMethodSignatures
        {
            void ApplyInput(ICommandContext context, string inputText);

            void Execute(ICommandContext context);

            ICommand GetDefaultCommand(ICommandContext context, IEnumerable<ICommand> commands);

            string GetDefaultValue(ICommandContext context);

            string GetPromptText(ICommandContext context);
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
            return CreateNewInstance(command);
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
            TCommand instance = null)
            where TCommand : class
        {
            var getDefaultCommandFunc =
                methodFactoryService.CreateDefaultCommandFuncForContainerCommand(getDefaultCommandExpression, instance);

            var descriptor = ThrowIfNullDescriptor(GetCommandDescriptor(getDescriptorFunc));

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
            return CreateNewInstance(command);
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
            TCommand instance = null)
            where TCommand : class
        {
            var executeAction = methodFactoryService.CreateExecuteActionForExecuteCommand(executeExpression, instance);

            var descriptor = ThrowIfNullDescriptor(GetCommandDescriptor(getDescriptorFunc));

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
            return CreateNewInstance(command);
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
            TCommand instance = null)
            where TCommand : class
        {
            var getPromptTextFunc =
                methodFactoryService.CreatePromptTextFuncForInputCommand(getPromptTextExpression, instance);
            var applyInputAction =
                methodFactoryService.CreateApplyInputActionForInputCommand(applyInputExpression, instance);
            var getDefaultFunc = methodFactoryService.CreateDefaultFuncForInputCommand(getDefaultExpression, instance);

            var descriptor = ThrowIfNullDescriptor(GetCommandDescriptor(getDescriptorFunc));

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
            TCommand instance = null)
            where TCommand : class
        {
            return AddInputCommand(getDescriptorFunc, getPromptTextExpression, applyInputExpression, null, instance);
        }

        public void Clear()
        {
            registeredCommands.Clear();
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
            return CreateNewInstance(command);
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
            TCommand instance = null)
            where TCommand : class
        {
            var getDefaultCommandFunc =
                methodFactoryService.CreateDefaultCommandFuncForContainerCommand(getDefaultCommandExpression, instance);

            var descriptor = ThrowIfNullDescriptor(GetCommandDescriptor(getDescriptorFunc));

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
            return CreateNewInstance(command);
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
            where TCommand : class
        {
            var executeAction = methodFactoryService.CreateExecuteActionForExecuteCommand(executeExpression, instance);

            var descriptor = ThrowIfNullDescriptor(GetCommandDescriptor(getDescriptorFunc));

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
            return CreateNewInstance(command);
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
            where TCommand : class
        {
            var getPromptTextFunc =
                methodFactoryService.CreatePromptTextFuncForInputCommand(getPromptTextExpression, instance);
            var applyInputAction =
                methodFactoryService.CreateApplyInputActionForInputCommand(applyInputExpression, instance);
            var getDefaultFunc = methodFactoryService.CreateDefaultFuncForInputCommand(getDefaultExpression, instance);

            var descriptor = ThrowIfNullDescriptor(GetCommandDescriptor(getDescriptorFunc));

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
            TCommand instance = null)
            where TCommand : class
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
            return CreateNewInstance(command);
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
            TCommand instance = null)
            where TCommand : class
        {
            var getDefaultCommandFunc =
                methodFactoryService.CreateDefaultCommandFuncForContainerCommand(getDefaultCommandExpression, instance);

            var descriptor = GetCommandDescriptor(getDescriptorFunc);

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
            return CreateNewInstance(command);
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
            TCommand instance = null)
            where TCommand : class
        {
            var executeAction = methodFactoryService.CreateExecuteActionForExecuteCommand(executeExpression, instance);

            var descriptor = GetCommandDescriptor(getDescriptorFunc);

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
            return CreateNewInstance(command);
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
            TCommand instance = null)
            where TCommand : class
        {
            var getPromptTextFunc =
                methodFactoryService.CreatePromptTextFuncForInputCommand(getPromptTextExpression, instance);
            var applyInputAction =
                methodFactoryService.CreateApplyInputActionForInputCommand(applyInputExpression, instance);
            var getDefaultFunc = methodFactoryService.CreateDefaultFuncForInputCommand(getDefaultExpression, instance);

            var descriptor = GetCommandDescriptor(getDescriptorFunc);

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

        private CommandRegistrations CreateNewInstance(ICommand command)
        {
            return new CommandRegistrations(registeredCommands, command, commandServiceProvider, methodFactoryService);
        }

        private ICommandDescriptor GetCommandDescriptor<TDescriptorContainer>(
            Func<TDescriptorContainer, ICommandDescriptor> getDescriptorFunc)
        {
            ICommandDescriptor descriptor = null;
            if (getDescriptorFunc != null)
            {
                var descriptorContainer = commandServiceProvider.Resolve<TDescriptorContainer>();
                descriptor = getDescriptorFunc(descriptorContainer);
            }

            return descriptor;
        }

        private ICommandDescriptor ThrowIfNullDescriptor(ICommandDescriptor descriptor)
        {
            if (descriptor == null)
            {
                throw new ArgumentException("A command descriptor is required for this registration.");
            }

            return descriptor;
        }
    }
}