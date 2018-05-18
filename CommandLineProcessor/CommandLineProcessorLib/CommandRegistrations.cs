namespace CommandLineProcessorLib
{
    using System;
    using System.Collections.Generic;

    using CommandLineProcessorContracts;

    public class CommandRegistrations : IRootCommandRegistration,
                                        IContainerCommandRegistration,
                                        IExecutableCommandRegistration,
                                        IInputCommandRegistration
    {
        private readonly List<ICommand> registeredCommands;

        private readonly ICommand targetCommand;

        protected CommandRegistrations(List<ICommand> registeredCommands, ICommand targetCommand)
        {
            this.registeredCommands = registeredCommands;
            this.targetCommand = targetCommand;
        }

        public CommandRegistrations()
            : this(new List<ICommand>(), null)
        {
        }

        public IEnumerable<ICommand> RegisteredCommands => registeredCommands;

        public IContainerCommandRegistration AddContainerCommand(ICommandDescriptor descriptor)
        {
            var command = new GenericContainerCommand(descriptor);
            command.Parent = targetCommand;
            (targetCommand as IContainerCommandEdit).AddChild(command);
            return new CommandRegistrations(registeredCommands, command);
        }

        public IExecutableCommandRegistration AddExecutableCommand(
            ICommandDescriptor descriptor,
            Action<ICommandContext> executeAction)
        {
            var command = new GenericExecutableCommand(descriptor, executeAction);
            command.Parent = targetCommand;
            (targetCommand as IContainerCommandEdit).AddChild(command);
            return new CommandRegistrations(registeredCommands, command);
        }

        public IInputCommandRegistration AddInputCommand(
            ICommandDescriptor descriptor,
            string promptText,
            Action<ICommandContext, string> applyInputAction)
        {
            var command = new GenericInputCommand(descriptor, promptText, applyInputAction);
            command.Parent = targetCommand;
            (targetCommand as IContainerCommandEdit).AddChild(command);
            return new CommandRegistrations(registeredCommands, command);
        }

        public IContainerCommandRegistration RegisterContainerCommand(ICommandDescriptor descriptor)
        {
            var command = new GenericContainerCommand(descriptor);
            registeredCommands.Add(command);
            return new CommandRegistrations(registeredCommands, command);
        }

        public IExecutableCommandRegistration RegisterExecutableCommand(
            ICommandDescriptor descriptor,
            Action<ICommandContext> executeAction)
        {
            var command = new GenericExecutableCommand(descriptor, executeAction);
            registeredCommands.Add(command);
            return new CommandRegistrations(registeredCommands, command);
        }

        public IInputCommandRegistration RegisterInputCommand(
            ICommandDescriptor descriptor,
            string promptText,
            Action<ICommandContext, string> applyInputAction)
        {
            var command = new GenericInputCommand(descriptor, promptText, applyInputAction);
            registeredCommands.Add(command);
            return new CommandRegistrations(registeredCommands, command);
        }

        public IContainerCommandRegistration SetChildToContainerCommand(ICommandDescriptor descriptor)
        {
            var command = new GenericContainerCommand(descriptor);
            command.Parent = targetCommand;
            (targetCommand as IInputCommand).NextCommand = command;
            return new CommandRegistrations(registeredCommands, command);
        }

        public IExecutableCommandRegistration SetChildToExecutableCommand(
            ICommandDescriptor descriptor,
            Action<ICommandContext> executeAction)
        {
            var command = new GenericExecutableCommand(descriptor, executeAction);
            command.Parent = targetCommand;
            (targetCommand as IInputCommand).NextCommand = command;
            return new CommandRegistrations(registeredCommands, command);
        }

        public IInputCommandRegistration SetChildToInputCommand(
            ICommandDescriptor descriptor,
            string promptText,
            Action<ICommandContext, string> applyInputAction)
        {
            var command = new GenericInputCommand(descriptor, promptText, applyInputAction);
            command.Parent = targetCommand;
            (targetCommand as IInputCommand).NextCommand = command;
            return new CommandRegistrations(registeredCommands, command);
        }
    }
}