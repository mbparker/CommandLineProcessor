namespace CommandLineProcessorLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommandLineProcessorContracts;

    using CommandLineProcessorEntity;

    public class CommandLineProcessorProvider : ICommandLineProcessorService
    {
        private readonly ICommandPathCalculator commandPathCalculator;

        private readonly ICommandRepositoryService commandRepository;

        private readonly Stack<CommandLineProcessorState> stateStack;

        private CommandLineProcessorState state;

        public CommandLineProcessorProvider(
            ICommandRepositoryService commandRepository,
            ICommandPathCalculator commandPathCalculator)
        {
            this.commandRepository = commandRepository;
            this.commandPathCalculator = commandPathCalculator;
            stateStack = new Stack<CommandLineProcessorState>();
            state = new CommandLineProcessorState { Status = CommandLineStatus.WaitingForCommandRegistration };
            Settings = new CommandLineSettings();
        }

        public event EventHandler<CommandLineCommandChangedEventArgs> ActiveCommandChanged;

        public event EventHandler<CommandLineErrorEventArgs> CommandRegistrationError;

        public event EventHandler<CommandLineProcessInputEventArgs> ProcessingInputElement;

        public event EventHandler<CommandLineProcessInputEventArgs> ProcessingRawInput;

        public event EventHandler<CommandLineErrorEventArgs> ProcessInputError;

        public event EventHandler<CommandLineStatusChangedEventArgs> StatusChangedEvent;

        public ICommand ActiveCommand
        {
            get => state.Command;
            private set
            {
                if (state.Command != value)
                {
                    var priorCommand = state.Command;
                    state.Command = value;
                    ActiveCommandChanged?.Invoke(
                        this,
                        new CommandLineCommandChangedEventArgs(priorCommand, state.Command));
                }
            }
        }

        public string LastInput { get; private set; }

        public string LastRawInput { get; private set; }

        public CommandLineSettings Settings { get; set; }

        public int StackDepth => stateStack.Count;

        public CommandLineStatus Status
        {
            get => state.Status;
            private set
            {
                if (state.Status != value)
                {
                    var oldStatus = state.Status;
                    state.Status = value;
                    StatusChangedEvent?.Invoke(this, new CommandLineStatusChangedEventArgs(oldStatus, state.Status));
                }
            }
        }

        public void ProcessInput(string input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    throw new ArgumentException("Value is required.", nameof(input));
                }

                input = input.Trim();
                NotifyNewRawInput(input);
                string[] inputElements = SplitInput(input);
                ProcessInput(inputElements);
            }
            catch (Exception e)
            {
                ActiveCommand = null;
                Status = CommandLineStatus.WaitingForCommand;
                stateStack.Clear();
                ProcessInputError?.Invoke(this, new CommandLineErrorEventArgs(e));
            }
        }

        public void RegisterCommands(IEnumerable<ICommand> commands)
        {
            try
            {
                commandRepository.Load(commands);
                Status = CommandLineStatus.WaitingForCommand;
            }
            catch (Exception e)
            {
                commandRepository.Clear();
                Status = CommandLineStatus.WaitingForCommandRegistration;
                CommandRegistrationError?.Invoke(this, new CommandLineErrorEventArgs(e));
            }
        }

        private bool ActiveCommandHasNoChildren()
        {
            return !((ActiveCommand as IContainerCommand)?.Children.Any() ?? false);
        }

        private bool ActiveCommandHasOneChild()
        {
            return ((ActiveCommand as IContainerCommand)?.Children.Count() ?? 0) == 1;
        }

        private void CancelCurrentCommand()
        {
            ActiveCommand = ActiveCommand?.Parent;
            while (ActiveCommand != null)
            {
                if (ActiveCommand.CommandIs<IInputCommand>() || ActiveCommand.CommandIs<IContainerCommand>())
                {
                    UpdateStateForEndOfProcessing();
                    return;
                }

                ActiveCommand = ActiveCommand.Parent;
            }

            UpdateStateForEndOfProcessing();
        }

        private string GetFullyQualifiedInput(string input)
        {
            return commandPathCalculator.CalculateFullyQualifiedPath(ActiveCommand, input);
        }

        private string GetTransparentCommandInput(string input)
        {
            return input.Remove(0, Settings.SuspendActiveCommandToken.Length);
        }

        private void HandleCommand(string input)
        {
            var fullyQualifiedInput = GetFullyQualifiedInput(input);
            SelectActiveCommandFromQuialifiedInput(fullyQualifiedInput);
            HandleCommand();
        }

        private void HandleCommand()
        {
            if (ActiveCommand.CommandIs<IContainerCommand>())
            {
                if (ActiveCommandHasNoChildren())
                {
                    TryExecuteActiveCommand();
                }
                else if (ActiveCommandHasOneChild())
                {
                    MakeSingleChildActive();
                    TryExecuteActiveCommand();
                }
            }
            else if (ActiveCommand.CommandIs<IExecutableCommand>())
            {
                TryExecuteActiveCommand();
            }

            UpdateStateForEndOfProcessing();
            TryResumePreviousCommand();
        }

        private void HandleInput(string input)
        {
            (ActiveCommand as IInputCommand)?.ApplyInput(input);
            ActiveCommand = (ActiveCommand as IInputCommand)?.NextCommand;
            HandleCommand();
        }

        private void MakeSingleChildActive()
        {
            ActiveCommand = (ActiveCommand as IContainerCommand)?.Children.First();
        }

        private void NotifyNewInput(string input)
        {
            LastInput = input;
            ProcessingInputElement?.Invoke(this, new CommandLineProcessInputEventArgs(input));
        }

        private void NotifyNewRawInput(string input)
        {
            LastRawInput = input;
            ProcessingRawInput?.Invoke(this, new CommandLineProcessInputEventArgs(input));
        }

        private void ProcessInput(IEnumerable<string> inputs)
        {
            foreach (var input in inputs)
            {
                ProcessInputBasedOnState(input);
            }
        }

        private void ProcessInputBasedOnState(string input)
        {
            input = input.Trim();
            if (ShouldSuspendCurrentCommand(input))
            {
                var newInput = GetTransparentCommandInput(input);
                SuspendCurrentCommand();
                ProcessInputBasedOnState(newInput);
                return;
            }

            NotifyNewInput(input);

            if (input.ToUpper() == Settings.CancelToken.ToUpper())
            {
                CancelCurrentCommand();
            }
            else if (Status == CommandLineStatus.WaitingForCommand)
            {
                HandleCommand(input);
            }
            else if (Status == CommandLineStatus.WaitingForInput)
            {
                HandleInput(input);
            }
        }

        private void SelectActiveCommandFromQuialifiedInput(string fullyQualifiedInput)
        {
            ActiveCommand = commandRepository[fullyQualifiedInput];
        }

        private bool ShouldSuspendCurrentCommand(string input)
        {
            return input.StartsWith(Settings.SuspendActiveCommandToken, StringComparison.CurrentCultureIgnoreCase);
        }

        private string[] SplitInput(string input)
        {
            return input.Split(new[] { Settings.CommandSeparatorToken }, StringSplitOptions.RemoveEmptyEntries);
        }

        private void SuspendCurrentCommand()
        {
            if (stateStack.Count == Settings.MaximumStackSize)
            {
                throw new InvalidOperationException(
                    $"The current command cannot be suspended. The maximum command stack size of {Settings.MaximumStackSize} has been reached.");
            }

            stateStack.Push(state);
            state = new CommandLineProcessorState { Status = CommandLineStatus.WaitingForCommand };
        }

        private void TryExecuteActiveCommand()
        {
            if (ActiveCommand is IExecutableCommand exe)
            {
                exe.Execute(exe.GetArguments());
                ActiveCommand = null;
            }
        }

        private void TryResumePreviousCommand()
        {
            if (ActiveCommand == null && stateStack.Any())
            {
                state = stateStack.Pop();
            }
        }

        private void UpdateStateForEndOfProcessing()
        {
            if (ActiveCommand?.CommandIs<IInputCommand>() ?? false)
            {
                Status = CommandLineStatus.WaitingForInput;
            }
            else
            {
                Status = CommandLineStatus.WaitingForCommand;
            }
        }

        private class CommandLineProcessorState
        {
            public ICommand Command { get; set; }

            public CommandLineStatus Status { get; set; }
        }
    }
}