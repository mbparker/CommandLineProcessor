﻿namespace CommandLineProcessorLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommandLineProcessorContracts;

    using CommandLineProcessorEntity;
    using CommandLineProcessorEntity.Exceptions;

    public class CommandLineProcessorProvider : ICommandLineProcessorService
    {
        private readonly ICommandPathCalculator commandPathCalculator;

        private readonly ICommandRepositoryService commandRepository;

        private readonly ICommandContextFactory contextFactory;

        private readonly Stack<CommandLineProcessorState> stateStack;

        private CommandLineProcessorState state;

        public CommandLineProcessorProvider(
            ICommandRepositoryService commandRepository,
            ICommandPathCalculator commandPathCalculator,
            ICommandContextFactory contextFactory)
        {
            this.commandRepository = commandRepository;
            this.commandPathCalculator = commandPathCalculator;
            this.contextFactory = contextFactory;
            stateStack = new Stack<CommandLineProcessorState>();
            state = new CommandLineProcessorState
                        {
                            Status = CommandLineStatus.WaitingForCommandRegistration,
                            Context = this.contextFactory.Create()
                        };
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

                if (ShouldContinuePausedCommand())
                {
                    ContinuePausedCommand(input);
                }
                else
                {
                    string[] inputElements = SplitInput(input);
                    ProcessInput(inputElements);
                }
            }
            catch (WaitForInputException)
            {
                // do nothing
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

        private void ClearCurrentState()
        {
            state.ResetInputList();
            state.Context.DataStore.Clear();
        }

        private void ContinuePausedCommand(string input)
        {
            var remainingInputs = GetRemainingCommandInputs();
            remainingInputs.Insert(0, input);
            ProcessInput(remainingInputs);
        }

        private bool FullyCompleted()
        {
            return StackDepth == 0 && ActiveCommand == null;
        }

        private string GetFullyQualifiedInput(string input)
        {
            return commandPathCalculator.CalculateFullyQualifiedPath(ActiveCommand, input);
        }

        private List<string> GetRemainingCommandInputs()
        {
            var remainingInputs = new List<string>();
            for (int i = state.InputIndex; i < state.InputList.Count; i++)
            {
                remainingInputs.Add(state.InputList[i]);
            }

            state.ResetInputList();
            return remainingInputs;
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

            if (FullyCompleted())
            {
                ClearCurrentState();
            }
        }

        private void HandleInput(string input)
        {
            try
            {
                (ActiveCommand as IInputCommand)?.ApplyInput(state.Context, input);
            }
            catch (WaitForInputException)
            {
                throw;
            }
            catch (Exception e)
            {
                ProcessInputError?.Invoke(this, new CommandLineErrorEventArgs(e));
                return;
            }

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
            var inputList = new List<string>(inputs);
            for (int i = 0; i < inputList.Count; i++)
            {
                try
                {
                    ProcessInputBasedOnState(inputList[i]);
                }
                catch (WaitForInputException)
                {
                    if (i < inputList.Count - 1)
                    {
                        state.InputList.AddRange(inputList);
                        state.InputIndex = i + 1;
                    }

                    throw;
                }
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

            if (input.ToUpper() == Settings.PauseForUserInputToken.ToUpper())
            {
                throw new WaitForInputException();
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
            try
            {
                ActiveCommand = commandRepository[fullyQualifiedInput];
            }
            catch (Exception e)
            {
                ProcessInputError?.Invoke(this, new CommandLineErrorEventArgs(e));
            }
        }

        private bool ShouldContinuePausedCommand()
        {
            return state.InputList.Count > 0 && state.InputIndex >= 0;
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
            state = new CommandLineProcessorState
                        { Status = CommandLineStatus.WaitingForCommand, Context = contextFactory.Create() };
        }

        private void TryExecuteActiveCommand()
        {
            if (ActiveCommand is IExecutableCommand exe)
            {
                exe.Execute(state.Context, exe.GetArguments(state.Context));
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
            public CommandLineProcessorState()
            {
                InputList = new List<string>();
                ResetInputList();
            }

            public ICommand Command { get; set; }

            public ICommandContext Context { get; set; }

            public int InputIndex { get; set; }

            public List<string> InputList { get; }

            public CommandLineStatus Status { get; set; }

            public void ResetInputList()
            {
                InputList.Clear();
                InputIndex = -1;
            }
        }
    }
}