namespace CommandLineProcessorLib
{
    using System;
    using System.Collections.Generic;

    using CommandLineProcessorContracts;
    using CommandLineProcessorContracts.Commands;
    using CommandLineProcessorContracts.Commands.Registration;

    public class CommandRegistrations : IRootCommandRegistration,
                                        IContainerCommandRegistration,
                                        IExecutableCommandRegistration,
                                        IInputCommandRegistration
    {
        private readonly List<ICommand> registeredCommands;

        private readonly ICommand targetCommand;

        public CommandRegistrations()
            : this(new List<ICommand>(), null)
        {
        }

        protected CommandRegistrations(List<ICommand> registeredCommands, ICommand targetCommand)
        {
            this.registeredCommands = registeredCommands;
            this.targetCommand = targetCommand;
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
                new CommandDescriptor(primarySelector, name, helpText, aliasSelectors),
                getDefaultCommandFunc);
            command.Parent = targetCommand;
            (targetCommand as IContainerCommandEdit).AddChild(command);
            return new CommandRegistrations(registeredCommands, command);
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
            var command = new GenericExecutableCommand(
                new CommandDescriptor(primarySelector, name, helpText, aliasSelectors),
                executeAction);
            command.Parent = targetCommand;
            (targetCommand as IContainerCommandEdit).AddChild(command);
            return new CommandRegistrations(registeredCommands, command);
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
            string promptText,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc)
        {
            var command = new GenericInputCommand(
                new CommandDescriptor(primarySelector, name, helpText, aliasSelectors),
                promptText,
                applyInputAction,
                getDefaultFunc);
            command.Parent = targetCommand;
            (targetCommand as IContainerCommandEdit).AddChild(command);
            return new CommandRegistrations(registeredCommands, command);
        }

        public IInputCommandRegistration AddInputCommand(
            string primarySelector,
            string[] aliasSelectors,
            string name,
            string helpText,
            string promptText,
            Action<ICommandContext, string> applyInputAction)
        {
            return AddInputCommand(primarySelector, aliasSelectors, name, helpText, promptText, applyInputAction, null);
        }

        public IInputCommandRegistration AddInputCommand(
            string primarySelector,
            string name,
            string helpText,
            string promptText,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc)
        {
            return AddInputCommand(
                primarySelector,
                new string[0],
                name,
                helpText,
                promptText,
                applyInputAction,
                getDefaultFunc);
        }

        public IInputCommandRegistration AddInputCommand(
            string primarySelector,
            string name,
            string helpText,
            string promptText,
            Action<ICommandContext, string> applyInputAction)
        {
            return AddInputCommand(primarySelector, new string[0], name, helpText, promptText, applyInputAction, null);
        }

        public IContainerCommandRegistration RegisterContainerCommand(
            string primarySelector,
            string[] aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc)
        {
            var command = new GenericContainerCommand(
                new CommandDescriptor(primarySelector, name, helpText, aliasSelectors),
                getDefaultCommandFunc);
            registeredCommands.Add(command);
            return new CommandRegistrations(registeredCommands, command);
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
            var command = new GenericExecutableCommand(
                new CommandDescriptor(primarySelector, name, helpText, aliasSelectors),
                executeAction);
            registeredCommands.Add(command);
            return new CommandRegistrations(registeredCommands, command);
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
            string promptText,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc)
        {
            var command = new GenericInputCommand(
                new CommandDescriptor(primarySelector, name, helpText, aliasSelectors),
                promptText,
                applyInputAction,
                getDefaultFunc);
            registeredCommands.Add(command);
            return new CommandRegistrations(registeredCommands, command);
        }

        public IInputCommandRegistration RegisterInputCommand(
            string primarySelector,
            string name,
            string helpText,
            string promptText,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc)
        {
            return RegisterInputCommand(
                primarySelector,
                new string[0],
                name,
                helpText,
                promptText,
                applyInputAction,
                getDefaultFunc);
        }

        public IContainerCommandRegistration SetChildToContainerCommand(
            string primarySelector,
            string[] aliasSelectors,
            string name,
            string helpText,
            Func<ICommandContext, IEnumerable<ICommand>, ICommand> getDefaultCommandFunc)
        {
            var command = new GenericContainerCommand(
                new CommandDescriptor(primarySelector, name, helpText, aliasSelectors),
                getDefaultCommandFunc);
            command.Parent = targetCommand;
            (targetCommand as IInputCommand).NextCommand = command;
            return new CommandRegistrations(registeredCommands, command);
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
            var command = new GenericExecutableCommand(
                new CommandDescriptor(primarySelector, name, helpText, aliasSelectors),
                executeAction);
            command.Parent = targetCommand;
            (targetCommand as IInputCommand).NextCommand = command;
            return new CommandRegistrations(registeredCommands, command);
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
            string promptText,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc)
        {
            var command = new GenericInputCommand(
                new CommandDescriptor(primarySelector, name, helpText, aliasSelectors),
                promptText,
                applyInputAction,
                getDefaultFunc);
            command.Parent = targetCommand;
            (targetCommand as IInputCommand).NextCommand = command;
            return new CommandRegistrations(registeredCommands, command);
        }

        public IInputCommandRegistration SetChildToInputCommand(
            string primarySelector,
            string[] aliasSelectors,
            string name,
            string helpText,
            string promptText,
            Action<ICommandContext, string> applyInputAction)
        {
            return SetChildToInputCommand(
                primarySelector,
                aliasSelectors,
                name,
                helpText,
                promptText,
                applyInputAction);
        }

        public IInputCommandRegistration SetChildToInputCommand(
            string primarySelector,
            string name,
            string helpText,
            string promptText,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc)
        {
            return SetChildToInputCommand(
                primarySelector,
                new string[0],
                name,
                helpText,
                promptText,
                applyInputAction,
                getDefaultFunc);
        }

        public IInputCommandRegistration SetChildToInputCommand(
            string promptText,
            Action<ICommandContext, string> applyInputAction,
            Func<ICommandContext, string> getDefaultFunc)
        {
            return SetChildToInputCommand(null, null, null, promptText, applyInputAction, getDefaultFunc);
        }

        public IInputCommandRegistration SetChildToInputCommand(
            string primarySelector,
            string name,
            string helpText,
            string promptText,
            Action<ICommandContext, string> applyInputAction)
        {
            return SetChildToInputCommand(primarySelector, name, helpText, promptText, applyInputAction, null);
        }

        public IInputCommandRegistration SetChildToInputCommand(
            string promptText,
            Action<ICommandContext, string> applyInputAction)
        {
            return SetChildToInputCommand(null, null, null, promptText, applyInputAction, null);
        }
    }
}